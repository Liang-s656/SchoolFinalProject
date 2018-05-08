using System.Collections.Generic;
using UnityEngine;

public class Island: Grid
{   
    [Header("Island main settings:")]
    public int islandDiameter;

    [Header("Seed for terrain:")]
    public Vector3 seed;
    public float seedFactor = 3;

    [Header("Seed for vegetation:")]
    public GameObject[] treeObj;
    public Vector3 treeSeed;
    public float treeSeedFactor;
    public float treeStartHeight = 10;


    private void Start() {
        ClearGrid(islandDiameter);
    }

    private void PopulateGridData() {
        float gridRadius = islandDiameter / 2.0f;
        for (int y = 0; y < islandDiameter; y++) {
            for (int x = 0; x < islandDiameter; x++) {
                if (PointInCircle(x, y)) {
                    Vector3 tilesCenterPoint = new Vector3(x - gridRadius, GetTilesHeight(x, y), y - gridRadius);
                    grid[x, y] = new Tile(tilesCenterPoint);
                    if(PlaceTree(tilesCenterPoint)){
                        Tile tile = (Tile)grid[x,y];
                        tile.containsTree = true;
                    }
                }
            }
        }
    }
    private bool PlaceTree(Vector3 point) {
        float p = Mathf.PerlinNoise(point.x / treeSeed.x, point.z / treeSeed.z) * treeSeed.y;
        return p > treeSeedFactor && point.y > treeStartHeight;
    }

    private void PopulateTilesNeighbors(int x, int y) {
        Tile main = (Tile)grid[x,y];        
        if (x > 0 && PointInCircle(x - 1, y)) {
            Tile left = (Tile)grid[x - 1, y];
            main.SetNeighbor(left, "LEFT");
            left.SetNeighbor(main, "RIGHT");
        }
        if (y > 0 && PointInCircle(x, y - 1)) {
            Tile top = (Tile)grid[x, y - 1];
            main.SetNeighbor(top, "TOP");
            top.SetNeighbor(main, "BOTTOM");            
        }
    }
    private float GetTilesHeight(int x, int y) {
        float height = Mathf.RoundToInt(Mathf.PerlinNoise(x / seed.x, y / seed.z) * seed.y) * seedFactor;
        return height;
    }
    private bool PointInCircle(int x, int y) {
        float radius = islandDiameter / 2.0f;
        return Mathf.Pow(x - radius, 2) + Mathf.Pow(y - radius, 2) < radius * radius;
    }
    public void SetMesh()
    {
        MeshFilter meshFilter = GetComponent < MeshFilter > ();
        Mesh mesh = GetMesh();
        meshFilter.mesh = mesh;

        MeshCollider meshCollider = GetComponent < MeshCollider > ();
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private Mesh GetMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int offset =0;
        for (int y = 0; y < islandDiameter; y++) {
            for (int x = 0; x < islandDiameter; x++) {
                if (grid[x, y] == null)
                    continue;

                Tile tile = (Tile)grid[x, y];
                TileStructure tileStructure = tile.tileStructure;
                tileStructure.GenerateTile();

                List<Vector3> tilesVertices = tileStructure.vertices;
                List < int > tilesTriangles = new List<int>();
                tilesTriangles.AddRange(tileStructure.triangles);

                for (int i = 0; i < tilesTriangles.Count; i++) {
                    tilesTriangles[i] += offset;
                }

                vertices.AddRange(tilesVertices);
                triangles.AddRange(tilesTriangles);
                normals.AddRange(tileStructure.normals);
                uvs.AddRange(tileStructure.uvs);

                //Adds tree
                if (tile.containsTree){
                    InitTree(tile.location, tile.getTreeType(), tile.getTreeRotation());
                }

                offset += tilesVertices.Count;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();        
        return mesh;
    }

    private void InitTree(Vector3 cord, int type, int rotation) {
        GameObject tree = (GameObject) Instantiate(treeObj[type]);
        tree.name = "Tree-" + type + ":" + rotation;
        tree.transform.position = new Vector3(cord.x + transform.position.x, cord.y, cord.z + transform.position.z);
        tree.transform.parent = transform;
        tree.transform.eulerAngles = new Vector3(0, rotation, 0);
    }
}