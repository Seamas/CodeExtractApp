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
    public static async Task ExtractSingleFileAsync(string path, string output, IEnumerable<IRegexComment> regexComments, string extension, int limit = 3000)
    {
        
        var regexes = regexComments
            .OrderBy(item => item!.Order)
            .ToList();
        
        var directory = new DirectoryInfo(path);
        var fileInfos = directory.GetFiles("*" + extension, SearchOption.AllDirectories);

        var index = 0;
        await using var sw = File.AppendText(output);
        
        foreach (var fileInfo in fileInfos)
        {
            var originText = await File.ReadAllTextAsync(fileInfo.FullName);
            var text = originText.RemoveComment(regexes);
            
            var strings = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strings)
            {
                await sw.WriteLineAsync(str);
                index++;
            }

            // 确保整个文件写完, 再跳出循环, 可以保证源代码中方法是完整的
            if (index >= limit)
            {
                return;
            }
        }
    }


    public static async Task ExtractMultipleFilesAsync(string input, string output, IEnumerable<IRegexComment> regexComments, string extension)
    {
        var regexes = regexComments
            .OrderBy(item => item.Order)
            .ToList();
        
        var directory = new DirectoryInfo(input);
        var fileInfos = directory.GetFiles("*" + extension, SearchOption.AllDirectories);
        foreach (var fileInfo in fileInfos)
        {
            var newFilePath = Path.Combine(output, fileInfo.Name);

            var directoryName = Path.GetDirectoryName(newFilePath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var originText = await File.ReadAllTextAsync(fileInfo.FullName);
            var text = originText.RemoveComment(regexes);
            await File.WriteAllTextAsync(newFilePath, text);
        }
    }
}