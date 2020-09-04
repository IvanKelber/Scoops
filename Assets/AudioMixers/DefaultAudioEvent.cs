using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="Audio Event/Default")]
public class DefaultAudioEvent : AudioEvent
{
    public AudioClip[] clips;

    public override void Play(AudioSource source) {
        if(clips.Length == 0) {
            return;
        }

        source.clip = clips[Random.Range(0, clips.Length)];
        source.Play();
    }
}
