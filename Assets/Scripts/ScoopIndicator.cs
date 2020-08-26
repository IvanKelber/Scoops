using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopIndicator : MonoBehaviour
{
    private Flavor incomingFlavor;
    private Renderer renderer;
    private Vector3 staticPosition;

    private void Start() {
        renderer = GetComponent<Renderer>();
    }

    private void Update() {
        renderer.material.SetColor("_MainColor", incomingFlavor.color);
        transform.position = staticPosition;
    }

    public void SetIncomingFlavor(Flavor flavor) {
        incomingFlavor = flavor;
    }

    public void SetPosition(Vector3 position) {
        this.staticPosition = position;
    }
}
