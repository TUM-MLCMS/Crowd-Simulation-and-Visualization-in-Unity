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
    private List<Pedestrian>[,] pedestrianField;

    /**
    *** Before anything else, this code finds all pedestrians with Pedestrian component
    **/
    private void Awake() {
        Pedestrians = PedestrianContainer.GetComponentsInChildren<Pedestrian>();
    }

    /**
    *** Creates dijkstra distance field
    *** Also finds the maximum value in dijkstra field, later to be used for dijkstra distance field visualization
    **/
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

    /**
    *** Creates a 2D array of Pedestrian Lists to store which pedestrians are registered to cells.
    **/
    private void SetupPedestrianDensityField() {
        pedestrianField = new List<Pedestrian>[Grid.Cols, Grid.Rows];
        for(var x = 0; x < Grid.Cols; x++) {
            for(var y = 0; y < Grid.Rows; y++) {
                pedestrianField[x, y] = new List<Pedestrian>();
            }
        }
        foreach(var pedestrian in Pedestrians) {
            var cell = Grid.GetCellCoordinate(pedestrian.transform.position);
            pedestrianField[cell.x, cell.y].Add(pedestrian);
            pedestrian.CurrentCell = cell;
            pedestrian.AddToPositionHistory(pedestrian.transform.position);
        }
    }
    
    /**
    *** Runs 50 times a second.
    *** Processes every pedestrian.
    *** Gets all the neighbors of a pedestrian, calls the utility function for each one of them.
    *** Decides on one of the neighbors to move towards, creates a direction vector.
    *** Adjusts speed if it's too close to another pedestrian
    *** Saves its new position to its history
    **/
    private void FixedUpdate() {
        if(!IsSimulating) {
            return;
        }
        
        CurrentFrame++;
        RecordedFrames++;

        foreach(var pedestrian in Pedestrians) {
            var pos = pedestrian.transform.position;
            var cell = Grid.GetCellCoordinate(pos);

            if(pedestrian.CurrentCell != cell) {
                pedestrianField[cell.x, cell.y].Add(pedestrian);
                pedestrianField[pedestrian.CurrentCell.x, pedestrian.CurrentCell.y].Remove(pedestrian);
                pedestrian.CurrentCell = cell;
            }

            var neighbors = Pathfinding.GetAllNeighbors(cell, Grid.Cols, Grid.Rows, Grid.GridContent);

            float currentMinCost = Mathf.Infinity;
            Vector2Int currentSelectedCell = Vector2Int.one * -1;
            
            foreach(var neighbor in neighbors) {
                var cost = GetCost(neighbor, pedestrian);

                if(cost < currentMinCost) {
                    currentSelectedCell = neighbor;
                    currentMinCost = cost;
                }
            }
            
            float currentNearestPedDistance = Mathf.Infinity;
            var directionVector = Vector3.zero;
            if(currentSelectedCell != Vector2Int.one * -1 && currentSelectedCell != cell) {
                pedestrian.TargetCell = currentSelectedCell;
                var targetPos = Grid.GetCoordinateFromCell(currentSelectedCell);

                directionVector += new Vector3(targetPos.x - pos.x, 0, targetPos.y - pos.z);

                foreach(var neighbor in neighbors) {
                    foreach(var pedestrian2 in pedestrianField[neighbor.x, neighbor.y]) {
                        if(pedestrian == pedestrian2) {
                            continue;
                        }

                        if(pedestrian2.NearestPedestrian == pedestrian) {
                            continue;
                        }
                        
                        var dist1 = Vector3.Distance(pedestrian2.transform.position, pedestrian.transform.position);
                        var dist2 = Vector3.Distance(pedestrian2.transform.position, pedestrian.transform.position + directionVector.normalized * 0.1f);
                       
                        if(dist2 < dist1 && dist1 < currentNearestPedDistance) {
                            currentNearestPedDistance = dist1;
                            pedestrian.NearestPedestrian = pedestrian2;
                        }
                    }
                }
            }
            
            pedestrian.CurrentSpeed = Mathf.Max(Mathf.Min((currentNearestPedDistance - 1f) / 10f, 0.1f), 0);
            pedestrian.transform.position = pos + directionVector.normalized * pedestrian.CurrentSpeed;
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
            RecordedFrames = 0;
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

    /**
    *** Utility (cost) function.
    *** Calculate the distance to all other pedestrians if the current pedestrian decides to the "pos"
    *** Sum it with dijkstra distance field value.
    **/
    private float GetCost(Vector2Int pos, Pedestrian pedestrian) {
        var neighbors = Pathfinding.GetAllNeighbors(pos, Grid.Cols, Grid.Rows, Grid.GridContent);
        var extraCosts = 0f;

        var targetPos = Grid.GetCoordinateFromCell(pos);
        var pedPos = pedestrian.transform.position;
        var velocity = new Vector3(targetPos.x - pedPos.x, 0, targetPos.y - pedPos.z).normalized * 0.1f;
        var newPos = pedPos + velocity;

        foreach(var neighbor in neighbors) {            
            foreach(var pedestrian2 in pedestrianField[neighbor.x, neighbor.y]) {
                extraCosts += 5 * Mathf.Exp(-1 * Vector3.Distance(pedestrian2.transform.position, newPos));
            }
        }

        return dijkstraField[pos.x, pos.y] + extraCosts;
    }

    /**
    *** Used to visualize dijkstra field on the grid.
    *** This doesn't have to be in a Update function.
    *** But we used it to highlight cells that have a pedestrian on them. 
    **/
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