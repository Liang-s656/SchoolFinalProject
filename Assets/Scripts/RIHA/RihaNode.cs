using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
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
                return float.Parse(GetValue().ToString());
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
    public RihaNode set(RihaNode[] parameters){
        RihaNode p = parameters[parameters.Length - 1];
        this.value = p.value;
        this.type = p.GetNodeType();
        return this;
    }
    public RihaNode equal(RihaNode[] parameters){
        bool ret = false;
        RihaNode comperisonNode = parameters[parameters.Length - 1];
        if(comperisonNode.GetNodeType() == GetNodeType()){
            if(comperisonNode.GetString() == GetString()){
                ret = true;
            }
        }
        return new RihaNode(ValueType.boolean, ret);
    }

    public RihaNode equal_type(RihaNode[] parameters){
        bool ret = false;
        RihaNode comperisonNode = parameters[parameters.Length - 1];
        if(comperisonNode.GetNodeType() == GetNodeType()){
            ret = true;
        }
        return new RihaNode(ValueType.boolean, ret);
    }

    public RihaNode bigger_than(RihaNode[] parameters){
        bool ret = false;
        RihaNode comperisonNode = parameters[parameters.Length - 1];
        if(comperisonNode.GetSize() < GetSize()){
            ret = true;
        }
        return new RihaNode(ValueType.boolean, ret);
    }

    public RihaNode less_than(RihaNode[] parameters){
        bool ret = false;
        RihaNode comperisonNode = parameters[parameters.Length - 1];
        if(comperisonNode.GetSize() > GetSize()){
            ret = true;
        }
        Debug.Log(ret);
        return new RihaNode(ValueType.boolean, ret);
    }

    public RihaNode GlobalCall(string method, RihaNode[] par){
        MethodInfo functionMethod = value.GetType().GetMethod(method);
        if(functionMethod != null){
            object[] parameters = new object[]{ par };
            RihaNode returnValue = (RihaNode)functionMethod.Invoke(value, parameters);
            return returnValue;
        }
        return null;
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
