[System.Serializable]
public class Buildable{
    public string name;
    public string description;
    public string type;
    public string prefabPath;

    public bool isRotatable;
    public bool requiresBlueprint;

    public Buildable[] upgrades;
}
