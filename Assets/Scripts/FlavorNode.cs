using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorNode
{

    public FlavorNode below;
    public FlavorNode above;
    public Color flavor;

    public int Count {
        get {return scoops.Count;}
    }

    public List<Scoop> scoops = new List<Scoop>();

    public FlavorNode(Scoop scoop) {
        below = null;
        above = null;
        flavor = scoop.flavor;
        scoops.Add(scoop);
    }



}
