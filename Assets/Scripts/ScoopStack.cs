using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopStack 
{

    ArrayList<StackableScoop> scoops = new ArrayList<StackableScoop>();

    public ScoopStack() {

    }

    public void Push(StackableScoop scoop) {
        scoops.Add(scoop);
    }

    public bool IsEmpty() {
        return scoops.Count == 0;
    }

    public StackableScoop Peek() {
        // Throws exception if scoops is empty
        return scoops[scoops.Count - 1];
    }

    public StackableScoop Pop() {
        // Throws exception if scoops is empty
        StackableScoop scoop = scoops[scoops.Count - 1];
        scoops.RemoveAt(scoops.Count - 1);
        return scoop;
    }

    public void ReverseRange(int index) {
        // Reverses everything above a valid index
        if(index >= scoops.Count || index < 0) {
            return;
        }
        for(int i = index; i < (scoops.Count - i)/2; i++) {
            int j = scoops.Count - i - 1;
            StackableScoop temp = scoops[j];
            scoops[j] = scoops[i];
            scoops[i] = temp;
        }
    }

    public int Count {
        get {return scoops.Count;}
    }

}
