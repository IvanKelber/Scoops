using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AudioEvent : ScriptableObject
{

    public string Name;
    public bool loop;

    public abstract void Play(AudioSource source);

    public virtual void PlayWithVolume(AudioSource source, float SFXVolume) {
        source.volume = SFXVolume;
        Play(source);
    }

    public virtual WaitForSeconds PlayAndWait(AudioSource source, float SFXVolume) {
        source.volume = SFXVolume;
        Play(source);
        return new WaitForSeconds(0);
    }

}
