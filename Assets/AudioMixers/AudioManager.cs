using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Audio/Manager/Simple")]
public class AudioManager : ScriptableObject
{
    public AudioEvent DropScoopAudio;
    public AudioEvent GameOverAudio;
    public AudioEvent SwitchScoopsAudio;

    public AudioEvent ScoopsMatchAudio;

    public AudioEvent DestroyScoopAudio;

    public AudioEvent ScoopLandAudio;
    
    public AudioEvent PopScoopAudio;
    public void Play(AudioSource source, AudioEvent audioEvent) {
        audioEvent.Play(source);
    }

    public WaitForSeconds PlayAndWait(AudioSource source, AudioEvent audioEvent) {
        return audioEvent.PlayAndWait(source);
    }
    
}
