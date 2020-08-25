using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AudioEvent : ScriptableObject
{

    public string Name;
    public bool loop;

    public abstract void Play(AudioSource source);

    public virtual WaitForSeconds PlayAndWait(AudioSource source) {
        Play(source);
        return new WaitForSeconds(0);
    }

}
