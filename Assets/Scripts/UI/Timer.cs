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
    public float time;
    void Update()
    {
        if (time != 0)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                test_move_interakt_start start = GameObject.Find("crystal").GetComponent<test_move_interakt_start>();   
                TimeOut(start);
            }
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

            if (seconds >= 60) { seconds = 0; }
            if (milliseconds >= 1000) { milliseconds = 0; }
            text.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public float GetTime() { return time; }

    public void TimeOut(test_move_interakt_start start)
    {
        start.PickUpCrystalStart = 0;
        text.color = Color.crimson;
        time = 0;
    }

    public void StartTimer(float minutes)
    {
        time = minutes * 60;
        text.color = Color.cyan;
    }

    public void StopTimer()
    {
        text.text = "";
        time = 0;
    }
}
