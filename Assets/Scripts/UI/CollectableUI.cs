using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectableUI : MonoBehaviour
{
    [SerializeField] Image crysImage1;
    [SerializeField] Image crysImage2;
    [SerializeField] Image crysImage3;
    [SerializeField] Image crysImage4;

    public test_move_interakt_start_end checker;

    private void OnEnable()
    {
        if (checker == null) { checker = GameObject.Find("Portal").GetComponent<test_move_interakt_start_end>(); }
    }

    public void ColorCode()
    {
        crysImage1.color = Color.clear;
        crysImage2.color = Color.clear;
        crysImage3.color = Color.clear;
        crysImage4.color = Color.clear;
        if (checker.PartOfTheGame >= 1)
        {
            crysImage1.color = Color.blue;
        }
        if (checker.PartOfTheGame >= 2)
        {
            crysImage2.color = Color.purple;
        }
        if (checker.PartOfTheGame >= 3)
        {
            crysImage3.color = Color.green;
        }
        if (checker.PartOfTheGame >= 4)
        {
            crysImage4.color = Color.red;
        }
    }
}
