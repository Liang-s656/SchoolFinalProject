using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Node[,] grid;
    
    public void ClearGrid(int gridSize) {
        grid = new Node[gridSize, gridSize];
    }

    public List<Vector3> GetPath(Node from, Node to) {
        //if(to.IsMovable() == false || from.IsMovable() == false) return null;

        Node current = null;
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();        
        int G = 0;
        int memmoryFix = 0;

        openList.Add(from);

        while (openList.Count > 0){
            memmoryFix++;
            if(memmoryFix > 200){
                return null;
            }

            //Retrieves the tile with lowest F score
            int lowest = openList.Min(l => l.combinedDistance);
            current = openList.First(l => l.combinedDistance == lowest);
 
            //Add the current square to the closed list
            closedList.Add(current);

            //Remove it from open list
            openList.Remove(current);  

            //If we added the destination to the closed list, we've found the path!
            if(closedList.FirstOrDefault(l => l.location.x == to.location.x && l.location.z == to.location.z) != null){
               Debug.Log("We found the destination!");
               List<Vector3> path = new List<Vector3>();
               while(current != null){
                   Vector3 curr = current.location;
                    if( path.Count > 1){
                        Vector3 prev = path[path.Count - 1];                        
                        if(curr.y > prev.y){
                            path.Add(new Vector3(prev.x, curr.y, prev.z));
                        }else if(curr.y < prev.y){
                            path.Add(new Vector3(curr.x, prev.y, curr.z));                            
                        }
                    }
                    path.Add(curr);
                    Node tmp = current.parent;
                    current.parent = null;
                    current = tmp;
                }
                return path;
            }

            List<Node> adjacentTiles = current.GetNeighbors();
            G++;

            foreach(Node adjacentTile in adjacentTiles){
                //If adjacent square is already in the closest list, ignore it
                if(closedList.FirstOrDefault(l => l.location.x == adjacentTile.location.x && l.location.z == adjacentTile.location.z) != null){
                    continue;
                }

                int g = adjacentTile.GetDistanceFromTargetTile(from) + G;
                //If it's not in the open list...
                if(openList.FirstOrDefault(l => l.location.x == adjacentTile.location.x && l.location.z == adjacentTile.location.z) == null){
                    //Compute score & set perant
                    adjacentTile.distanceFromStart = g;
                    //adjacentTile.distanceFromStart = adjacentTile.getDistanceFromTargetTile(from);
                    adjacentTile.distanceToDestination = adjacentTile.GetDistanceFromTargetTile(to);
                    adjacentTile.combinedDistance = adjacentTile.distanceFromStart + adjacentTile.distanceToDestination;
                    adjacentTile.parent = current;

                    openList.Insert(0, adjacentTile);
                }else{
                    if( g + adjacentTile.distanceToDestination < adjacentTile.combinedDistance ){
                        adjacentTile.distanceFromStart = g;
                        adjacentTile.combinedDistance = adjacentTile.distanceFromStart + adjacentTile.distanceToDestination;
                        adjacentTile.parent = current;
                    }
                }
            }
        }
        return null;
    }
}
