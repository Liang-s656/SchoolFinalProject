using System.Collections;
using System.Collections.Generic;

public class RihaNode{
    ValueType type = ValueType.auto;
    object value;

    public RihaNode(ValueType type, object value = null){
        this.type = type;
        this.value = value;
    }

    override public string ToString() {
        return GetString();
    }

    public string GetString(){
        if(type != ValueType.array){
            return value.ToString();
        }

        string output = "";
        foreach (RihaNode node in ((List<RihaNode>)value)){
                output += node.GetString() + ",";
        }
        return output;
    }
    public object GetValue(){
        return value;
    }
    public void SetType(ValueType type){
        this.type = type;
    }
    public ValueType GetNodeType(){
        return type;
    }
    public void Add(RihaNode addNode){
        if(type == ValueType.number){
            float add = addNode.GetNodeType() == ValueType.number ? RihaNode.GetNumeric(addNode.GetValue()) : 0;
            float newValue = RihaNode.GetNumeric(value) + add;
            value = newValue.ToString();
        }else if(type == ValueType.text){
            value += addNode.GetString();
        }else if(type == ValueType.array){
            ((List<RihaNode>)value).Add(addNode);
        }
    }

    public float GetSize(){
        switch(type){
            case ValueType.number:
                return (float)GetValue();
            case ValueType.array:
                return ((List<RihaNode>)value).Count;
            case ValueType.boolean:
                return ((bool)value) == true ? 1 : 0;
            default:
                return GetString().Length;
        }
    }

    public RihaNode get(RihaNode[] parameters){
        int id = (int)(parameters[0].GetSize());
        return ((List<RihaNode>)value)[id];
    }

    private static float GetNumeric(object value){
        float n;
        bool isNumeric = float.TryParse(value.ToString(), out n);
        return isNumeric ? n : 0;
    }

    private static bool IsNumeric(object value){
        float n;
        bool isNumeric = float.TryParse(value.ToString(), out n);
        return isNumeric;
    }

    private static bool IsList(object value){
        return value is IList  && value.GetType().IsGenericType;
    }
}
