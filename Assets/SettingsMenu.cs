using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{

    [SerializeField]
    private Settings settings;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider SFXVolumeSlider;

    [SerializeField]
    private Toggle runTutorialToggle;

    [SerializeField]
    private Toggle devControlsToggle;
    
    [SerializeField]
    private AudioEvent exampleSFXAudio;
    private AudioSource audioSource;
    private bool hidden = true;

    private void Start() {
        musicVolumeSlider.value = settings.musicVolume;
        SFXVolumeSlider.value = settings.SFXVolume;
        runTutorialToggle.isOn = settings.runTutorial;
        devControlsToggle.isOn = settings.devControls;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ToggleMenu() {
        if(!hidden) {
            hidden = true;
            GetComponent<CanvasGroup>().alpha = 0;
        }
        else {
            hidden = false;
            GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public void OnMusicSliderChanged(float value) {
        settings.musicVolume = value;
    }
    public void OnSFXSliderChanged(float value) {
        settings.SFXVolume = value;
        StopCoroutine("TestSFXAudio");
        StartCoroutine("TestSFXAudio");

    }

    public void OnRunTutorialClicked(bool value) {
        settings.runTutorial = value;
    }

    public void OnDevControlsClicked(bool value) {
        settings.devControls = value;
    }

    private IEnumerator TestSFXAudio() {
        yield return new WaitForSeconds(.1f);
        if(!hidden) {
            exampleSFXAudio.PlayWithVolume(audioSource, settings.SFXVolume);
        }
    }

}
