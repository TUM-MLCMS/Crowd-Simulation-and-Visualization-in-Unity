using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simulation : MonoBehaviour {
    public Grid Grid;
    public Transform[] Pedestrians;
    private float[,] dijkstraField;

    private void Start() {
        dijkstraField = Pathfinding.CreateDijkstraField(Grid.Cols, Grid.Rows, Grid.GridContent);
    } 
    
    private void FixedUpdate() {
        foreach(var pedestrian in Pedestrians) {
            var cell = Grid.GetCellCoordinate(pedestrian.position);

            var neighbors = Pathfinding.GetAllNeighbors(cell, Grid.Cols, Grid.Rows, Grid.GridContent);

            float currentMinDistance = Mathf.Infinity;
            Vector2Int currentSelectedCell = Vector2Int.zero;

            foreach(var neighbor in neighbors) {
                if(dijkstraField[neighbor.x, neighbor.y] < currentMinDistance) {
                    currentSelectedCell = neighbor;
                    currentMinDistance = dijkstraField[neighbor.x, neighbor.y];
                }
            }
            
            var pos = pedestrian.position;
            var target_pos = Grid.GetCoordinateFromCell(currentSelectedCell);
            
            var directionVector = new Vector3(target_pos.x - pos.x, 0, target_pos.y - pos.z);
            pedestrian.position = pos + directionVector.normalized / 10f;
        }
    }
}