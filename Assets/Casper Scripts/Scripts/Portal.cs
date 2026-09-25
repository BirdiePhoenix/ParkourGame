using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject spike;
    public Gamemanager gamemanager;

    public GameObject crystalOne;
    public GameObject crystalTwo;
    public GameObject crystalThree;
    public GameObject crystalFour;

    public GameObject portal;

    public bool crystalOneActive;
    public bool crystalTwoActive;
    public bool crystalThreeActive;
    public bool crystalFourActive;

    public List<GameObject> newMap2 = new List<GameObject>();
    public List<GameObject> newMap3 = new List<GameObject>();
    public List<GameObject> newMap4 = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portal.SetActive(false);
        crystalOne.SetActive(false); crystalTwo.SetActive(false); crystalThree.SetActive(false); crystalFour.SetActive(false);
        crystalOneActive = false; crystalTwoActive = false; crystalThreeActive = false; crystalFourActive = false;

        newMap4.ForEach(x => x.SetActive(false));
        newMap3.ForEach(x => x.SetActive(false));
        newMap2.ForEach(x => x.SetActive(false));

        gamemanager = GameObject.Find("GameManager").GetComponent<Gamemanager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (crystalOneActive)
        {
            crystalOne.SetActive (true);
            spike.SetActive (false);
            newMap2.ForEach(x => x.SetActive(true));
            CheckpointManager.Instance.SetCheckpoint(2);
        }
        else
        {
            crystalOne.SetActive (false);
        }
        if (crystalTwoActive)
        {
            crystalTwo.SetActive(true);
            newMap3.ForEach(x => x.SetActive(true));
            CheckpointManager.Instance.SetCheckpoint(4);
        }
        else
        {
            crystalTwo.SetActive(false);
        }
        if (crystalThreeActive)
        {
            crystalThree.SetActive (true);
            newMap4.ForEach(x => x.SetActive(true));
            CheckpointManager.Instance.SetCheckpoint(6);
        }
        else
        {
            crystalThree.SetActive(false);
        }
        if (crystalFourActive)
        {
            crystalFour.SetActive (true);
            CheckpointManager.Instance.SetCheckpoint(8);
        }
        else
        {
            crystalFour.SetActive(false);
        }

        if (crystalOneActive && crystalTwoActive && crystalThreeActive && crystalFourActive)
        {
            portal.SetActive(true);
        }
        else
        {
            portal.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (crystalOneActive && crystalTwoActive && crystalThreeActive && crystalFourActive)
        {
            string timetext = gamemanager.GetTime();
            FinalScreen finalScreen = GameObject.Find("FinalScreenActual").GetComponent<FinalScreen>();
            finalScreen.Awaken(timetext);
        }
    }
}
