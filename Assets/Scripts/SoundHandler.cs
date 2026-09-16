using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour
{
    
    // maybe add events from movement to trigger player events?
    private AudioClip[] AvaibleSounds;
    private List<GameObject> _audioInScene;
    private string _pathToSFX = Path.Combine(Application.dataPath, "SFX");
    [SerializeField] private GameObject playerObject;
    private PlayerMovement _playerMovement;

    private string[] JumpSFX;
    

    bool walking = false;
    
    private void Awake()
    {
        JumpSFX = new string[3];
        JumpSFX[0] = "jump01";
        JumpSFX[1] = "jump02";
        JumpSFX[2] = "jump03";

        
        
        int _AvaibleSoundCount = 0;
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>("SFX"))
        {
            _AvaibleSoundCount++;
        }
        AvaibleSounds =  new AudioClip[_AvaibleSoundCount];
        for (int i = 0; i < _AvaibleSoundCount; i++)
        {
            AvaibleSounds[i] = Resources.LoadAll<AudioClip>("SFX")[i];
        }
    }

    private void Start()
    {
        _playerMovement = playerObject.GetComponent<PlayerMovement>();
        _playerMovement.moveAction.performed += moveAction_performed;
        _playerMovement.slideAction.performed += SlideAction_performed;
        _playerMovement.jumpAction.performed += JumpAction_performed;
        _playerMovement.sprintAction.performed += SprintAction_performed;
        
        _playerMovement.moveAction.canceled += moveAction_canceled;
    }

    private void JumpAction_performed(InputAction.CallbackContext obj)
    {
        if (!_playerMovement.TouchingGround)
        {
            return;
        }
        string jumpSound = JumpSFX[Random.Range(0,3)];
        PlaySFX($"{jumpSound}", false, playerObject,false,"");
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        if (!_playerMovement.TouchingGround)
        {
            return;
        }
        PlaySFX("Run_Asphalt", false, playerObject,false,"");
    }
    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        if (!_playerMovement.TouchingGround)
        {
            return;
        }
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        
    }

    private void moveAction_performed(InputAction.CallbackContext obj)
    {
        if (!walking && _playerMovement.TouchingGround)
        {
            PlaySFX("Walk_Asphalt",false,playerObject);
            walking = !walking;
        }
    }

    private void moveAction_canceled(InputAction.CallbackContext obj)
    {
        StopSFX("Player");
        walking = false;
    }
    
    public void StopSFX(string name)
    {
        GameObject obj = GameObject.Find(name);
        obj.GetComponent<AudioSource>().Stop();
    }


    
    public void PlaySFX(string soundName, bool loops, GameObject objectToBePlayedOn,bool createNewObject = false,string newObjectName = "",Vector3 newObjectPosition = new Vector3())
    {
        //iterates to the right sound
        for (int i = 0; i < AvaibleSounds.Length; i++)
        {
            if (AvaibleSounds[i].name != soundName)
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
                else
                {
                    //else it will create 
                    for (int v = 0; v < Resources.LoadAll<GameObject>("itemsToSpawn").Length; v++)
                    {
                        if (Resources.LoadAll<GameObject>("itemsToSpawn")[v].name == newObjectName)
                        {
                            GameObject newObj = new GameObject(newObjectName);
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
                            Debug.Log($"{newObjectName} prefab doesn't exist");
                        }
                    }
                }
            }
            Debug.Log(objectToBePlayedOn);
            // if it does not have audio source then add it
            if (objectToBePlayedOn.GetComponent<AudioSource>() == null)
            {
                objectToBePlayedOn.AddComponent<AudioSource>();
            }
            // if loop is true then set it to loop
            if (loops == true)
            {
                objectToBePlayedOn.GetComponent<AudioSource>().loop = true;
            }
            //finally plays the sound
            objectToBePlayedOn.GetComponent<AudioSource>().PlayOneShot(AvaibleSounds[i]);
            
            Debug.Log(objectToBePlayedOn);
        }
    }
}
