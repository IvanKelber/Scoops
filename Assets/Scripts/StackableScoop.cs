using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableScoop 
{
    // Reference to Scoop game object
    Scoop scoop;
    ScoopStack stack;
    Color flavor;
    int index;

    // Below/Above/Total for determining matches efficiently
    public BAT bat;

    public int ConsecutiveFlavorScoops {
        get {return bat.Total;}
    }

    public StackableScoop(Scoop scoop, ScoopStack stack, int index) {
        this.scoop = scoop;
        this.flavor = scoop.flavor;
        this.stack = stack;
        this.index = index;
    }

    private BAT CalculateBAT() {
        BAT bat = new BAT();
        if(index != 0 && stack[index - 1].flavor == this.flavor)) {
            bat.Below = stack[index - 1].bat.Below + 1;
            
        }
        return stack.Peek().ConsecutiveFlavorScoops + 1;
    }

    public void MeltScoop() {
        this.scoop.Destroy();
    }




    struct BAT {
        public int Below;
        public int Above;
        public int Total;

        public BAT() {
            this.Below = 0;
            this.Above = 0;
            this.Total = 0;
        }
    }
}
