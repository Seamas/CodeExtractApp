using System.Reflection;
using CodeExtractTool.Comment;

namespace CodeExtractTool;

public class CodeExtract
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">输入目录</param>
    /// <param name="output">输出文件名称</param>
    /// <param name="regexComments">需要移除的注释</param>
    /// <param name="extension">扩展名, 默认 *.java</param>
    /// <param name="limit">行数, 3000</param>
    public static void Extract(string path, string output, IEnumerable<IRegexComment>? regexComments, string extension = "*.java", int limit = 3000)
    {
        if (regexComments == null)
        {
            var list = new List<IRegexComment>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var comments = assembly.GetTypes()
                    .Where(item => item.IsClass && item.IsAssignableTo(typeof(IRegexComment)))
                    .Select(item => Activator.CreateInstance(item) as IRegexComment)
                    .ToList();
                list.AddRange(comments!);
            }

            regexComments = list;
        }

        var regexes = regexComments.Where(item => item != null)
            .OrderBy(item => item!.Order)
            .ToList();


        var directory = new DirectoryInfo(path);
        var fileInfos = directory.GetFiles(extension, SearchOption.AllDirectories);

        var index = 0;
        using var sw = File.AppendText(output);
        foreach (var fileInfo in fileInfos)
        {
            var text = File.ReadAllText(fileInfo.FullName).RemoveComment(regexes!);
            var strings = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strings)
            {
                sw.WriteLine(str);
                index++;
            }

            if (index >= limit)
            {
                return;
            }
        }
    }
}