using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pedestrian : MonoBehaviour {
    [HideInInspector] public Vector2Int CurrentCell;
    [HideInInspector] public Vector2Int PreviousCell;
    [HideInInspector] public Vector2Int PreviousPreviousCell;
    public List<Vector3> previousPositions = new List<Vector3>();

    public void AddToPositionHistory(Vector3 pos) {
        pos.y = 1;
        previousPositions.Add(pos);
    }
}