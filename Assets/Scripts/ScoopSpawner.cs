using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopSpawner : MonoBehaviour
{

    public Scoop scoopPrefab;
    public Grid grid;
    public float speed;

    public Color[] flavors;
    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 1) {
            if (Input.GetTouch(1).phase == TouchPhase.Began) {
                ChooseScoop();
            }
        }
    }

    private void ChooseScoop() {
        Scoop scoop = Instantiate(scoopPrefab, transform.position, transform.rotation) as Scoop;
        scoop.SetFlavor(RandomFlavor());
        scoop.SetSpeed(speed);
        scoop.Initialize(grid, new Vector2Int(Random.Range(0,3), grid.numberOfRows - 1));
    }

    private Color RandomFlavor() {
        return flavors[Random.Range(0,flavors.Length)];
    }
}
