using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource mainBeat;
    [SerializeField] AudioSource crystalMusic;
    [SerializeField] AudioSource loopMusic;
    [SerializeField] AudioSource bassOneShot;
    [SerializeField] AudioSource crystalOneShot;
    [SerializeField] protected FaderEnum faderType;
    [SerializeField] private float faderSpeed = 0.5f;
    [SerializeField] private float maxVolume = 0.75f;
    protected bool isFading = false;
    private bool isPlaying = false;
    private bool hasLooped = false;

    private void Awake()
    {
        bgm.volume = maxVolume;
        loopMusic.volume = 0;
        mainBeat.volume = maxVolume;
        crystalMusic.volume = 0;
        bassOneShot.volume = maxVolume;
        crystalOneShot.volume = maxVolume;
    }

    //public void StartCrystalMusic()
    //{
    //    if(crystalMusic.volume == 0)
    //    {
    //        //mainBeat.volume = 0;
    //        crystalMusic.volume = maxVolume;
    //    }
    //    else
    //    {
    //       // mainBeat.volume = maxVolume;
    //        crystalMusic.volume = 0;
    //    }
    //}

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
        //StartCrystalMusic();
        if (!isPlaying)
        {
            isPlaying = true;
            bassOneShot.Play();
            faderType = FaderEnum.In;
        }
        else
        {
            isPlaying = false;
            crystalOneShot.Play();
            faderType = FaderEnum.Out;
        }

        switch (faderType)
        {
            case FaderEnum.In:
                if (!isFading)
                {
                    StartCoroutine(FadeIn());
                }
                break;
            case FaderEnum.Out:
                if (!isFading)
                {
                    StartCoroutine(FadeOut());
                }
                break;
        }
    }

    private IEnumerator FadeIn()
    {
        if (!isFading)
        {
            isFading = true;
        }
        yield return new WaitForSeconds(faderSpeed);
        
        crystalMusic.volume += 0.1f;
        mainBeat.volume -= 0.1f;

        if(crystalMusic.volume < maxVolume || mainBeat.volume > 0)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            isFading = false;
        }
    }

    private IEnumerator FadeOut()
    {
        if (!isFading)
        {
            isFading = true;
        }
        yield return new WaitForSeconds(faderSpeed);
        crystalMusic.volume -= 0.1f;
        mainBeat.volume += 0.1f;

        if (crystalMusic.volume > 0 || mainBeat.volume < maxVolume)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            isFading = false;
        }
    }
}
