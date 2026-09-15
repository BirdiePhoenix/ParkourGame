using UnityEngine;

public class test_move_interakt_start_end : MonoBehaviour
{
    public float PickUpCrystalEnd = 0;
    public float PartOfTheGame = 0;
    public test_move_interakt_start test_move_interakt_start;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PartOfTheGame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (test_move_interakt_start.PickUpCrystalStart == 0)
        {
            PickUpCrystalEnd = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (test_move_interakt_start.PickUpCrystalStart == 1)
        {
            if (PartOfTheGame <= 3)
            {
                if (PartOfTheGame == 3)
                {
                    PartOfTheGame += 1;
                    PickUpCrystalEnd = 2;
                    Debug.Log("good game");
                }
                else
                {
                    PartOfTheGame += 1;
                    PickUpCrystalEnd = 1;
                    Debug.Log("good job");
                }
            }
        }  
    }
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit");
    }
}
