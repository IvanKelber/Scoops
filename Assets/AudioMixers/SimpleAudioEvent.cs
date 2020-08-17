using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="Audio Event/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip[] clips;


    [Range(0,1)]
    public float minVolume;

    [Range(0,1)]
    public float maxVolume;

    [Range(0,4)]
    public float minPitch;

    [Range(0,4)]
    public float maxPitch;

    public override void Play(AudioSource source) {
        if(clips.Length == 0) {
            return;
        }

        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = Random.Range(minVolume, maxVolume);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();

    }
}
