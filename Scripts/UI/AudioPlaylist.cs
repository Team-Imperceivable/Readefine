using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlaylist : MonoBehaviour
{
    [SerializeField] private List<AudioClip> songs;
    private AudioSource audioSource;
    int index;
    bool finishedLoad;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        ShuffleSongs();
        index = 0;
        audioSource.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if(!finishedLoad)
        {
            finishedLoad = true;
            audioSource.UnPause();
        }
        if(index >= songs.Count - 1)
        {
            ShuffleSongs();
            index = 0;
        }
        else if(!audioSource.isPlaying)
        {
            audioSource.clip = songs[index];
            audioSource.Play();
            index++;
        }
    }

    private void ShuffleSongs()
    {
        List<AudioClip> newSongs = new List<AudioClip>();
        AudioClip currSong = songs[index];
        while(songs.Count != 0)
        {
            AudioClip song = songs[Random.Range(0, songs.Count - 1)];
            if(newSongs.Count != 0 || song != currSong)
            {
                newSongs.Add(song);
                songs.Remove(song);
            }
        }
        songs = newSongs;
    }
}
