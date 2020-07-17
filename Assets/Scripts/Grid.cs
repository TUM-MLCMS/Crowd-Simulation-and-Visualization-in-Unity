using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Grid : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public Collider Collider;
    public GridElements[,] GridContent;
    public int Rows;
    public int Cols;
    public GameObject StaticObjectContainer; 
    public Material DefaultMaterial;
    public Material DijkstraMaterial;

    private void Awake() {
        Cols = (int) Collider.bounds.size.x;
        Rows = (int) Collider.bounds.size.z;
        
        GenerateCustomMesh();

        GridContent = new GridElements[Cols, Rows];

        for(var x = 0; x < Cols; x++) {
            for(var y = 0; y < Rows; y++) {
                GridContent[x, y] = GridElements.EMPTY;
            }
        }

        var elements = StaticObjectContainer.GetComponentsInChildren<SimulationElement>();
        foreach(var element in elements) {
            AddElement(element.Collider, element.Type);
        }
    }

    private void GenerateCustomMesh() {
        var mesh = new Mesh();
        Vector3[] vertices = new Vector3[Cols * Rows * 6];

        var counter = 0;

        var xStart = (transform.position.x - Collider.bounds.extents.x) / transform.localScale.x;
        var zStart = (transform.position.z - Collider.bounds.extents.z) / transform.localScale.z;

        var xInterval = Collider.bounds.size.x / Cols / transform.localScale.x;
        var zInterval = Collider.bounds.size.z / Rows / transform.localScale.z;

        while(counter < Cols * Rows * 6) {
            var i = counter / 6;

            var x = xStart + (i % Cols) * xInterval;
            var y = zStart + (i / Cols) * zInterval;

            vertices[counter]     = new Vector3(x, 0, y);
            vertices[counter + 1] = new Vector3(x, 0, y + zInterval);
            vertices[counter + 2] = new Vector3(x + xInterval, 0, y);

            vertices[counter + 3] = new Vector3(x, 0, y + zInterval);
            vertices[counter + 4] = new Vector3(x + xInterval, 0, y + zInterval);
            vertices[counter + 5] = new Vector3(x + xInterval, 0, y);
            counter += 6;
        }

        var triangles = new int[Cols * Rows * 6];
        for(int t = 0; t < Cols * Rows * 6; t++) {
            triangles[t] = t;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        MeshFilter.mesh = mesh;
    }

    public void AddElement(Collider col, GridElements type) {
        var xStart = col.transform.position.x - col.bounds.extents.x;
        var xEnd = col.transform.position.x + col.bounds.extents.x;

        var zStart = col.transform.position.z - col.bounds.extents.z;
        var zEnd = col.transform.position.z + col.bounds.extents.z;

        var startCell = GetCellCoordinate(new Vector3(xStart, 0, zStart));
        var endCell = GetCellCoordinate(new Vector3(xEnd, 0, zEnd));

        for(var x = startCell.x; x <= endCell.x; x++) {
            for(var y = startCell.y; y <= endCell.y; y++) {
                if(x >= 0 && x < Cols && y >= 0 && y < Rows) {
                    GridContent[x, y] = type;
                }
            }
        }
    }

    public Vector2Int GetCellCoordinate(Vector3 position) {
        var xStart = transform.position.x - Collider.bounds.extents.x;
        var zStart = transform.position.z - Collider.bounds.extents.z;

        var x = (int) (position.x - xStart);
        var z = (int) (position.z - zStart);
        
        return new Vector2Int(x, z);
    }

    public Vector2 GetCoordinateFromCell(Vector2Int cell) {
        var xStart = transform.position.x - Collider.bounds.extents.x;
        var zStart = transform.position.z - Collider.bounds.extents.z;

        var xInterval = Collider.bounds.size.x / Cols;
        var zInterval = Collider.bounds.size.z / Rows;

        return new Vector2(xStart + 0.5f + cell.x * xInterval, zStart + 0.5f + cell.y * zInterval);
    }

    public void ResetMeshColors(bool isDijkstra) {
        var colors = new Color[MeshFilter.mesh.vertexCount];
        for(var i = 0; i < MeshFilter.mesh.vertexCount; i++) {
            colors[i] = Color.white;
        }
        MeshFilter.mesh.SetColors(colors);
        GetComponent<Renderer>().material = isDijkstra ? DijkstraMaterial : DefaultMaterial;
    }
}
