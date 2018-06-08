using UnityEngine;
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
            Debug.Log(tileLocation.GetNodeType().ToString());
            float x = (float)tileLocation.get(getZero).GetValue();
            float z = (float)tileLocation.get(getOne).GetValue();    
                    
            Tile t = GameController.island.GetTileByWorldCoords(x, z);
            GameController.playerController.SetBuilding(building);
            GameController.playerController.Build(t);
            return new RihaNode(ValueType.boolean, true);
        }
        return new RihaNode(ValueType.boolean, false);
    }
}
