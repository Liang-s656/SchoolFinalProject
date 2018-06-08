[System.Serializable]
public class Buildable{
    public string name;
    public string description;
    public string type;
    public string prefabPath;

    public bool isRotatable;
    public bool requiresBlueprint;

    public Resource[] requiredResources;

    public string GetDescription(){
        string lines = 
            "<color=#13fade><b>description:</b></color> " + description + "\n" +
            "<color=#13fade><b>type:</b></color> " + type + "\n" +
            "<color=#13fade><b>requires:[</b></color> " + "\n" 
        ;
        foreach(Resource res in requiredResources){
            lines += "   <color=#13fade><b>" + GameData.GetResourceName(res.resourceID) + "</b></color> " + ":" + res.amount + "\n";
        }  
        lines += "<color=#13fade><b>]</b></color>";
        return lines;
    }
}
