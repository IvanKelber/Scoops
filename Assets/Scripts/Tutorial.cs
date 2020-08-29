using System.Collections;
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
        Init, Swipe, Tap, Done
    };

    void Start()
    {
        step = TutorialStep.Init;
    }

    // Update is called once per frame
    void Update()
    {
        if(step == TutorialStep.Init) {
            Flavor matchFlavor = scoopManager.flavors[0];
            Flavor nonMatchFlavor = scoopManager.flavors[1];
            for(int i = 0; i < scoops.Length; i++) {
                scoops[i] = scoopManager.InstantiateScoop();
                Vector2Int coneIndex = new Vector2Int(board.ConeLane(), board.ConeStackHeight() + i);
                scoopManager.SetScoop(scoops[i], i == 1 ? nonMatchFlavor: matchFlavor, scoopManager.speed, i == 3 ? new Vector2Int(coneIndex.x + 1, board.TotalRows - 1) :coneIndex);
            }; 
            step = TutorialStep.Swipe;
        }

        if(step == TutorialStep.Swipe && scoops[3].currentIndex.y < board.numberOfRows - 3) {
            // last generated scoop has reached the game screen.
            tutorialFinger.StartSwipe();
            board.FreezeGame();
        } else if (step != TutorialStep.Swipe){
            tutorialFinger.StopSwipe();
        }

        if(step == TutorialStep.Tap && board.ConeStackHeight() == scoops.Length + 1) {
            tutorialFinger.StartTap(board.GetScreenPosition(scoops[1].currentIndex.x, scoops[1].currentIndex.y));
            board.FreezeGame();
        } else if (step != TutorialStep.Tap) {
            tutorialFinger.StopTap();
        }
    }

    public void SetStep(TutorialStep step) {
        this.step = step;

    }

    public TutorialStep GetStep() {
        return step;
    }

    public void Destroy() {
        tutorialFinger.Destroy();
        Destroy(gameObject);
    }

}
