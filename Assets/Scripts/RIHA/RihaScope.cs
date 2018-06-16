public enum ScopeType{
    loop, check
}
public class RihaScope
{
    public ScopeType type;
    public bool isOpen = true;

    public RihaNode parameter;
    public int iteration = 0;
    
    public int startLine = 0;
    public int endLine = 0;

    
}
