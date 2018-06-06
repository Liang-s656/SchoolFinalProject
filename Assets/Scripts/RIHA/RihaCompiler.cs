using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Reflection;
public class RihaCompiler : MonoBehaviour {

    static Dictionary<string, RihaNode> variableMemory;

	public void Execute (string command) {
        variableMemory = new Dictionary<string, RihaNode>();

        string[] commands = command.Split(new [] { '\r', '\n' });

        string commandLine = "";
        foreach(string line in commands){
            commandLine += line;
            if(!IsEOL(line)){
                continue;
            }
            List<string> actions = SplitActions(commandLine);
            ExecuteAction(actions);
            commandLine = "";
        }
	}

    List<string> SplitWords(string text){
        string[] arrayWords = text.Split(' ');
        List<string> words = arrayWords.OfType<string>().ToList();
        List<string> returnList = new List<string>();

        foreach (string word in words){
            if(word != "" && word != " " && word != null){
                returnList.Add(word);
            }
        }
        return returnList;
    }

    public static void AddToMemory(string key, RihaNode value){
        RihaCompiler.variableMemory.Add(key, value);
    }
/* 
    loop from a by b till c:

    if a equal to b:

    if a not equal to b:
    if a less than b:
    if a bigger than b
*/
    List<string> SplitActions(string text){
        string[] arreyActions = text.Split(':');
        List<string> actions = arreyActions.OfType<string>().ToList();
        return actions;
    }

    //END OF LINE
    public bool IsEOL(string line){
        List<string> words = SplitWords(line);
        line = words.Last();
        return line[line.Length - 1] != ':';
    }

    public void set (string action, string[] words, RihaNode[] previuseActionResult){
        //Patern set [varible_key] as [variable_type];
        string setPattern = @"set\s+\w+\s+as\s+\w+\s*";
        if(MatchesRegex(action, setPattern)){
            RihaNode varible = previuseActionResult[previuseActionResult.Length - 1];
            ValueType type = GetValueType(words[3]);
            varible.SetType(type);
            variableMemory.Add(words[1], varible);
        }
    }

    public void print (string action, string[] words, RihaNode[] previuseActionResult) {
        string printPattern = @"print\s*";
        if(MatchesRegex(action, printPattern)){
            RihaNode node = previuseActionResult[previuseActionResult.Length - 1];
            string value = node.GetString();
            Debug.Log(value);
        }
    }

    public void size (string action, string[] words, RihaNode[] previuseActionResult) {
        string printPattern = @"size\s+of\s*";
        if(MatchesRegex(action, printPattern)){
            RihaNode node = previuseActionResult[previuseActionResult.Length - 1];
            Debug.Log(node.GetSize());
        }
    }

    public void add (string action, string[] words, RihaNode[] previuseActionResult) {
        string addPattern = @"add\s+to\s+\w+\s*";
        if(MatchesRegex(action, addPattern)){
            string variableKey = words[2];
            if(variableMemory.ContainsKey(variableKey)){
                RihaNode varibale = variableMemory[variableKey];
                varibale.Add(previuseActionResult[previuseActionResult.Length - 1]);
            }
        }
    }

    private bool MatchesRegex(string text, string patern){
        Regex r = new Regex(patern, RegexOptions.IgnoreCase);
        Match m = r.Match(text);
        return m.Success;
    }

    void ExecuteAction (List<string> actions){
        actions.Reverse();
        List<RihaNode> finishedActionsReturns = new List<RihaNode>();
        foreach(string action in actions){
            string[] words = SplitWords(action).ToArray();
            object isAction =  EvaluateAction(action, words);
            if(isAction == null){
                RihaNode functionNode = IsFunction(action, words);
                if(functionNode != null){
                    string[] functions = words[0].Split('.');
                    MethodInfo functionMethod = functionNode.GetType().GetMethod(functions[1].ToLower());
                    if(functionMethod != null){
                        object[] parameters = new object[]{
                            finishedActionsReturns.ToArray(),
                        };
                        RihaNode returnValue = (RihaNode)functionMethod.Invoke(functionNode, parameters);
                        finishedActionsReturns.Add(returnValue);
                    }
                }else{
                    MethodInfo actionMethod = this.GetType().GetMethod(words[0].ToLower());
                    if(actionMethod != null){
                        object[] parameters = new object[]{
                            action,
                            words, 
                            finishedActionsReturns.ToArray() 
                        };
                        actionMethod.Invoke(this, parameters);
                    }
                }
            }else{
                finishedActionsReturns.Add(isAction == null ? null : (RihaNode)isAction);
            }
        }
    }


    RihaNode IsFunction(string action, string[] words){
        string[] functions = words[0].Split('.');
        if(functions.Length > 0){
            if( variableMemory.ContainsKey(functions[0]) ){
                return variableMemory[functions[0]];
            }
        }
        return null;
    }

    object EvaluateAction(string action, string[] words){
        // Is number:
        float n;
        bool isNumeric = float.TryParse(action, out n);
        if(isNumeric){
            return new RihaNode(ValueType.number, n);
        }

        //Is boolean
        if(words.Length == 1){
            if(words[0].ToLower() == "true"){
                return new RihaNode(ValueType.boolean, true);
            }else if(words[0].ToLower() == "false"){
                return new RihaNode(ValueType.boolean, false);
            }
        }

        //Is varibale
        if(variableMemory.ContainsKey(words[0])){
            return variableMemory[words[0]];
        }

        //Is array
        string arrayPattern = @"\[.*\]";
        List<string> arrayGroups = GroupMatch(action, arrayPattern);
        if(arrayGroups != null){
            string arrayless = arrayGroups[0].Substring(1, arrayGroups[0].Length - 2);
            string[] elements = arrayless.Split(',');
            List<RihaNode> array = new List<RihaNode>();
            foreach(string element in elements){
                string[] elementWords = SplitWords(element).ToArray();
                object value = EvaluateAction(element, elementWords);
                RihaNode variable = new RihaNode( ValueType.auto, value);
                array.Add(variable);
            }
            return new RihaNode(ValueType.array, array);
        }

        //Is text:
        string textPattern = @"\"".*?\""";
        List<string> textGroups = GroupMatch(action, textPattern);
        if(textGroups != null){
            string qouteless = textGroups[0].Substring(1, textGroups[0].Length - 2);
            return new RihaNode(ValueType.text, qouteless);
        }


        return null;
    }

    List<string> GroupMatch(string text, string pattern){
        Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
        Match m = r.Match(text);
        if(m.Groups.Count > 0 && m.Groups[0].Value != ""){
            List<string> groups = new List<string>();
            for(int i = 0; i < m.Groups.Count; i++){
                groups.Add(m.Groups[i].Value);
            }
            return groups;
        }
        return null;
    }

    ValueType GetValueType(string typeName){
        ValueType type = (ValueType) Enum.Parse(typeof(ValueType), typeName); 
        return type;
    }

}
