using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientHandleScript : MonoBehaviour
{
    [SerializeField] private SoundEnum current;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        SwitchCurrent(current);
    }

    public void SwitchCurrent(SoundEnum track)
    {
        Debug.Log(@$"track = {track}");
        SoundHandleScript.Current.PlaySound(track, source);
    }


}
