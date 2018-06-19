using UnityEngine;
using System.Collections.Generic;

public class PlayerNode
{
    public RihaNode place(RihaNode[] parameters){
        
        RihaNode par = parameters[parameters.Length - 1];

        RihaNode[] getZero = new RihaNode[]{
            new RihaNode(ValueType.number, 0)
        };
        RihaNode[] getOne = new RihaNode[]{
            new RihaNode(ValueType.number, 1)
        };

        string name = par.get(getZero).GetString();

        RihaNode tileLocation = par.get(getOne);        

        Buildable building = GameData.GetBuilding(name);
        
        if(building != null){
            float x = (float)tileLocation.get(getZero).GetValue();
            float z = (float)tileLocation.get(getOne).GetValue();    
                    
            Tile t = GameController.island.GetTileByWorldCoords(x, z);
            GameController.playerController.SetBuilding(building);
            GameController.playerController.Build(t);
            return new RihaNode(ValueType.boolean, true);
        }
        return new RihaNode(ValueType.boolean, false);
    }

    public RihaNode get_selected_object(RihaNode[] parameters){
        if(GameController.playerController.selectedGO != null){
            GameObject selected = GameController.playerController.selectedGO;
            return new RihaNode(ValueType.array, new List<RihaNode>(){
                new RihaNode(ValueType.number, selected.transform.position.x),
                new RihaNode(ValueType.number, selected.transform.position.z)            
            });
        }
        return new RihaNode(ValueType.boolean, false);
    }

    public RihaNode look_at(RihaNode[] parameters){
        List<RihaNode> par = ((List<RihaNode>)parameters[0].GetValue());
        GameController.playerController.transform.position = new Vector3(
            par[0].GetSize(),
            GameController.playerController.transform.position.y,
            par[1].GetSize()
        );
        return new RihaNode(ValueType.boolean, true);
    }
}
