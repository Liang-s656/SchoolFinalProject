using System.Collections.Generic;
using UnityEngine;
public class TileStructure{
    public Vector3 location;
	public List <Vector3> vertices;
    public List <Vector3> normals;    
    public List <Vector2> uvs;
    public List <int> triangles;

    public TileNeighbors neighbors;

    private bool bottomVerticesInitiated = false;

    public TileStructure() {
        Init();
    }

    private void Init() {
        this.vertices = new List <Vector3> ();
        this.triangles = new List <int> ();
        this.uvs = new List <Vector2> ();
        this.normals = new List <Vector3> ();
        this.neighbors = new TileNeighbors();
    }

    public void GenerateTile(){
        GenerateTopVertices(location.y);
        AddFacesTriangles(0, 1, 2, 3);

        if(AddSide(neighbors.top)) {
            GenerateTileSide(1, 0, 5, 4);
        }
        if(AddSide(neighbors.right)) {
            GenerateTileSide(3, 1, 7, 5);            
        }
        if(AddSide(neighbors.bottom)) {
            GenerateTileSide(2, 3, 6, 7);            
        }
        if(AddSide(neighbors.left)) {
            GenerateTileSide(0, 2, 4, 6);
        }

        PopulateNormals();
    }

    private bool AddSide(Tile neighbor) {
        return neighbor == null || neighbor.location.y != location.y;
    }

    public void SetHeight(float height){
        if(height < 0 ) height = 0;
        this.location = new Vector3(this.location.x, height, this.location.z);
    }

    private void GenerateTopVertices(float height) {
        for (int y = 0; y < 2; y++) {
            for (int x = 0; x < 2; x++) {
                vertices.Add( new Vector3( location.x - 0.5f + x, height, location.z - 0.5f + y ));
            }
        }
        AddUV(height);
    }

    private void PopulateNormals() {
        for (int i = 0; i < vertices.Count; i++) {
            normals.Add(Vector3.up);
        }
    }

    private void AddUV(float y) {
        int height = Mathf.RoundToInt(y);

        int rowCount = 10;
        float top = (rowCount - height <= 0) ? 0.9f : 1.0f / (rowCount - height) - 0.1f;

        for (int z = 0; z < 2; z++) {
            for (int x = 0; x < 2; x++) {
                uvs.Add(new Vector2(0, top));
            }
        }
    }

    private void GenerateTileSide(int topLeft, int topRight, int bottomLeft, int bottomRight) {
        if( location.y == 0 ) return;
        if(!bottomVerticesInitiated){
            GenerateTopVertices( 0 );
            bottomVerticesInitiated = true;
        }
         AddFacesTriangles(topLeft, topRight, bottomLeft, bottomRight);
    }

    private void AddFacesTriangles(int topLeft, int topRight, int bottomLeft, int bottomRight) {
        // 1st triangle
        triangles.Add(topLeft);
        triangles.Add(bottomLeft);
        triangles.Add(bottomRight);

        // 2nd triangle
        triangles.Add(topLeft);
        triangles.Add(bottomRight);
        triangles.Add(topRight);
    }

    public List<Tile> GetNeighboursAsList(){
        List<Tile> allNeigbours = new List<Tile>();
        allNeigbours.Add(neighbors.top);
        allNeigbours.Add(neighbors.left);
        allNeigbours.Add(neighbors.bottom);
        allNeigbours.Add(neighbors.right);
        return allNeigbours;        
    }

}

