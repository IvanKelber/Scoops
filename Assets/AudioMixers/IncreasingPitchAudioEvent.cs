using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="Audio Event/Increasing Pitch")]
public class IncreasingPitchAudioEvent : AudioEvent
{
    public AudioClip[] clips;


    [Range(0,1)]
    public float minVolume;

    [Range(0,1)]
    public float maxVolume;

    [Range(0,4)]
    public float startingPitch;

    private float pitch;

    public override void Play(AudioSource source) {
        if(clips.Length == 0) {
            return;
        }
        if(pitch == 0 || pitch > startingPitch + 1) {
            pitch = startingPitch;
        }

        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = Random.Range(minVolume, maxVolume);
        source.pitch = pitch;
        pitch += .1f;
        source.Play();

    }
}
