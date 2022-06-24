using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    PlayerController player;

    public AudioClip walk_grass;
    public AudioClip walk_wood;
    public AudioClip swim;

    public AudioSource audio;

    public float volume;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // On grass
        if (player.Grounded && 
            Mathf.Abs(player.RawMovement.x) > 0 && 
            !audio.isPlaying)
        {
            audio.PlayOneShot(walk_grass, volume);
        }

        // On wood
        if (true)
        {
            audio.PlayOneShot(walk_wood, volume);
        }

        // In water
        if (true)
        {
            audio.PlayOneShot(swim, volume);
        }
    }
}
