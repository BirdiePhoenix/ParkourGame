using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    //[SerializeField] private PlayerMovement movementScript;
    // maybe add events from movement to trigger player events?
    private AudioClip[] AvaibleSounds;
    private List<GameObject> _audioInScene;
    private string _pathToSFX = System.IO.Path.Combine(Application.dataPath, "SFX");
    private void Awake()
    {
        Debug.Log(_pathToSFX);
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

    private void playJumpSFX()
    {
        
    }
    
    public void StopSFX(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (!_audioInScene.Contains(obj))
        {
            return;
        }
        obj.GetComponent<AudioSource>().Stop();
        _audioInScene.Remove(obj);
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
            //finally adds it to the clips in scene list
            _audioInScene.Add(go);
            go.GetComponent<AudioSource>().PlayOneShot(AvaibleSounds[i]);
        }
    }
}
