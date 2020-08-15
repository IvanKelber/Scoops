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
        Scoop.ScoopTapped += HandleScoopTap;
        this.Count = 0;
        this.bottom = null;
        this.top = null;
    }

    public void Push(Scoop scoop) {
        FlavorNode node = new FlavorNode(scoop);
        scoop.flavorNodeIndex = new FlavorNodeIndex(node, 0);
        Push(node);
    }

    // For adding a single node to the top of the stack
    public void Push(FlavorNode node) {
        this.Count += node.scoops.Count;
        if(top == null) {
            bottom = node;
            top = bottom;

            bottom.above = top;
            bottom.below = top;
            
        } else {
            // Node has a different flavor
            node.above = bottom;
            bottom.below = node;

            top.above = node;
            node.below = top;

            top = node;
        }

        if(node != node.below && node.below.flavor == node.flavor) {
            MergeFlavorNodes(node.below, node);
        }

    }

    public void HandleScoopTap(FlavorNodeIndex flavorNodeIndex) {
        // FlavorNode node = flavorNodeIndex.flavorNode;
        // int index = flavorNodeIndex.index;
        // if(index == 1) {
        //     // If we tap a scoop that is not the base of a flavor node then
        //     // we must expand the scoop into it's own FlavorNode
        //     Scoop scoop = node.scoops[index];
        //     FlavorNode newNode = new FlavorNode(scoop);
        //     node.scoops.RemoveAt(index);
        //     scoop.flavorNodeIndex = new FlavorNodeIndex(newNode, 0);
        
        //     newNode.above = node.above;
        //     newNode.below = node;

        //     node.above = newNode;
        //     newNode.above.below = newNode;
            
        //     // Set node = newNode so we can proceed as if newNode was tapped
        //     node = newNode;
        // } 

        // //if index == 0 then we don't need to create a new node.
        // FlavorNode breakNode = node.below;
        // FlavorNode cur = node;
        // FlavorNode prev = null;


        // top.below = breakNode;
        // breakNode.above = top;

        // while(cur != breakNode) {
        //     FlavorNode next = cur.above;
        //     cur.above = prev;
        //     cur.below = next;
        //     prev = cur;
        //     cur = next;
        // }
        // top = node;
        // top.above = bottom;
        // bottom.below = top;

        // if(breakNode.flavor == breakNode.above.flavor) {
        //     MergeFlavorNodes(breakNode, breakNode.above);
        // }

    }

    private void MergeFlavorNodes(FlavorNode bottomNode, FlavorNode topNode) {
        if(bottomNode == topNode) {
            return;
        }
        foreach(Scoop scoop in topNode.scoops) {
                bottomNode.scoops.Add(scoop);
                scoop.flavorNodeIndex = new FlavorNodeIndex(bottomNode, bottomNode.scoops.Count - 1);
        }
        bottomNode.above = topNode.above;
        topNode.above.below = bottomNode;

        if(topNode == top) {
            top = bottomNode;
            bottom.below = top;
        } 
        if(bottomNode.scoops.Count >= 3) {
            Count -= bottomNode.Count;
            ScoopMatchFound(bottomNode.scoops);

            if(bottom == top) {
                bottom = null;
                top = null;
            } else {
                bottomNode.below.above = bottomNode.above;
                bottomNode.above.below = bottomNode.below;
                if(bottomNode.above.flavor == bottomNode.below.flavor) {
                    MergeFlavorNodes(bottomNode.above, bottomNode.below);
                }
            }
        }
    }

    public string StackInfo_Debug() {
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
       return "Top: " + top.Debug() + " Bottom: " +  bottom.Debug() + "\nScoopStack flavor nodes: " + flavorNodes + "]";
    }

}
