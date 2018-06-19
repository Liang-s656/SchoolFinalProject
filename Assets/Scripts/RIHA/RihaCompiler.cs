using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.PostProcessing;
public class RihaCompiler : MonoBehaviour {

    static Dictionary<string, RihaNode> variableMemory;
    static Dictionary<string, RihaNode> globals;

    static List<RihaScope> scopes;

    public GUISkin consoleSkin;
    public bool showMenu;
    string codeText, codeOutput;
    float left = -400, sideOffset = 400;

    bool filesShow = false;

    Dictionary<string, string> files;
    string activeFile = "main";


    string[][] keyWords = new string[][] {
        new string[]{"set", "scope", "end", "print", "scope", "as", "size", "add", "to", "of", "import"},
        new string[]{"number", "text", "boolean", "GLOBAL", "loop", "check", "function", "gameobject", "auto", "array"},                
        new string[]{"get", "equal", "equal_type", "bigger_than", "less_than"},                
    };

    void Start()
    {
        files = new Dictionary<string, string>();
        files.Add("main", "");
        files.Add("basic", "");
        PopulateGlobals();

        InvokeRepeating("Updater", 1, 0.25f);
    }

    void Updater()
    {
        if(showMenu == false && Time.timeScale >= 1){
            if(!String.IsNullOrEmpty(files["main"])){
                Execute(files["main"]);
            }
        }
    }

    public int lineNumber;
    void OnGUI(){
        GUI.skin = consoleSkin;

        int lineCount = 1;

        string formatedCode =  files[activeFile];        
        if(!string.IsNullOrEmpty( files[activeFile])){
            lineCount = files[activeFile].Split(new [] { '\r', '\n' }).Length;
            string before = "<b><color=white>";
            string after = "</color></b>";
            for (int i = 0; i < keyWords.Length; i++){
                if(i == 1){
                    before = "<i><color=#fff600>";
                    after = "</color></i>";
                }else if(i == 2){
                    before = "<color=#0DECCF>";
                    after = "</color>";
                }
                for (int w = 0; w < keyWords[i].Length; w++){
                    string pattern = @"\b"+keyWords[i][w]+@"\b";
                    string replace = before + keyWords[i][w] + after;
                    formatedCode = Regex.Replace(formatedCode, pattern, replace);
                }
            }

            formatedCode = Regex.Replace(formatedCode, @"\s*\/\/.*", m => "<color=#777777><i>" + m.Groups[0].Value + "</i></color>" );
        }

        GUI.BeginGroup(new Rect(left, 0, sideOffset, Screen.height));
        GUI.Box( new Rect(0, 0, 43, Screen.height), "");
        if(GUI.Button( new Rect(0, Screen.height - 43, 43, 43), "./>")){
            codeOutput = "";
            Execute(files[activeFile]);
        }
        /*if(GUI.Button( new Rect(0, Screen.height - 86, 43, 43), "", "handbook")){
            
        }*/
        filesShow = GUI.Toggle( new Rect(0, 0, 43, 43), filesShow, "", "files");
        if(filesShow){
            GUI.Box(new Rect(43, 0, 200, Screen.height), "", "file-explorer");
            GUI.Label(new Rect(43, 0, 200, 43), "EXPLORER", "file-explorer-title");
            GUI.BeginGroup(new Rect(43, 43, 200, Screen.height-43));
            foreach(KeyValuePair<string, string> file in files){
                if(GUILayout.Button(file.Key + ".rih", (activeFile == file.Key ? "file-active" : "file"), GUILayout.Width(250))){
                    activeFile = file.Key;
                }
            }
            GUI.EndGroup();
        }
        
        GUI.BeginGroup(new Rect(sideOffset - 357, 0, 357, Screen.height));
            GUI.Box( new Rect(0, 0, 357, Screen.height - 250), formatedCode, "output-code");
            files[activeFile] = GUI.TextArea( new Rect(0, 0, 357, Screen.height - 250), files[activeFile]);
            for(int i = 1; i <= lineCount; i++){
                string output = (i < 10 ? "0" + i : i.ToString());
                GUI.Label(new Rect(10, 10 + (i - 1) * 16, 30, 20), output);
            }
            GUI.Box( new Rect(0, Screen.height - 250, 357, 250), codeOutput, "compiled-output");
        GUI.EndGroup();

        GUI.EndGroup();

        //GUI.Box(new Rect(left, Screen.height - 310, 400, 260), codeOutput);

        /*if(GUI.Button(new Rect(left, Screen.height - 50, 400, 30), "run")){
            codeOutput = "";
            Execute(codeText);
        }*/

        AnimateTerminal();
    }

