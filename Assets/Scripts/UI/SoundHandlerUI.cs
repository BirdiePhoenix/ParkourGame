using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SoundHandlerUI : MonoBehaviour
{
    private AudioClip[] _obtainableSound; 
    private static SoundHandlerUI instance;
    
    private void Awake()
    {
        //keeps object between scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        //loads all sfx into the array
        _obtainableSound = Resources.LoadAll<AudioClip>("SFX");
    }
    
    //if scene switch then
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded();
    }

    private void OnSceneLoaded(Scene scene = new Scene(), LoadSceneMode mode = new LoadSceneMode())
    {
        //stores all new buttons in an array
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include);
        //plays diffrent sound depending on button
        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "StartButton":
                {
                    button.onClick.AddListener(() => ClickedButton("Generic_click"));
                    break;
                }
                case "PauseButton":
                {
                    button.onClick.AddListener(() => ClickedButton("Pause"));
                    break;
                }
                case "OptionsButton":
                {
                    button.onClick.AddListener(() => ClickedButton("Generic_click"));
                    break;
                }
                case "QuitButton":
                {
                    button.onClick.AddListener(() => ClickedButton("Pause"));
                    break;
                }
                default:
                {
                    button.onClick.AddListener(() => ClickedButton("Generic_click"));
                    break;
                }
            }
        }
    }

    private AudioClip FindClipInArray(string clipName)
    {
        return Array.Find(_obtainableSound, clip => clip.name == clipName);
    }
    private GameObject FindCamera()
    {
        return GameObject.Find("Main Camera");
    }
    private void ClickedButton(string clipName)
    {
        //fins clip and camera
        AudioClip clip = FindClipInArray(clipName);
        GameObject camera = FindCamera();
        if (clip == null || camera == null)
            return;
        //if camera dosent have an audiosource then add it
        AudioSource source = camera.GetComponent<AudioSource>();
        if (source == null)
            source = camera.AddComponent<AudioSource>();

        //sets and plays the clip
        source.clip = clip;
        source.PlayOneShot(clip);
    }
}
