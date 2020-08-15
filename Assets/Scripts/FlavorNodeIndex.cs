using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorNodeIndex
{

    //Reference to flavorNode
    public FlavorNode flavorNode;

    //Reference to index within that flavorNode 
    public int index;

    public FlavorNodeIndex(FlavorNode flavorNode, int index) {
        this.flavorNode = flavorNode;
        this.index = index;
    }

}