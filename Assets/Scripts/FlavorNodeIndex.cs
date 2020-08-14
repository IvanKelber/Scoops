using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorNodeIndex
{

    //Reference to flavorNode
    FlavorNode flavorNode;

    //Reference to index within that flavorNode 
    int index;

    public FlavorNodeIndex(FlavorNode flavorNode, int index) {
        this.flavorNode = flavorNode;
        this.index = index;
    }

}