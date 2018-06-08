using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RihaAction {

    protected string action;
    protected string[] words;
    protected List<RihaNode> parameters = new List<RihaNode>();


    public void __call(string action, string[] words, RihaNode[] parameters){
        if(MatchesPatter(action)){
            this.action = action;
            this.words = words;
            foreach(RihaNode parameter in parameters){
                this.parameters.Add(parameter);
            }
            Execute();
        }
    }

    virtual protected void Execute() {

    }

    virtual protected string GetPattern() {
        return "";
    }

    protected bool MatchesPatter(string text) {
        string pattern = GetPattern();
        Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
        Match m = r.Match(text);
        return m.Success;
    }

}
