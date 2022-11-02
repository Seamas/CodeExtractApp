namespace CodeExtractTool.Comment;

public class EmptyLineComment : IRegexComment
{
    public int Order => Int32.MaxValue;
    public string Name => "空行注释";
    public string Value => @"^[\s\t]*$\n";
}