using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBob : MonoBehaviour
{

    public float yHeight;
    public float bobSpeed;

    void Update()
    {
        Vector3 bob = Vector3.up * Mathf.Sin(bobSpeed * (Time.time)) * yHeight;
        transform.Translate(bob);
    }

}
