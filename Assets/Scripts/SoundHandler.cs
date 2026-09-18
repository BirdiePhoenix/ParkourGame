using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour
{
    
    // maybe add events from movement to trigger player events?
    private AudioClip[] _obtainableSounds;
    private List<GameObject> _audioInScene;
    [SerializeField] private GameObject playerObject;
    private PlayerMovement _playerMovement;
    
    private AudioSource _jumpAudio;
    private AudioSource _walkAudio;
    private AudioSource _sprintAudio;
    private AudioSource _slideAudio;
    
    private List<AudioClip> _jumpSfx;
    private List<AudioClip> _walkSfx;
    private List<AudioClip> _sprintSfx;
    private List<AudioClip> _slideSfx;
    
    private bool _sprinting;
    
    
    private void Awake()
    {
        //initalized audio clips and stores them in an array and player sfx in a list to easily choose a random sound
        _jumpSfx = new List<AudioClip>();
        _walkSfx = new List<AudioClip>();
        _sprintSfx = new List<AudioClip>();
        _slideSfx = new List<AudioClip>();
        
        
        int obtainableSoundsCount = 0;
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("SFX"))
        {
            if (clip.name.Contains("Jump"))
            {
                _jumpSfx.Add(clip);
            }
            if (clip.name.Contains("Slide"))
            {
                _slideSfx.Add(clip);
            }
            if (clip.name.Contains("Walk"))
            {
                _walkSfx.Add(clip);
            }
            if (clip.name.Contains("Sprint"))
            {
                _sprintSfx.Add(clip);
            }
            
            obtainableSoundsCount++;
        }
        
        _obtainableSounds =  new AudioClip[obtainableSoundsCount];
        for (int i = 0; i < obtainableSoundsCount; i++)
        {
            _obtainableSounds[i] = Resources.LoadAll<AudioClip>("SFX")[i];
        }
    }

    private bool CurrentlyTochingGround()
    {
        return _playerMovement.grounded;
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


        if (!playerObject.GetComponent<AudioSource>().clip)
        {
            _jumpAudio = playerObject.GetComponent<AudioSource>();
        }
        if (!playerObject.GetComponent<AudioSource>().clip)
        {
            _walkAudio = playerObject.GetComponent<AudioSource>();
        }
        if (!playerObject.GetComponent<AudioSource>().clip)
        {
            _sprintAudio = playerObject.GetComponent<AudioSource>();
        }
        if (!playerObject.GetComponent<AudioSource>().clip)
        {
            _slideAudio = playerObject.GetComponent<AudioSource>();
        }
        
    }

    private void CheckAudioComponent(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            audioSource = playerObject.GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = playerObject.AddComponent<AudioSource>();
        }
    }
    
    //actions calling the right function
    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        // if its touching ground then play a random jump sfx
        if (!CurrentlyTochingGround())
            return;
        AudioClip sound = _jumpSfx[Random.Range(0, _jumpSfx.Count)];
        _jumpAudio.clip = sound;
        _jumpAudio.Play();
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTochingGround())
            return;
        AudioClip sound = _sprintSfx[Random.Range(0, _sprintSfx.Count)];
        _sprintAudio.clip = sound;
        _sprintAudio.Play();
    }

    private void SprintAction_canceled(InputAction.CallbackContext obj)
    {
        _sprinting = false;
        _sprintAudio.Stop();
    }
    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        if (!CurrentlyTochingGround())
            return;
        AudioClip sound = _slideSfx[Random.Range(0, _slideSfx.Count)];
        _slideAudio.clip = sound;
        _slideAudio.Play();
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        _slideAudio.Stop();
    }

    private void moveAction_performed(InputAction.CallbackContext obj)
    {
        UpdateMovementSFX();
    }

    private void moveAction_canceled(InputAction.CallbackContext obj)
    {
        StopMovementSfx();

    }

    void startSprintSfx()
    {
        if (!CurrentlyTochingGround())
            return;
        AudioClip sound = _slideSfx[Random.Range(0, _slideSfx.Count)];
        _slideAudio.clip = sound;
        _slideAudio.Play();
    }

    void startWalkSfx()
    {
        if (!CurrentlyTochingGround())
            return;
        AudioClip sound = _walkSfx[Random.Range(0, _walkSfx.Count)];
        _walkAudio.clip = sound;
        _walkAudio.Play();
    }

    private void UpdateMovementSFX()
    {
        //gets player movement
        bool moving = _playerMovement.moveAction.ReadValue<Vector2>().sqrMagnitude > 0.01f;
        // if it is not moving or not touching ground stop the current walk sfx
        if (!CurrentlyTochingGround() || !moving)
        {
            StopMovementSfx();
            return;
        }
        if (_sprinting)
        {
            startSprintSfx();
        }
        else
        {
            startWalkSfx();
        }
        
        
    }

    private void StopMovementSfx()
    {
        // if it can find audio then stop it and reset clip
        _sprintAudio.Stop();
        _walkAudio.Stop();
    }

    public void PlaySFX(string soundName, bool loops, GameObject objectToBePlayedOn,bool createNewObject = false,string newObjectName = "Created_SFX_Object",Vector3 newObjectPosition = new Vector3())
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
            Debug.Log(objectToBePlayedOn);
        }
    }
}
