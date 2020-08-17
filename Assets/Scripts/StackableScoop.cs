using System.Collections;
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
    }

    //Should be called before scoop is part of stack
    private int DetermineConsecutiveFlavorScoops() {
        if(stack.Count == 0 || stack.Peek().flavor != this.flavor) {
            return 1; // Only one of this flavor consecutively
        }
        return stack.Peek().ConsecutiveFlavorScoops + 1;
    }

    public int CalculateConsecutiveFlavors(Stack<StackableScoop> stack) {
        this.stack = stack;
        this.ConsecutiveFlavorScoops = DetermineConsecutiveFlavorScoops();
        return ConsecutiveFlavorScoops;
    }

    public void MoveToIndex(Vector2Int index) {
        scoop.MoveToIndex(index);
    }

    public void RemoveInputHandlers() {
        scoop.RemoveInputHandlers();
    }

    public void MeltScoop(AudioSource source, AudioEvent meltEvent) {
        scoop.MeltScoop(source, meltEvent);
    }
    

}
