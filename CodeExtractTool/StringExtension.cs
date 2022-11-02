using System.Text.RegularExpressions;
using CodeExtractTool.Comment;

namespace CodeExtractTool;

public static class StringExtension
{

    public static string RemoveComment(this string text, IEnumerable<IRegexComment> enumerable)
    {
        foreach (var comment in enumerable)
        {
            text = text.RemoveComment(comment);
        }

        return text;
    }

    public static string RemoveComment(this string text, IRegexComment comment)
    {
        return Regex.Replace(text, comment.Value, "", RegexOptions.Multiline);
    }
    
    public static string RemoveComment(this string text)
    {
        // 多行注释 /* */
        text = Regex.Replace(text, @"/\*[\s\S]*?\*/", "", RegexOptions.Multiline);
        // html的注释 <!--  -->
        text = Regex.Replace(text, @"<!--[\s\S]*?-->", "", RegexOptions.Multiline);
        // 单行注释 //
        text = Regex.Replace(text, @"//[\s\S]*?$", "", RegexOptions.Multiline);
        // 删除空格和tab组成的空行
        text = Regex.Replace(text, @"^[\s\t]*$\n", "", RegexOptions.Multiline);
        return text;
    }
}