using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour
{
    //GAME
    private AudioClip[] _obtainableSounds;
    [SerializeField] private GameObject playerObject;
    private PlayerMovement _playerMovement;

    private AudioSource _jumpAudio;
    private AudioSource _walkAudio;
    private AudioSource _sprintAudio;
    private AudioSource _slideAudio;
    private AudioSource _slideCancel;

    private bool _sprinting;
    private bool _wasWallrunning;
    private bool _isSlidingAudio;
    private bool _jumpAudioPlayed;
    private bool _wasGrounded;

    private Dictionary<PlayerSoundType, List<AudioClip>> _clipsByType;
    private Dictionary<PlayerSoundType, AudioSource> _sourcesByType;

    private enum PlayerSoundType
    {
        Jump,
        Walk,
        Sprint,
        Slide,
        Vault
    }

    private void Awake()
    {
        //initalized audio clips and stores them in an array and player sfx in a dictionary with player sounds
        _clipsByType = new Dictionary<PlayerSoundType, List<AudioClip>>();
        _sourcesByType = new Dictionary<PlayerSoundType, AudioSource>();
        
        _clipsByType.Add(PlayerSoundType.Jump, new List<AudioClip>());
        _clipsByType.Add(PlayerSoundType.Walk, new List<AudioClip>());
        _clipsByType.Add(PlayerSoundType.Sprint, new List<AudioClip>());
        _clipsByType.Add(PlayerSoundType.Slide, new List<AudioClip>());
        _clipsByType.Add(PlayerSoundType.Vault, new List<AudioClip>());

        int obtainableSoundsCount = 0;
        //loads all sfx once as to not create lagg
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");

        //puts each sfx in its own category
        foreach (AudioClip clip in clips)
        {
            switch (clip.name)
            {
                case string cname when cname.Contains("Jump"):
                    _clipsByType[PlayerSoundType.Jump].Add(clip);
                    break;
                case string cname when cname.Contains("Slide") &&
                                       !cname.Equals("slide_cancel", StringComparison.OrdinalIgnoreCase):
                    _clipsByType[PlayerSoundType.Slide].Add(clip);
                    break;
                case string cname when cname.Contains("Walk"):
                    _clipsByType[PlayerSoundType.Walk].Add(clip);
                    break;
                case string cname when cname.Contains("Sprint"):
                    _clipsByType[PlayerSoundType.Sprint].Add(clip);
                    break;
                case string cname when cname.Contains("Vault"):
                    _clipsByType[PlayerSoundType.Vault].Add(clip);
                    break;
            }
            obtainableSoundsCount++;
        }
        
        //creates the array for all the other sfx's as to be easily obtainable
        _obtainableSounds = new AudioClip[obtainableSoundsCount];
        for (int i = 0; i < obtainableSoundsCount; i++)
        {
            _obtainableSounds[i] = clips[i];
        }
    }
    
    private void Start()
    {
       
        //gets player actions
        _playerMovement = playerObject.GetComponent<PlayerMovement>();
        _playerMovement.moveAction.performed += moveAction_performed;
        _playerMovement.slideAction.performed += SlideAction_performed;
        _playerMovement.jumpAction.performed += JumpAction_performed;
        _playerMovement.sprintAction.performed += SprintAction_performed;
        _playerMovement.sprintAction.canceled += SprintAction_canceled;
        _playerMovement.moveAction.canceled += moveAction_canceled;
        _playerMovement.slideAction.canceled += SlideAction_canceled;

        //creates an audio source for each audio
        _jumpAudio = CreateAudioSource();
        _sourcesByType.Add(PlayerSoundType.Jump,_jumpAudio);
        _walkAudio = CreateAudioSource();
        _sourcesByType.Add(PlayerSoundType.Walk,_walkAudio);
        _sprintAudio = CreateAudioSource();
        _sourcesByType.Add(PlayerSoundType.Sprint,_sprintAudio);
        _slideAudio = CreateAudioSource();
        _sourcesByType.Add(PlayerSoundType.Slide,_slideAudio);
    }
    
    private void OnDestroy()
    {
        if (_playerMovement == null)
            return;
        //releases all the actions
        _playerMovement.moveAction.performed -= moveAction_performed;
        _playerMovement.moveAction.canceled -= moveAction_canceled;
        _playerMovement.slideAction.performed -= SlideAction_performed;
        _playerMovement.slideAction.canceled -= SlideAction_canceled;
        _playerMovement.jumpAction.performed -= JumpAction_performed;
        _playerMovement.sprintAction.performed -= SprintAction_performed;
        _playerMovement.sprintAction.canceled -= SprintAction_canceled;
    }
    
    private void Update()
    {
        if (_playerMovement == null)
            return;

        bool wallrunning = CurrentlyWallrunning();
        if (wallrunning != _wasWallrunning)
        {
            //matches _wasWallrunning to current state and plays the correct sound type
            _wasWallrunning = wallrunning;
            if (wallrunning)
            {
                StopMovementSfx();
                PlayRandomSoundOfType(PlayerSoundType.Sprint, true);
            }
            else
            {
                AudioSource sprintAudio = GetValidAudioSource(PlayerSoundType.Sprint);
                if (sprintAudio != null)
                    sprintAudio.Stop();
            }
        }
        //updates grounded and continues playing the walk sound
        bool grounded = CurrentlyTouchingGround();
        if (grounded && !_wasGrounded)
            UpdateMovementSfx();

        //updates _wasgrounded
        _wasGrounded = grounded;
        if (!grounded)
            _jumpAudioPlayed = false;
    }

    private bool CurrentlyTouchingGround()
    {
        return _playerMovement.grounded;
    }

    private bool CurrentlyWallrunning()
    {
        return _playerMovement.IsWallRunning;
    }
    
    private AudioSource CreateAudioSource()
    {
        //if player exist then add an audio source component and return it
        if (playerObject == null)
            return null;
        
        AudioSource source = playerObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private AudioSource GetValidAudioSource(PlayerSoundType soundType)
    {
        if (playerObject == null)
            return null;
        //if there is no audio source or it cant find it then create a new one and add it to the _sources dict and also returns it
        AudioSource source;
        if (!_sourcesByType.TryGetValue(soundType, out source) || source == null)
        {
            source = CreateAudioSource();
            _sourcesByType[soundType] = source;
        }
        return source;
    }
    
    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        //checks to make sure it does not play twice
        if (_jumpAudioPlayed)
            return;

        //stops the walk/sprint sfx
        _jumpAudioPlayed = true;
        StopMovementSfx();
        if (!CurrentlyTouchingGround())
        {
            if (CurrentlyWallrunning())
            {
                PlayRandomSoundOfType(PlayerSoundType.Jump);
            }
            return;
        }

        //plays jump+vault or only jump
        PlayRandomSoundOfType(PlayerSoundType.Jump);
        if (_playerMovement.vaulting)
        {
            PlayRandomSoundOfType(PlayerSoundType.Vault);
        }
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTouchingGround())
            return;
        //plays sprint sound
        _sprinting = true;
        PlayRandomSoundOfType(PlayerSoundType.Sprint);
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        //configures values and stops sfx
        _sprinting = false;
        AudioSource sprintAudio = GetValidAudioSource(PlayerSoundType.Sprint);
        if (sprintAudio != null)
            sprintAudio.Stop();
    }

    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTouchingGround() || _isSlidingAudio)
            return;

        _isSlidingAudio = true;
        PlayRandomSoundOfType(PlayerSoundType.Slide);
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        if (!_isSlidingAudio)
            return;
        //configures values and stops sfx
        _isSlidingAudio = false;
        AudioSource slideAudio = GetValidAudioSource(PlayerSoundType.Slide);
        if (slideAudio != null)
            slideAudio.Stop();
    }

    private void moveAction_performed(InputAction.CallbackContext obj)
    {
        UpdateMovementSfx();
    }

    private void moveAction_canceled(InputAction.CallbackContext obj)
    {
        StopMovementSfx();
    }
    
    private void UpdateMovementSfx()
    {
        if (_playerMovement == null)
            return;

        //gets if player is moving
        bool moving = _playerMovement.moveAction.ReadValue<Vector2>().sqrMagnitude > 0.01f;

        // if not touching ground but is wallrunning then play sfx else if its not touching ground or not moving then stop movementsound
        if (!CurrentlyTouchingGround() || !moving)
        {
            if (CurrentlyWallrunning())
            {
                PlayRandomSoundOfType(PlayerSoundType.Sprint, true);
                return;
            }

            StopMovementSfx();
            return;
        }
        
        //if it is touching ground or moving then plays the correct sound depending on walk or sprint
        PlayerSoundType soundType = _sprinting ? PlayerSoundType.Sprint : PlayerSoundType.Sprint;
        PlayRandomSoundOfType(soundType, true);
    }

    private void StopMovementSfx()
    {
        //gets sources and stops them if not null
        AudioSource sprintAudio = GetValidAudioSource(PlayerSoundType.Sprint);
        AudioSource walkAudio = GetValidAudioSource(PlayerSoundType.Walk);

        if (sprintAudio != null)
            sprintAudio.Stop();

        if (walkAudio != null)
            walkAudio.Stop();
    }

    private void PlayRandomSoundOfType(PlayerSoundType sound, bool loop = false)
    {
        //gets all the clips by the type and a valid source
        List<AudioClip> clips = _clipsByType[sound];

        if (clips.Count == 0)
            return;

        AudioSource audio = GetValidAudioSource(sound);
        if (audio == null)
            return;
        //plays a random sound from that list
        audio.loop = loop;
        audio.clip = clips[Random.Range(0, clips.Count)];
        if (!audio.isPlaying)
            audio.Play();
    }
    
    
    /* [CURRENTLY UNUSED POSSIBLE TO DELETE BEFORE SHIPPING]
    public void PlaySFX(string soundName, bool loops, GameObject objectToBePlayedOn, bool createNewObject = false,
        string newObjectName = "Created_SFX_Object", Vector3 newObjectPosition = new Vector3())
    {
        //iterates to the right sound
        for (int i = 0; i < _obtainableSounds.Length; i++)
        {
            if (_obtainableSounds[i].name != soundName)
            {
                continue;
            }

            if (objectToBePlayedOn == null)
            {
                // if no gameobject or position then 
                if (!createNewObject)
                {
                    Debug.Log($"GameObject createNewObject = false");
                    return;
                }
                else //else it will create a new item
                {
                    // iterates through every items
                    for (int v = 0; v < Resources.LoadAll<GameObject>("Items").Length; v++)
                    {
                        if (Resources.LoadAll<GameObject>("Items")[v].name == newObjectName)
                        {
                            //spawns a new gameobject
                            GameObject newObj = new GameObject(newObjectName);
                            // if a position is set then set it or throw an error
                            try
                            {
                                newObj.transform.position = newObjectPosition;
                            }
                            catch (Exception err)
                            {
                                Debug.Log($"You forgot position values! {err}");
                                return;
                            }

                            objectToBePlayedOn = newObj;
                        }
                        else
                        {
                            // if it cant find the object in Items folder
                            Debug.Log($"{newObjectName} prefab doesn't exist");
                        }
                    }
                }
            }

            // if it does not have audio source then add it
            if (objectToBePlayedOn.GetComponent<AudioSource>() == null)
            {
                objectToBePlayedOn.AddComponent<AudioSource>();
            }

            AudioSource source = objectToBePlayedOn.GetComponent<AudioSource>();
            source.loop = loops;
            if (loops)
            {
                source.clip = _obtainableSounds[i];
                source.Play();
            }
            else
            {
                source.PlayOneShot(_obtainableSounds[i]);
            }
        }
    }
    */
}