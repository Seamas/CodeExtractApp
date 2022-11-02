namespace CodeExtractTool.Comment;

public class MultiComment : IRegexComment
{
    public int Order => 1;
    public string Name => "多行注释/* */";
    public string Value => @"/\*[\s\S]*?\*/";
}