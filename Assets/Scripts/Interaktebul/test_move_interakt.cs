using UnityEngine;

public class test_move_interakt_start : MonoBehaviour
{
    public float PickUpCrystalStart = 0;
    public test_move_interakt_start_end test_move_interakt_start_end;
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (test_move_interakt_start_end.PickUpCrystalEnd == 0 && PickUpCrystalStart == 0)
        {
            Timer timer = GameObject.Find("TimerText").GetComponent<Timer>();
            if (timer != null) { timer.StartTimer(0.1f); }
            PickUpCrystalStart = 1;
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit");
    }
}
