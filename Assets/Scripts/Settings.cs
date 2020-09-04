using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Settings/MainSettings")]
public class Settings : ScriptableObject
{
    public bool devControls;
    public bool runTutorial;


    [Range(0,1)]
    public float musicVolume;
    [Range(0,1)]
    public float SFXVolume;

}
