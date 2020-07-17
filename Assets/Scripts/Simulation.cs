using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simulation : MonoBehaviour {
    public Grid Grid;
    public GameObject PedestrianContainer;
    public bool IsSimulating {get; private set;}
    public int CurrentFrame = 0;
    public int RecordedFrames = 0;
    [HideInInspector] public Pedestrian[] Pedestrians;
    private float[,] dijkstraField;
    private float dijkstraMax = 0f;
    private int[,] pedestrianCounts;
    private int[,] targetCounts;

    private void Awake() {
        Pedestrians = PedestrianContainer.GetComponentsInChildren<Pedestrian>();
    }

    private void Start() {
        var start = Time.realtimeSinceStartup;
        dijkstraField = Pathfinding.CreateDijkstraField(Grid.Cols, Grid.Rows, Grid.GridContent);
        Debug.Log($"It takes: {Time.realtimeSinceStartup - start} seconds");
        foreach(var i in dijkstraField) {
            if(i > dijkstraMax && i < Mathf.Infinity) {
                dijkstraMax = i;
            }
        }
        SetupPedestrianDensityField();
    }

    private void SetupPedestrianDensityField() {
        targetCounts = new int[Grid.Cols, Grid.Rows];
        pedestrianCounts = new int[Grid.Cols, Grid.Rows];
        foreach(var pedestrian in Pedestrians) {
            var cell = Grid.GetCellCoordinate(pedestrian.transform.position);
            pedestrianCounts[cell.x, cell.y] ++;
            pedestrian.CurrentCell = cell;
            pedestrian.PreviousCell = cell;
            pedestrian.PreviousPreviousCell = cell;

            pedestrian.AddToPositionHistory(pedestrian.transform.position);
        }
    }
    
    private void FixedUpdate() {
        if(!IsSimulating) {
            return;
        }
        
        CurrentFrame++;
        RecordedFrames++;

        foreach(var pedestrian in Pedestrians) {
            if(pedestrian.TargetCell != Vector2Int.one * -1) {
                targetCounts[pedestrian.TargetCell.x, pedestrian.TargetCell.y] -=1;
                pedestrian.TargetCell = Vector2Int.one * -1;
            }

            var pos = pedestrian.transform.position;

            var cell = Grid.GetCellCoordinate(pos);
            if(pedestrian.CurrentCell != cell) {
                pedestrianCounts[cell.x, cell.y] ++;
                pedestrianCounts[pedestrian.CurrentCell.x, pedestrian.CurrentCell.y] --;
                pedestrian.PreviousPreviousCell = pedestrian.PreviousCell;
                pedestrian.PreviousCell = pedestrian.CurrentCell;
                pedestrian.CurrentCell = cell;
            }

            var neighbors = Pathfinding.GetAllNeighbors(cell, Grid.Cols, Grid.Rows, Grid.GridContent);

            float currentMinDistance = Mathf.Infinity;
            Vector2Int currentSelectedCell = Vector2Int.one * -1;
            
            int tries = 0;
            while(tries < 3 && currentSelectedCell == Vector2Int.one * -1) {
                foreach(var neighbor in neighbors) {
                    if(neighbor == pedestrian.PreviousPreviousCell && tries < 1) {
                        continue;
                    }
                    if(neighbor == pedestrian.PreviousCell && tries < 2) {
                        continue;
                    }
                    if((dijkstraField[neighbor.x, neighbor.y] + targetCounts[neighbor.x, neighbor.y] * 0.1f) < currentMinDistance && pedestrianCounts[neighbor.x, neighbor.y] == 0) {
                        currentSelectedCell = neighbor;
                        currentMinDistance = dijkstraField[neighbor.x, neighbor.y] + targetCounts[neighbor.x, neighbor.y] * 0.1f;
                    }
                }
                tries ++;
            }
            
            var directionVector = Vector3.zero;
            if(currentSelectedCell != Vector2Int.one * -1 && currentSelectedCell != cell) {
                if(Mathf.Abs(currentSelectedCell.x - cell.x) + Mathf.Abs(currentSelectedCell.y - cell.y) != 1) {
                    foreach(var neighbor in neighbors) {
                        if(pedestrianCounts[neighbor.x, neighbor.y] > 0) {
                            var avoidPos = Grid.GetCoordinateFromCell(neighbor);
                            directionVector -= (new Vector3(avoidPos.x - pos.x, 0, avoidPos.y - pos.z)) / Mathf.Sqrt(2);
                        }
                    }
                }

                targetCounts[currentSelectedCell.x, currentSelectedCell.y] ++;
                pedestrian.TargetCell = currentSelectedCell;
                var targetPos = Grid.GetCoordinateFromCell(currentSelectedCell);
                directionVector += new Vector3(targetPos.x - pos.x, 0, targetPos.y - pos.z);
            }
            
            pedestrian.transform.position = pos + directionVector.normalized / 10f;
            pedestrian.AddToPositionHistory(pedestrian.transform.position);
        }
    }

    public void StartSimulation() {
        if(RecordedFrames == 0) {
            IsSimulating = true;
        }
        else {
            foreach(var pedestrian in Pedestrians) {
                pedestrian.transform.position = pedestrian.previousPositions[0];
                pedestrian.previousPositions.Clear();
            }
            SetupPedestrianDensityField();
            CurrentFrame = 0;
            IsSimulating = true;
        }
    }

    public void StopSimulation() {
        IsSimulating = false;
    }

    public void Seek(int frameNumber) {
        CurrentFrame = frameNumber;
        foreach(var pedestrian in Pedestrians) {
            pedestrian.transform.position = pedestrian.previousPositions[Mathf.Min(pedestrian.previousPositions.Count - 1, CurrentFrame)];
        }
    }

    private void Update() {
        if(UIManager.Instance.ShowDijkstraField) {
            var colors = new Color[Grid.MeshFilter.mesh.vertexCount];
            for(var x = 0; x < Grid.Cols; x++) {
                for(var y = 0; y < Grid.Rows; y++) {
                    var color = Color.Lerp(Color.black, Color.white, dijkstraField[x, y]/dijkstraMax);
                    
                    var val = y * Grid.Cols + x;
                    colors[val * 6] = color;
                    colors[val * 6 + 1] = color;
                    colors[val * 6 + 2] = color;
                    colors[val * 6 + 3] = color;
                    colors[val * 6 + 4] = color;
                    colors[val * 6 + 5] = color;
                }
            }
            Grid.MeshFilter.mesh.SetColors(colors);
        }   
    }
}