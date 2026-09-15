using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject crystalOne;
    public GameObject crystalTwo;
    public GameObject crystalThree;
    public GameObject crystalFour;

    public GameObject portal;

    public bool crystalOneActive;
    public bool crystalTwoActive;
    public bool crystalThreeActive;
    public bool crystalFourActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portal.SetActive(false);
        crystalOne.SetActive(false); crystalTwo.SetActive(false); crystalThree.SetActive(false); crystalFour.SetActive(false);
        crystalOneActive = false; crystalTwoActive = false; crystalThreeActive = false; crystalFourActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (crystalOneActive)
        {
            crystalOne.SetActive (true);
        }
        if (crystalTwoActive)
        {
            crystalTwo.SetActive (true);
        }
        if (crystalThreeActive)
        {
            crystalThree.SetActive (true);
        }
        if (crystalFourActive)
        {
            crystalFour.SetActive (true);
        }

        if(crystalOneActive && crystalTwo && crystalThree && crystalFour)
        {
            portal.SetActive(true);
        }
    }
}
