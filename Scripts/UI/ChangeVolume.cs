using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ChangeVolume : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private string playerPrefabPath;
    [SerializeField] private string levelMusicPrefabPath;

    // Start is called before the first frame update
    void Start()
    {
        musicVolumeSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        SFXVolumeSlider.onValueChanged.AddListener(delegate { ChangeSFXVolume(); });
    }

    private void ChangeSFXVolume()
    {
        GameObject playerPrefab = PrefabUtility.LoadPrefabContents(playerPrefabPath);
        playerPrefab.GetComponent<AudioSource>().volume = SFXVolumeSlider.value * masterVolumeSlider.value;
    }
    private void ChangeMusicVolume()
    {
        GameObject levelMusicPrefab = PrefabUtility.LoadPrefabContents(levelMusicPrefabPath);
        levelMusicPrefab.GetComponent<AudioSource>().volume = musicVolumeSlider.value * masterVolumeSlider.value;
    }
}
