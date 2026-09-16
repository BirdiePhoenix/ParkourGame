using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour
{
    // maybe add events from movement to trigger player events?
    private AudioClip[] _obtainableSounds;
    [SerializeField] private GameObject playerObject;
    private PlayerMovement _playerMovement;

    private AudioSource _jumpAudio;
    private AudioSource _walkAudio;
    private AudioSource _sprintAudio;
    private AudioSource _slideAudio;

    private bool _sprinting;

    private Dictionary<PlayerSoundType, List<AudioClip>> _clipsByType;
    private Dictionary<PlayerSoundType, AudioSource> _sourcesByType;

    private enum PlayerSoundType
    {
        Jump,
        Walk,
        Sprint,
        Slide
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

        int obtainableSoundsCount = 0;
        //loads all sfx once
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");

        //puts each sfx in its own category
        foreach (AudioClip clip in clips)
        {
            switch (clip.name)
            {
                case string cname when cname.Contains("Jump"):
                    _clipsByType[PlayerSoundType.Jump].Add(clip);
                    break;
                case string cname when cname.Contains("Slide"):
                    _clipsByType[PlayerSoundType.Slide].Add(clip);
                    break;
                case string cname when cname.Contains("Walk"):
                    _clipsByType[PlayerSoundType.Walk].Add(clip);
                    break;
                case string cname when cname.Contains("Sprint"):
                    _clipsByType[PlayerSoundType.Sprint].Add(clip);
                    break;
            }

            obtainableSoundsCount++;
        }

        //creates the array for all the other sfx's
        _obtainableSounds = new AudioClip[obtainableSoundsCount];
        for (int i = 0; i < obtainableSoundsCount; i++)
        {
            _obtainableSounds[i] = clips[i];
        }
    }

    private bool CurrentlyTouchingGround()
    {
        return _playerMovement.grounded;
    }


    private AudioSource CreateAudioSource()
    {
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

        AudioSource source;
        if (!_sourcesByType.TryGetValue(soundType, out source) || source == null)
        {
            source = CreateAudioSource();
            _sourcesByType[soundType] = source;
        }

        return source;
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

        _playerMovement.moveAction.performed -= moveAction_performed;
        _playerMovement.moveAction.canceled -= moveAction_canceled;
        _playerMovement.slideAction.performed -= SlideAction_performed;
        _playerMovement.slideAction.canceled -= SlideAction_canceled;
        _playerMovement.jumpAction.performed -= JumpAction_performed;
        _playerMovement.sprintAction.performed -= SprintAction_performed;
        _playerMovement.sprintAction.canceled -= SprintAction_canceled;
    }


    //actions calling the right function
    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTouchingGround())
            return;
        PlayRandomSoundOfType(PlayerSoundType.Jump);
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTouchingGround())
            return;

        _sprinting = true;
        PlayRandomSoundOfType(PlayerSoundType.Sprint);
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        _sprinting = false;
        AudioSource sprintAudio = GetValidAudioSource(PlayerSoundType.Sprint);
        if (sprintAudio != null)
            sprintAudio.Stop();
    }

    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTouchingGround())
            return;
        PlayRandomSoundOfType(PlayerSoundType.Slide);
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        //finds the correct audio source and stops it
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

        //gets player movement
        bool moving = _playerMovement.moveAction.ReadValue<Vector2>().sqrMagnitude > 0.01f;
        // if it is not moving or not touching ground stops both sprint and walk sfx
        if (!CurrentlyTouchingGround() || !moving)
        {
            StopMovementSfx();
            return;
        }
        //plays correct sound
        PlayerSoundType soundType = _sprinting ? PlayerSoundType.Sprint : PlayerSoundType.Walk;
        PlayRandomSoundOfType(soundType, true);
    }

    private void StopMovementSfx()
    {
        AudioSource sprintAudio = GetValidAudioSource(PlayerSoundType.Sprint);
        AudioSource walkAudio = GetValidAudioSource(PlayerSoundType.Walk);

        if (sprintAudio != null)
            sprintAudio.Stop();

        if (walkAudio != null)
            walkAudio.Stop();
    }


    private void PlayRandomSoundOfType(PlayerSoundType sound, bool loop = false)
    {
        List<AudioClip> clips = _clipsByType[sound];

        if (clips.Count == 0)
            return;

        AudioSource audio = GetValidAudioSource(sound);
        if (audio == null)
            return;

        audio.loop = loop;
        audio.clip = clips[Random.Range(0, clips.Count)];
        if (!audio.isPlaying)
            audio.Play();
    }


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
}