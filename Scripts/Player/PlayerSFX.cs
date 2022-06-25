using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private float volume;
    [SerializeField] private float bufferTime;
    [SerializeField] private AudioClip walk_grass;
    [SerializeField] private AudioClip walk_wood;
    [SerializeField] private AudioClip swim;
    [SerializeField] private AudioClip swap;

    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayAudio(string soundEffect)
    {          
        if(!audioSource.isPlaying)
        {
            if (soundEffect.Equals("Ground"))
                audioSource.clip = walk_grass;
            if (soundEffect.Equals("Water"))
                audioSource.clip = swim;
            if (soundEffect.Equals("Object"))
                audioSource.clip = walk_wood;
            if(soundEffect.Equals("Platform"))
                audioSource.clip = walk_wood;
            audioSource.Play();
        }
        else
        {
            if (audioSource.clip != walk_grass)
            {
                if(soundEffect.Equals("Ground"))
                {
                    audioSource.clip = walk_grass;
                    audioSource.Play();
                }
            } 
            else if (audioSource.clip != swim)
            {
                if (soundEffect.Equals("Water"))
                {
                    audioSource.clip = swim;
                    audioSource.Play();
                }
            }
            else if (audioSource.clip != walk_wood)
            {
                if(soundEffect.Equals("Object"))
                {
                    audioSource.clip = walk_wood;
                    audioSource.Play();
                }
            }
            else if (audioSource.clip != walk_wood)
            {
                if (soundEffect.Equals("Ground"))
                {
                    audioSource.clip = walk_wood;
                    audioSource.Play();
                }
            }
            else
            {
                PlayBuffered();
            }
        }
    }

    private double nextTime = 0;
    private void PlayBuffered()
    {
        double time = AudioSettings.dspTime;

        if(time > nextTime)
        {
            nextTime += bufferTime;
            audioSource.PlayScheduled(nextTime);
        }
    }

    public void PlaySwapEffect()
    {
        audioSource.clip = swap;
        audioSource.Play();
    }
}
