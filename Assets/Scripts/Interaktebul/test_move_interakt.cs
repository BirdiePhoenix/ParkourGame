using UnityEngine;

public class test_move_interakt_start : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager;
    public float PickUpCrystalStart = 0;
    public test_move_interakt_start_end test_move_interakt_start_end;
    public Timer timer;

    public MeshRenderer mesh;

    public float timeMins;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (test_move_interakt_start_end.PickUpCrystalEnd == 1)
        {
            PickUpCrystalStart = 0;
        }
        if(timer.time == 0)
        {
            mesh.enabled = true;
            PickUpCrystalStart = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (test_move_interakt_start_end.PickUpCrystalEnd == 0 && PickUpCrystalStart == 0)
        {
            mesh.enabled = false;
            Timer timer = GameObject.Find("GameManager").GetComponent<Timer>();
            if (timer != null) { timer.StartTimer(timeMins); }
            PickUpCrystalStart = 1;
            musicManager.FadeMusic();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit");
    }

}
