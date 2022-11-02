namespace CodeExtractTool.Comment;

public class HtmlComment : IRegexComment
{
    public int Order => 1;
    public string Name => "html的注释 <!--  -->";
    public string Value => @"<!--[\s\S]*?-->";
}