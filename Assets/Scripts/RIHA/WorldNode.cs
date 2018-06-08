using UnityEngine;
using System.Collections.Generic;
public class WorldNode
{
    public RihaNode freetile(RihaNode[] parameters){
        Tile tile = GameController.island.GetRandomFreeTile();
        return new RihaNode(ValueType.array, new List<RihaNode>(){
            new RihaNode(ValueType.number, tile.location.x),
            new RihaNode(ValueType.number, tile.location.z)            
        });
    }
}
