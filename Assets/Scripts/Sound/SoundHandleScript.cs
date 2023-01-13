using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandleScript : MonoBehaviour
{
    public static SoundHandleScript Current;

    [SerializeField] private List<AudioClip> sounds;
    [SerializeField] private List<SoundEnum> sourceNames;
    private Dictionary<SoundEnum, AudioClip> audioList;

    private void Awake()
    {
        InitDictionary();
        if (Current != null)
            Debug.LogError("Создано больше 1 звукового сервиса!");

        Current = this;
    }
    private void InitDictionary()
    {
        audioList = new Dictionary<SoundEnum, AudioClip>();
        int i = 0;
        foreach (AudioClip sound in sounds)
        {
            audioList.Add(sourceNames[i], sound);
            i++;
        }
    }

    public void PlaySound(SoundEnum sound, AudioSource source)
    {
        source.clip = audioList[sound];
        source.Play();
    }


}
