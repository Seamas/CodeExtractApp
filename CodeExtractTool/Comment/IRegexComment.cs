namespace CodeExtractTool.Comment;

public interface IRegexComment
{
    public int Order { get; }
    public string Name { get; }
    public string Value { get; }
}