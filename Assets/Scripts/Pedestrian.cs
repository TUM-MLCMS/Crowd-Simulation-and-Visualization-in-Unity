using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
*** Component that is attached to every pedestrian. 
**/ 
public class Pedestrian : MonoBehaviour {
    [HideInInspector] public Vector2Int CurrentCell;
    [HideInInspector] public Vector2Int TargetCell = new Vector2Int(-1, -1);
    public Pedestrian NearestPedestrian; // Closest pedestrian to adjust speed
    public float CurrentSpeed;
    public List<Vector3> previousPositions = new List<Vector3>();

    public void AddToPositionHistory(Vector3 pos) {
        pos.y = 1;
        previousPositions.Add(pos);
    }
}