using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SoundHandler : MonoBehaviour
{
    
    // maybe add events from movement to trigger player events?
    private AudioClip[] AvaibleSounds;
    private List<GameObject> _audioInScene;
    private string _pathToSFX = System.IO.Path.Combine(Application.dataPath, "SFX");
    [SerializeField]private GameObject playerObject;
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
        string jumpSound = JumpSFX[Random.Range(0,3)];
        PlaySFX($"{jumpSound},false,Player");
    }

    private void SprintAction_performed(InputAction.CallbackContext obj)
    {
        PlaySFX($"Run_Asphalt,false,Player");
    }
    private void SlideAction_performed(InputAction.CallbackContext obj)
    {
        
    }

    private void SlideAction_canceled(InputAction.CallbackContext obj)
    {
        
    }

    private void moveAction_performed(InputAction.CallbackContext obj)
    {
        if (!walking)
        {
            PlaySFX("Walk_Asphalt,false,Player");
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
    public void PlaySFX(string args)
    {
        //[0] SFX NAME [1] Loops [2] GameObject [3,4,5] PosX,PosY,PosZ
        string[] argsArray = args.Split(',');
        //iterates to the right sound
        for (int i = 0; i < AvaibleSounds.Length; i++)
        {
            if (AvaibleSounds[i].name != argsArray[0])
            {
                continue;
            }
            //tries to find the gameobject
            Debug.Log(argsArray[2]);
            GameObject go = GameObject.Find(argsArray[2]);
            if (go == null)
            {
                // if no gameobject or position then 
                if (argsArray[3] == "" || argsArray[4] == "" || argsArray[5] == "")
                {
                    Debug.Log($"GameObject {argsArray[2]} not found and no position set");
                    return;
                }
                else
                {
                    //creates a new gameobject at position [x,y,z]
                    GameObject newObj = new GameObject("SFXObject");
                    newObj.transform.position = new Vector3(float.Parse(argsArray[3]), float.Parse(argsArray[4]),
                        float.Parse(argsArray[5]));
                }
            }
            // if it does not have audio source then add it
            if (go.GetComponent<AudioSource>() == null)
            {
                go.AddComponent<AudioSource>();
            }
            // if loop is true then set it to loop
            if (argsArray[1] == "true")
            {
                go.GetComponent<AudioSource>().loop = true;
            }
            //finally plays the sound
            go.GetComponent<AudioSource>().Stop();
            go.GetComponent<AudioSource>().PlayOneShot(AvaibleSounds[i]);
            
            Debug.Log(go);
        }
    }
}
