using System;
using UnityEngine;

public class MusicTrigger : MusicManager
{
    private void OnTriggerEnter(Collider other)
    {
        FadeMusic();
    }
}
