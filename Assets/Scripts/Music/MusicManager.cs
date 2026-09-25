using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource mainBeat;
    [SerializeField] AudioSource crystalMusic;
    [SerializeField] AudioSource loopMusic;
    [SerializeField] AudioSource transition;
    [SerializeField] protected FaderEnum faderType;
    [SerializeField] private float faderSpeed = 0.5f;
    [SerializeField] private float maxVolume = 0.75f;
    protected bool hasCrystal = false;
    private bool hasLooped = false;

    private void Awake()
    {
        bgm.volume = maxVolume;
        loopMusic.volume = 0;
        mainBeat.volume = maxVolume;
        crystalMusic.volume = 0;
        transition.volume = maxVolume;
    }

    private void FixedUpdate()
    {
        if(bgm.time >= 10 && !hasLooped)
        {
            loopMusic.volume = maxVolume;
            hasLooped = true;
        }
    }

    public enum FaderEnum
    {
        In,
        Out
    }

    public void FadeMusic()
    {
        transition.Play();

        if (!hasCrystal)
        {
            faderType = FaderEnum.In;
        }
        else
        {
            faderType = FaderEnum.Out;
        }

        switch (faderType)
        {
            case FaderEnum.In:
                StartCoroutine(FadeIn());
                break;
            case FaderEnum.Out:
                StartCoroutine(FadeOut());
                break;
        }
    }

    private IEnumerator FadeIn()
    {
        if (!hasCrystal)
        {
            hasCrystal = true;
        }
        yield return new WaitForSeconds(faderSpeed);
        
        crystalMusic.volume += 0.1f;
        mainBeat.volume -= 0.1f;

        if(crystalMusic.volume < maxVolume || mainBeat.volume > 0)
        {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut()
    {
        if (hasCrystal)
        {
            hasCrystal = false;
        }
        yield return new WaitForSeconds(faderSpeed);
        crystalMusic.volume -= 0.1f;
        mainBeat.volume += 0.1f;

        if (crystalMusic.volume > 0 || mainBeat.volume < maxVolume)
        {
            StartCoroutine(FadeOut());
        }
    }
}
