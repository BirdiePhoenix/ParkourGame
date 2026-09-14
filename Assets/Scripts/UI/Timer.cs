using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    int MinTime;
    int SecTime;

    string MinText;
    string SecText;

    private void Update()
    {
        MinTime = 0 - ((int)Time.time / 60);
        SecTime = 60 - ((int)Time.time % 60);

        if (SecTime >= 60) { SecTime = 0; MinTime += 1; }
        if (SecTime <= 0) { SecTime = 0; }
        if (MinTime <= 0) { MinTime = 0; }

        MinText = MinTime.ToString();
        SecText = SecTime.ToString();

        if (MinText.Length <= 1) { MinText = $"0{MinText}"; };
        if (SecText.Length <= 1) { SecText = $"0{SecText}"; };

        text.text = $"{MinText}:{SecText}";

        if (text.text == "00:00") { Debug.Log("Times Up!"); }
    }

    public void StartTimer()
    {
        
    }
}
