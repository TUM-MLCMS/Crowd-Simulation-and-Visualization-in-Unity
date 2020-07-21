using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pedestrian : MonoBehaviour {
    [HideInInspector] public Vector2Int CurrentCell;
    [HideInInspector] public Vector2Int TargetCell = new Vector2Int(-1, -1);
    public float TargetCellScore;
    //Debugging only
    public Pedestrian NearestPedestrian;
    public float CurrentDistance;
    public float CurrentSpeed;    
    public List<Vector3> previousPositions = new List<Vector3>();

    public void AddToPositionHistory(Vector3 pos) {
        pos.y = 1;
        previousPositions.Add(pos);
    }
}