using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {
    public Simulation Simulation;
    public Material TrajectoryMaterial;
    public Material GridMaterial;

    /**
    *** This function runs after Unity draws all the objects in the scene.
    *** Rendering lines for Trajectories and Grid if they are selected in UI. 
    **/ 
    public void OnPostRender() {
        TrajectoryMaterial.SetPass(0);

        if(UIManager.Instance.ShowTrajectories) {
            foreach(var pedestrian in Simulation.Pedestrians) {
                GL.Begin(GL.LINES);
                for(var i = 0; i < Mathf.Min(pedestrian.previousPositions.Count - 1, Simulation.CurrentFrame); i++) {
                    GL.Vertex(pedestrian.previousPositions[i]);
                    GL.Vertex(pedestrian.previousPositions[i+1]);
                }
                GL.End();
            }
        }

        if(UIManager.Instance.ShowGrid) {
            var grid = Simulation.Grid;
            var Cols = (int) grid.Collider.bounds.size.x;
            var Rows = (int) grid.Collider.bounds.size.z;

            var xStart = grid.transform.position.x - grid.Collider.bounds.extents.x;
            var xEnd = grid.transform.position.x + grid.Collider.bounds.extents.x;

            var zStart = grid.transform.position.z - grid.Collider.bounds.extents.z;
            var zEnd = grid.transform.position.z + grid.Collider.bounds.extents.z;

            var xInterval = grid.Collider.bounds.size.x / Cols;
            var zInterval = grid.Collider.bounds.size.z / Rows;

            var y = grid.transform.position.y + 0.01f;
            GridMaterial.SetPass(0);

            for(int i = 0; i < Cols + 1; i++) {
                GL.Begin(GL.LINES);
                GL.Vertex3(xStart + i * xInterval, y, zStart);
                GL.Vertex3(xStart + i * xInterval, y, zEnd);
                GL.End();
            }

            for(int i = 0; i < Rows + 1; i++) {
                GL.Begin(GL.LINES);
                GL.Vertex3(xStart, y, zStart + i * zInterval);
                GL.Vertex3(xEnd, y, zStart + i * zInterval);
                GL.End();
            }
        }
    }
}