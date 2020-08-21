using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Flavor {
    public Color color;
    public string description;

    public Flavor(Color color, string description) {
        this.color = color;
        this.description = description;
    }
}


