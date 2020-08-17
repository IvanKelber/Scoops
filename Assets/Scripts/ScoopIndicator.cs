using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopIndicator : MonoBehaviour
{
    private Color incomingFlavor;
    private Renderer renderer;
    private Vector3 staticPosition;

    private void Start() {
        renderer = GetComponent<Renderer>();
    }

    private void Update() {
        renderer.sharedMaterial.color = incomingFlavor;
        transform.position = staticPosition;
    }

    public void SetIncomingFlavor(Color flavor) {
        incomingFlavor = flavor;
    }

    public void SetPosition(Vector3 position) {
        this.staticPosition = position;
    }
}
