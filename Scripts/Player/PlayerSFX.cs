using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private AudioSource SFXAudioSource;
    [SerializeField] private float volume;
    [SerializeField] private float bufferTime;
    [SerializeField] private AudioClip walk_grass;
    [SerializeField] private AudioClip walk_wood;
    [SerializeField] private AudioClip swim;

    public void PlayAudio(string soundEffect)
    {          
        if(!SFXAudioSource.isPlaying)
        {
            if (soundEffect.Equals("Ground"))
                SFXAudioSource.clip = walk_grass;
            if (soundEffect.Equals("Water"))
                SFXAudioSource.clip = swim;
            if (soundEffect.Equals("Object"))
                SFXAudioSource.clip = walk_wood;
            if(soundEffect.Equals("Platform"))
                SFXAudioSource.clip = walk_wood;
            SFXAudioSource.Play();
        }
        else
        {
            if (SFXAudioSource.clip != walk_grass)
            {
                if(soundEffect.Equals("Ground"))
                {
                    SFXAudioSource.clip = walk_grass;
                    SFXAudioSource.Play();
                }
            } 
            else if (SFXAudioSource.clip != swim)
            {
                if (soundEffect.Equals("Water"))
                {
                    SFXAudioSource.clip = swim;
                    SFXAudioSource.Play();
                }
            }
            else if (SFXAudioSource.clip != walk_wood)
            {
                if(soundEffect.Equals("Object"))
                {
                    SFXAudioSource.clip = walk_wood;
                    SFXAudioSource.Play();
                }
            }
            else if (SFXAudioSource.clip != walk_wood)
            {
                if (soundEffect.Equals("Ground"))
                {
                    SFXAudioSource.clip = walk_wood;
                    SFXAudioSource.Play();
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
            SFXAudioSource.PlayScheduled(nextTime);
        }
    }
}
