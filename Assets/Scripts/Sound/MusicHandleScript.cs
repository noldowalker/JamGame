using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandleScript : MonoBehaviour
{
    public static MusicHandleScript Current;
    [SerializeField] private MusicEnum currentMusic;
    [SerializeField] public MusicEnum defaultSceneMusic;
    [SerializeField] private List<AudioClip> music;
    [SerializeField] private List<MusicEnum> trackNames;
    private Dictionary<MusicEnum, AudioClip> musicList;
    private AudioSource musicSource;

    private void Awake()
    {
        InitDictionary();
        musicSource = GetComponent<AudioSource>();
        if (Current != null)
            Debug.LogError("Создано больше 1 музыкального сервиса!");

        Current = this;
        SwitchCurrentMusic(currentMusic);
    }
    private void InitDictionary()
    {
        musicList = new Dictionary<MusicEnum, AudioClip>();
        int i = 0;
        foreach (AudioClip sound in music)
        {
            musicList.Add(trackNames[i], sound);
            i++;
        }
    }

    public void SwitchCurrentMusic(MusicEnum track)
    {
        currentMusic = track;
        musicSource.clip = musicList[currentMusic];
        musicSource.Play();
    }

    public void SwitchToDefault()
    {
        currentMusic = defaultSceneMusic;
        musicSource.clip = musicList[currentMusic];
        musicSource.Play();
    }


}
