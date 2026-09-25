using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SoundHandlerUI : MonoBehaviour
{
    private AudioClip[] _obtainableSound; 
    private static SoundHandlerUI instance;
    private InputAction pauseActionUI;
    private GameObject _player;
    private GameObject _camera;
    private bool paused = false;

    
    private void Awake()
    {
        pauseActionUI = InputSystem.actions.FindAction("UI/Pause");
        //keeps object between scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        //loads all sfx into the array
        _obtainableSound = Resources.LoadAll<AudioClip>("SFX/UI");
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
        _player = GameObject.Find("Player");
        _camera  = GameObject.Find("Main Camera");
        
        if (_camera != null)
            gameObject.transform.position = _camera.transform.position;
        
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
                case "ResumeButton":
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

    private void Update()
    {
        if (_player  == null)
            
            return;
        gameObject.transform.position = _player.transform.position;
        if (_player.GetComponent<PlayerMovement>().paused &&  !paused)
        {
            paused = true;
            ClickedButton("Pause");
        }

        if (!_player.GetComponent<PlayerMovement>().paused)
        {
            paused = false;
        }
    }

    private AudioClip FindClipInArray(string clipName)
    {
        return Array.Find(_obtainableSound, clip => clip.name == clipName);
    }
    private GameObject FindCamera()
    {
        return GameObject.Find("mainCamera");
    }
    private void ClickedButton(string clipName)
    {
        //fins clip and camera
        AudioClip clip = null;
        foreach (AudioClip sfx in _obtainableSound)
        {
            if (sfx.name == clipName)
            {
                clip = sfx;
            }
        }
        GameObject camera = FindCamera();
        if (camera == null)
        {
            camera = _camera;
        }
        if (clip == null || camera == null)
            return;
        //if camera dosent have an audiosource then add it
        AudioSource source = gameObject.GetComponent<AudioSource>();
        if (source == null)
            source = camera.AddComponent<AudioSource>();

        //sets and plays the clip
        source.clip = clip;
        source.PlayOneShot(clip);
    }
}
