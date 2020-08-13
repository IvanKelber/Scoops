﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableScoop 
{
    Scoop scoop;
    Stack<StackableScoop> stack;
    Color flavor;

    public int ConsecutiveFlavorScoops;

    public StackableScoop(Scoop scoop, Stack<StackableScoop> stack) {
        this.scoop = scoop;
        this.flavor = scoop.flavor;
        this.stack = stack;
        this.ConsecutiveFlavorScoops = DetermineConsecutiveFlavorScoops();
        stack.Push(this);
    }

    private int DetermineConsecutiveFlavorScoops() {
        if(stack.Count == 0 || stack.Peek().flavor != this.flavor) {
            return 1; // Only one of this flavor consecutively
        }
        return stack.Peek().ConsecutiveFlavorScoops + 1;
    }

    public void MeltScoop() {
        this.scoop.Destroy();
    }
}
