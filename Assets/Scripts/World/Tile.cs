using System.Collections.Generic;
using UnityEngine;

public class Tile: Node {
    public TileStructure tileStructure;
    public GameObject building;
    public bool containsTree;

    public Tile (Vector3 location, TileStructure structure = null) {
        tileStructure = (structure != null) ? structure : new TileStructure();
        this.location = structure.location = location;
    }
    private bool IsClimbable() {
        return false;
    }

    private bool IsMovable() {
        return true;
    }

    public override List<Node> GetNeighbors() {
        List<Node> neighbors = new List<Node>();
        foreach (Tile tile in tileStructure.neighbors.ToArray())
        {
            bool tileIsClimbable = tile.location.y > location.y && tile.IsClimbable();
            bool iAmClimbable = tile.location.y < location.y && IsClimbable();
            bool isClimbable = tileIsClimbable|| iAmClimbable;

            bool isMovable = tile.IsMovable() && tile.location.y == location.y;

            if(!isMovable || !isClimbable)
                continue;

            neighbors.Add(tile);
        }
        return neighbors;
    }
    public void SetNeighbor(Tile neighbor, string dir){
        switch(dir.ToUpper()){
            case "LEFT":
                tileStructure.neighbors.left = neighbor;
                return;
            case "RIGHT":
                tileStructure.neighbors.right = neighbor;
                return;
            case "TOP":
                tileStructure.neighbors.top = neighbor;
                return;
            case "BOTTOM":
                tileStructure.neighbors.bottom = neighbor;
                return;
        }
        Debug.LogError("Unknown direction use - LEFT, RIGHT, TOP, BOTTOM");
    }

    public override int GetDistanceFromTargetTile(Node target) {
        if(target == null) return 99999;
        return base.GetDistanceFromTargetTile(target);
    }


    // Tree optimization
    private bool NeighborHasLinkedTree(Tile neighbor) {
        return neighbor != null && neighbor.containsTree && neighbor.location.y == location.y ;
    }
    private int getTreeSum() {
        int sum = 0;
        if( NeighborHasLinkedTree(tileStructure.neighbors.top) ) sum += 1;
        if( NeighborHasLinkedTree(tileStructure.neighbors.right) ) sum += 2;
        if( NeighborHasLinkedTree(tileStructure.neighbors.bottom) ) sum += 4;
        if( NeighborHasLinkedTree(tileStructure.neighbors.left) ) sum += 8;        
        return sum;
    }

    public int getTreeType() {
        int sum = getTreeSum();
        if ( sum == 0 ){
            return 0;
        } else if ( sum == 15 ) {
            return 5;
        } else if ( sum % 3 == 0 ){
            return 2;            
        } else if ( sum % 5 == 0 ){
            return 3;
        }else if ( sum == 1 || sum == 2 || sum == 4 || sum == 8 ) {
            return 1;            
        }else {
            return 4;
        }
    }

    public int getTreeRotation() {
        int sum = getTreeSum();
        if (sum == 4 || sum == 5 || sum == 7 || sum == 6){
            return 90;
        }else if (sum == 2 || sum == 11 || sum == 3){
            return 180;
        }else if (sum == 1 || sum == 13 || sum == 9){
            return 270;
        }else{
            return 0;
        }
    }

    // TOP - 1
    // RIGHT - 2
    // BOTTOM - 4
    // LEFT - 8
    // 1, 2, 4, 8 - 1 SIDE HIDDEN
    // 3, 9, 6, 12 - 2 CLOSE HIDDEN
    // 5, 10 - 2 DIFF HIDDEN
    // 7, 14, 13, 11 - 3 SIDES HIDDEN
    // 15 - ALL SIDES HIDDEN

}
