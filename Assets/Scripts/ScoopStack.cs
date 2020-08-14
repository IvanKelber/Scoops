using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoopStack 
{

    private FlavorNode top;
    private FlavorNode bottom;

    public int Count;

    public event Action<List<Scoop>> ScoopMatchFound = delegate {};

    public ScoopStack() {
        this.Count = 0;
        this.bottom = null;
        this.top = null;
    }

    public ScoopStack(FlavorNode node) {
        // if(node == null) {
        //     ScoopStack();
        // }

        FlavorNode cur = node;
        while(cur.above != null) {
            this.Count += cur.Count;
            cur = cur.above;
        }
        this.top = cur;
        this.bottom = node;

        this.top.above = this.bottom;
        this.bottom.below = this.top;
    }

    public void Push(Scoop scoop) {
        FlavorNode node = new FlavorNode(scoop);
        Push(node);
    }

    // For adding a single node only.
    // For adding multiple nodes use MergeScoopStack
    public void Push(FlavorNode node) {
        this.Count += node.scoops.Count;
        if(top == null) {
            bottom = node;
            top = bottom;

            bottom.above = top;
            bottom.below = top;
            
        } else if(top.flavor == node.flavor) {
            // Merge nodes
            top.scoops.AddRange(node.scoops);
            if(top.scoops.Count >= 3) {
                Count -= top.Count;

                ScoopMatchFound(top.scoops);
                if(top == bottom) {
                    top = null;
                    bottom = null;
                } else {
                    top.below.above = bottom;
                    bottom.below = top.below;

                    top = top.below;
                }
            }
        } else {
            // Node has a different flavor
            node.above = bottom;
            bottom.below = node;

            top.above = node;
            node.below = top;

            top = node;
        }

    }

    public string StackInfo_Debug() {
        // Debug.Log("ScoopStack.Count: " + Count);
        // Debug.Log("Top flavor: " + top.flavor);
        // Debug.Log("Bottom flavor: " + bottom.flavor);
        if(Count == 0) {
            return "Stack empty";
        }
        FlavorNode cur = top;
        string flavorNodes = "[";
        while(true) {
            flavorNodes += "(" + cur.flavor +", " + cur.Count + "), ";
            if(cur == bottom) {
                break;
            }
            cur = cur.below;
        }
       return "ScoopStack flavor nodes: " + flavorNodes + "]";
    }

    // public void Merge(ScoopStack stack2) {
    //     if(stack2.bottom.flavor == this.top.flavor) {
    //         // Merge nodes
    //         this.top.scoops.AddRange(stack2.bottom.scoops);
    //     } else {

    //     }
    //     stack2.bottom.below = this.top;
    //     this.top.above = stack2.bottom;
    //     stack2.top.above = this.bottom;
    //     this.bottom.below = stack2.top;

    //     this.top = stack2.top;

    //     this.Count += stack2.Count;
    // }

}
