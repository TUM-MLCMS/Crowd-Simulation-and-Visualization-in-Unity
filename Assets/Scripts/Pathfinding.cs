using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
*** This class contains everything related to Dijkstra's Algorithm.
**/
public static class Pathfinding {
    private static bool[,] visitedCells;
    private static Vector2Int nullCell = new Vector2Int(-1, -1);
    public static float[,] CreateDijkstraField(int cols, int rows, GridElements[,] gridElements) {
        visitedCells = new bool[cols, rows];
        var field = new float[cols, rows];

        for(var x = 0; x < cols; x++) {
            for(var y = 0; y < rows; y++) {
                if(gridElements[x, y] == GridElements.TARGET) {
                    field[x, y] = 0;
                }
                else {
                    field[x, y] = Mathf.Infinity;
                }
            }
        }

        while(true) {
            var cell = GetSmallestDistanceCell(field, cols, rows);
            if(cell == nullCell) {
                break;
            }
            visitedCells[cell.x, cell.y] = true;
            var neighbors = GetAllNeighbors(cell, cols, rows, gridElements);
            foreach(var neighbor in neighbors) {
                var newDistance = field[cell.x, cell.y] + Vector2Int.Distance(cell, neighbor); 
                if(newDistance < field[neighbor.x, neighbor.y]) {
                    field[neighbor.x, neighbor.y] = newDistance;
                }
            }
        }

        return field;
    }

    public static List<Vector2Int> GetAllNeighbors(Vector2Int cell, int cols, int rows, GridElements[,] gridElements) {
        var neighbors = new List<Vector2Int>();

        var startCol = Mathf.Max(0, cell.x - 1);
        var startRow = Mathf.Max(0, cell.y - 1);

        var endCol = Mathf.Min(cell.x + 1, cols - 1);
        var endRow = Mathf.Min(cell.y + 1, rows - 1);
        
        for(var x = startCol; x <= endCol; x++) {
            for(var y = startRow; y <= endRow; y++) {
                if(gridElements[x, y] != GridElements.OBSTACLE) {
                    neighbors.Add(new Vector2Int(x, y));
                }
            }
        }

        return neighbors;
    }

    private static Vector2Int GetSmallestDistanceCell(float[,] field, int cols, int rows) {
        float currentMinDistance = Mathf.Infinity;
        Vector2Int currentSelectedCell = new Vector2Int(-1, -1);

        for(var x = 0; x < cols; x++) {
            for(var y = 0; y < rows; y++) {
                if(field[x, y] < currentMinDistance && !visitedCells[x, y]) {
                    currentSelectedCell.Set(x, y);
                    currentMinDistance = field[x, y];
                }
            }
        }

        return currentSelectedCell;
    }
}