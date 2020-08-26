using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Flavor {
    public Color color;
    public string name;

    public Flavor(Color color, string name) {
        this.color = color;
        this.name = name;
    }

    public static bool operator ==(Flavor a, Flavor b) {
        return a.color == b.color;
    }

    public static bool operator !=(Flavor a, Flavor b) {
        return a.color != b.color;
    }

}


