using System.Collections.Generic;
using UnityEngine;
public class Node: MonoBehaviour {
    public Vector3 location;

    //Pathfinding stuff
    //Scores:
    //distanceFromStart (G) - Distance from the starting point, 
    //distanceToDestination  (H) - estimated distance from the destination
    //combinedDistance (F) - G + H
    //More about scores : https://www.raywenderlich.com/4946/introduction-to-a-pathfinding
    public int distanceFromStart, distanceToDestination, combinedDistance;
    public Node parent;

    public virtual List<Node> GetNeighbors() {
        return new List<Node>();
    }
    public virtual int GetDistanceFromTargetTile(Node target) {
        return (int)Vector3.Distance(location, target.location);
    }
}