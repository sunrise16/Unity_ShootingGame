using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    
    public AudioClip[] bgmClip;
    public static SoundManager instance;

    void Awake()
    {
        if (SoundManager.instance == null)
        {
            SoundManager.instance = this;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
	}
	
    // BGM 재생
    public void PlayBGM(int number)
    {
        audioSource.clip = bgmClip[number - 1];
        audioSource.Play();
    }

    // BGM 정지
    public void StopBGM()
    {
        audioSource.Stop();
    }

    // 효과음 재생
    public void PlaySE(int number)
    {
        AudioSource audioSource = transform.GetChild(number - 1).gameObject.GetComponent<AudioSource>();
        if (audioSource.isPlaying.Equals(true))
        {
            audioSource.Stop();
        }
        audioSource.Play();
    }
}
