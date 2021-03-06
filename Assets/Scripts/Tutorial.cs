﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DentedPixel;


public class Tutorial : MonoBehaviour
{

    public ScoopManager scoopManager;
    public BoardManager board;

    Scoop[] scoops = new Scoop[4];

    private TutorialStep step;

    [SerializeField]
    private TutorialFinger tutorialFinger;

    public enum TutorialStep {
        Init, Swipe, WaitingForSwipe, Tap, WaitingForTap, Done
    };

    void Start()
    {
        if(!board.RunTutorial) {
            board.scoopManager.spawning = true;
            Destroy(gameObject);
            return;
        }
        step = TutorialStep.Init;
        BoardManager.UnfreezeGame += IncrementStep;
    }

    void Update()
    {
        ContinueTutorial();
    }

    public void Init() {
        Flavor matchFlavor = scoopManager.flavors[0];
        Flavor nonMatchFlavor = scoopManager.flavors[1];
        for(int i = 0; i < scoops.Length; i++) {
            scoops[i] = scoopManager.InstantiateScoop();
            Vector2Int coneIndex = new Vector2Int(board.ConeLane(), board.ConeStackHeight() + i);
            scoopManager.SetScoop(scoops[i], i == 1 ? nonMatchFlavor: matchFlavor, scoopManager.speed, i == 3 ? new Vector2Int(coneIndex.x + 1, board.TotalRows - 1) :coneIndex);
        }
        step = TutorialStep.Swipe;
    }



    public void ContinueTutorial() {
        switch(step) {
            case TutorialStep.Init:
                Init();
                break;
            case TutorialStep.WaitingForSwipe:
            case TutorialStep.WaitingForTap: 
                break;
            case TutorialStep.Swipe:
                if(scoops[3].currentIndex.y < board.numberOfRows - 3) {
                    tutorialFinger.StartSwipe();
                    board.Freeze();
                    step = TutorialStep.WaitingForSwipe;
                }
                break;
            case TutorialStep.Tap:
                if(board.ConeStackHeight() == scoops.Length + 1) {
                    tutorialFinger.StartTap(board.GetScreenPosition(scoops[1].currentIndex.x, scoops[1].currentIndex.y));
                    board.Freeze();
                    step = TutorialStep.WaitingForTap;
                }
                break;
            case TutorialStep.Done:
                StartCoroutine(Destroy());
                break;
        }
    }

    public void IncrementStep() {
        step++;
        tutorialFinger.StopTween();
    }

    public TutorialStep GetStep() {
        return step;
    }

    public IEnumerator Destroy() {
        yield return new WaitForSeconds(.1f);
        board.scoopManager.spawning = true;
        tutorialFinger.Destroy();
        BoardManager.UnfreezeGame -= IncrementStep;
        Destroy(gameObject);
        yield return null;
    }

}
