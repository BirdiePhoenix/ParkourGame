using System;
using UnityEngine;

public class MusicTrigger : MusicManager
{
    private bool isPlaying = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            faderType = FaderEnum.In;
        }
        else
        {
            isPlaying = false;
            faderType = FaderEnum.Out;
        }

        FadeMusic();
    }
}
