using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    int MinTime;
    int SecTime;

    string MinText;
    string SecText;
    private float time;
    private void Start()
    {
        StartTimer(0.5f);
    }
    void Update()
    {
        if (time != 0)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                text.color = Color.crimson;
                time = 0;
            }
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

            if (seconds >= 60) { seconds = 0; }
            if (milliseconds >= 1000) { milliseconds = 0; }
            text.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public void StartTimer(float minutes)
    {
        time = minutes * 60;
        text.color = Color.cyan;
    }
}
