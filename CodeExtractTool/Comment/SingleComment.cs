namespace CodeExtractTool.Comment;

public class SingleComment : IRegexComment
{
    public int Order => 2;
    public string Name => "单行注释 //";
    public string Value => @"//[\s\S]*?$";
}