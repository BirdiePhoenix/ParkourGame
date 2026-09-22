using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource crystalMusic;
    [SerializeField] protected FaderEnum faderType;
    [SerializeField] private float faderSpeed = 0.5f;
    protected bool isFading = false;

    public enum FaderEnum
    {
        In,
        Out
    }

    public void SetFaderType(FaderEnum _faderType)
    {
        faderType = _faderType;
    }

    public void FadeMusic()
    {
        
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
