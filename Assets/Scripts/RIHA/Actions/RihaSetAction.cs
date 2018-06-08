using System.Collections;
using System.Collections.Generic;
using System;

public class RihaSetAction: RihaAction {
    override protected string GetPattern() {
        return @"set\s+\w+\s+as\s+\w+\s*";
    }

    override protected void Execute() {
        RihaNode variable = this.parameters[0];
        ValueType type = GetValueType(words[3]);
        variable.SetType(type);

        RihaCompiler.AddToMemory(words[1], variable);
    }

    private ValueType GetValueType(string typeName){
        ValueType type = (ValueType) Enum.Parse(typeof(ValueType), typeName); 
        return type;
    }
}
