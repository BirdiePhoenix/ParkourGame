using System.Collections.Generic;
using UnityEngine;

public class test_move_interakt_start_end : MonoBehaviour
{
    [SerializeField] MusicManager musicManager;
    public float PickUpCrystalEnd = 0;
    public float PartOfTheGame = 0;
    public int variable = 0;
    //public test_move_interakt_start test_move_interakt_start;

    public List<test_move_interakt_start> test_move_interakt_start_;

    public List<GameObject> crystals = new List<GameObject>();

    public Portal portalScript_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PartOfTheGame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (test_move_interakt_start_[variable].PickUpCrystalStart == 0)
        {
            PickUpCrystalEnd = 0;
        }

        if (PartOfTheGame == 1)
        {
            portalScript_.crystalOneActive = true;
            Destroy(crystals[0]);
        }
        if (PartOfTheGame == 2)
        {
            portalScript_.crystalTwoActive = true;
            Destroy(crystals[1]);
        }
        if (PartOfTheGame == 3)
        {
            portalScript_.crystalThreeActive = true;
            Destroy(crystals[2]);
        }
        if (PartOfTheGame == 4)
        {
            portalScript_.crystalFourActive = true;
            Destroy(crystals[3]);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (test_move_interakt_start_[variable].PickUpCrystalStart == 1)
        {
            Timer timer = GameObject.Find("GameManager").GetComponent<Timer>();
            if (PartOfTheGame <= 3 && timer.GetTime() != 0)
            {
                if (PartOfTheGame == 3)
                {
                    if (timer != null) { timer.StopTimer(); }
                    PartOfTheGame += 1;
                    PickUpCrystalEnd = 2;
                    Debug.Log("good game");
                    musicManager.FadeMusic();
                }
                else
                {
                    if (timer != null) { timer.StopTimer(); }
                    PartOfTheGame += 1;
                    PickUpCrystalEnd = 1;
                    variable += 1;
                    Debug.Log("good job");
                    musicManager.FadeMusic();
                }
            }
        }  
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit");
    }
}
