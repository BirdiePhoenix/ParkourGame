using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource crystalMusic;
    [SerializeField] AudioSource crystalMusic2;
    [SerializeField] protected FaderEnum faderType;
    [SerializeField] private float faderSpeed = 0.5f;
    protected bool isFading = false;
    private bool isPlaying = false;
    private bool hasCrystal = false;

    public enum FaderEnum
    {
        In,
        Out
    }

    public void FadeMusic()
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

        switch (faderType)
        {
            case FaderEnum.In:
                if(!isFading)
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
        if (!hasCrystal)
        {
            crystalMusic2.volume += 0.1f;
            hasCrystal = true;
        }

        if(crystalMusic.volume < 1)
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

        if (crystalMusic.volume > 0)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            isFading = false;
        }
    }
}