    void AnimateTerminal(){
        sideOffset = filesShow ? 600 : 400;
        if(showMenu == true){
           if(left < 0){
               left += Time.deltaTime * 700;
           }else{
               left = 0;
           }
        }else{
            if(left > -sideOffset){
               left -= Time.deltaTime * 700;
           }else{
               left = -sideOffset;
           }
        }
    }


    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q)){
            showMenu = !showMenu;
            Camera.main.GetComponent<PostProcessingBehaviour>().enabled = showMenu;
            GameController.playerController.GetComponent<Debuger>().enabled = showMenu;
        }
    }

    private void PopulateGlobals(){
        globals = new Dictionary<string, RihaNode>();
        globals.Add("WORLD", new RihaNode(ValueType.GLOBAL, new WorldNode()));
        globals.Add("PLAYER", new RihaNode(ValueType.GLOBAL, new PlayerNode()));
    }

	public void Execute (string command) {
        codeOutput = "";
        command.Replace("<br>", "\n");

        
    try{
        variableMemory = new Dictionary<string, RihaNode>();
        scopes = new List<RihaScope>();

        string[] commands = command.Split(new [] { '\r', '\n' });

        string commandLine = "";

        Resource requiredEnergy = new Resource(){ resourceID = 2, amount = commands.Length * 5};
        if(!PlayerData.HasEnoughResource(requiredEnergy)){
            codeOutput = "NOT ENOUGH ENERGY!\nREQUIRED: " + requiredEnergy.amount;
            return;
        }
        PlayerData.RemoveResource(requiredEnergy);
        for( lineNumber = 0; lineNumber < commands.Length; lineNumber++) {
            string line = commands[lineNumber];

            if(scopes.Count > 0){
                RihaScope acctiveScope = scopes.Last();
                if(!MatchesRegex(line, @"\s*end\s+scope\s*")){
                    // Check
                    if(acctiveScope.type == ScopeType.check){
                        if(!(bool)acctiveScope.parameter.GetValue()){
                            continue;
                        }
                    }
                    // Loop
                    else if (acctiveScope.type == ScopeType.loop){
                        if(acctiveScope.parameter.GetSize() < 1){
                            continue;
                        }
                    }
                }
            }

            if(MatchesRegex(line, @"\s*\/\/.*"))
                continue;

            commandLine += line;
            if(!IsEOL(line)){
                continue;
            }
            List<string> actions = SplitActions(commandLine);
            ExecuteAction(actions);
            commandLine = "";
        }
    } catch (Exception e){
            codeOutput += "\n<color=red><b>ERROR OCCURRED:</b></color>" + e.Message;
            Debug.LogError(e);
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
        if(!String.IsNullOrEmpty(line) && words.Count() > 0){
            line = words.Last();
            return line[line.Length - 1] != ':';
        }
        return false;
    }

    public void set (string action, string[] words, RihaNode[] previuseActionResult){
        //Patern set [varible_key] as [variable_type];
        string setPattern = @"\s*set\s+\w+\s+as\s+\w+\s*";
        if(MatchesRegex(action, setPattern)){
            RihaNode varible = previuseActionResult[previuseActionResult.Length - 1];
            ValueType type = GetValueType(words[3]);
            varible.SetType(type);
            variableMemory.Add(words[1], varible);
        }
    }
    public void import (string action, string[] words, RihaNode[] previuseActionResult){
        //Patern set [varible_key] as [variable_type];
        string setPattern = @"\s*import\s*";
        if(MatchesRegex(action, setPattern)){
            RihaNode varible = previuseActionResult[previuseActionResult.Length - 1];
        }
    }

    public void scope (string action, string[] words, RihaNode[] previuseActionResult){
        string setPattern = @"\s*scope\s+\w+\s*";
        if(MatchesRegex(action, setPattern)){
            RihaNode value = previuseActionResult[previuseActionResult.Length - 1];
            ScopeType type = GetScopeType(words[1]);
            if(type == ScopeType.check && value.GetNodeType() == ValueType.boolean || type == ScopeType.loop && value.GetNodeType() == ValueType.number){
                scopes.Add(new RihaScope(){
                    type = type,
                    parameter = value,
                    startLine = lineNumber
                });
            }
        }
    }

    public void end (string action, string[] words, RihaNode[] previuseActionResult){
        string setPattern = @"\s*end\s+scope\s*";
        if(MatchesRegex(action, setPattern)){
            if(scopes.Count > 0){
                RihaScope acctiveScope = scopes.Last();
                if(acctiveScope.type == ScopeType.loop && acctiveScope.iteration < acctiveScope.parameter.GetSize() - 1){
                    acctiveScope.iteration++;
                    acctiveScope.endLine = lineNumber;
                    lineNumber = acctiveScope.startLine;
                }else{
                        scopes.Remove(acctiveScope);
                }
            }

        }
    }

    public void print (string action, string[] words, RihaNode[] previuseActionResult) {
        string printPattern = @"\s*print\s*";
        if(MatchesRegex(action, printPattern)){
            RihaNode node = previuseActionResult[previuseActionResult.Length - 1];
            string value = node.GetString();
            codeOutput += value + "\n";
        }
    }

    public void size (string action, string[] words, RihaNode[] previuseActionResult) {
        string printPattern = @"\s*size\s+of\s*";
        if(MatchesRegex(action, printPattern)){
            RihaNode node = previuseActionResult[previuseActionResult.Length - 1];
            codeOutput += node.GetSize() + "\n";
            Debug.Log(node.GetSize());
        }
    }

    public void add (string action, string[] words, RihaNode[] previuseActionResult) {
        string addPattern = @"\s*add\s+to\s+\w+\s*";
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
            object isAction =  EvaluateAction(action, words, finishedActionsReturns);
            if(isAction == null){
                RihaNode functionNode = IsFunction(action, words);
                if(functionNode != null){
                    
                    string[] functions = words[0].Split('.');
                    string methodName = (functionNode.GetNodeType() == ValueType.GLOBAL) ? "GlobalCall" : functions[1].ToLower();
                    MethodInfo functionMethod = functionNode.GetType().GetMethod(methodName);
                    if(functionMethod != null){
                        object[] parameters;
                        if((functionNode.GetNodeType() == ValueType.GLOBAL)){
                            parameters = new object[]{
                                functions[1].ToLower(),
                                finishedActionsReturns.ToArray(),
                            };
                        }else{
                            parameters = new object[]{
                                finishedActionsReturns.ToArray(),
                            };
                        }
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

    object EvaluateAction(string action, string[] words, List<RihaNode> pevActions){
        // Is number:
        float n;
        bool isNumeric = float.TryParse(action, out n);
        if(isNumeric){
            return new RihaNode(ValueType.number, n);
        }


        if(words.Length == 1){
            if(words[0] == "GLOBAL"){
                if(globals.ContainsKey(pevActions.Last().GetString())){
                    return globals[pevActions.Last().GetString()];
                }
            }
            //Is boolean            
            else if(words[0].ToLower() == "true"){
                return new RihaNode(ValueType.boolean, true);
            }else if(words[0].ToLower() == "false"){
                return new RihaNode(ValueType.boolean, false);
            }else if(words[0].ToLower() == "null"){
                return new RihaNode(ValueType.tmp, null);
            }
            // NULL
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
                RihaNode variable = (RihaNode)EvaluateAction(element, elementWords, null);
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

    ScopeType GetScopeType(string scopeName){
        ScopeType type = (ScopeType) Enum.Parse(typeof(ScopeType), scopeName); 
        return type;
    }

}
