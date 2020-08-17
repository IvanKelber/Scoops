using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapStart : MonoBehaviour
{
    public float yThreshold;
    void Start()
    {
        Gestures.OnTap += StartGame;
    }

    private void StartGame(Vector3 position) {
        Debug.Log(position);
        if(position.y <= yThreshold) {
            Gestures.OnTap -= StartGame;
            SceneState.LoadScene(1);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawRay(new Vector3(0,yThreshold,1), Vector3.right * 10);
    }

}
