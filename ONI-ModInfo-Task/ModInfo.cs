using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using YamlDotNet.Serialization;
using BaseTask = Microsoft.Build.Utilities.Task;

namespace ONI_ModInfo_Task
{
    public class ModInfo : BaseTask
    {
        private readonly ISerializer _serializer = new SerializerBuilder()
            .Build();

        // 输入的文件目录
        [Required] public string InputFilePath { get; set; }

        // 输出的文件目录
        [Required] public string OutputFilePath { get; set; }

        // 是否启用归档格式，当SupportedContent的内容存在多项时，该值自动为true
        public bool UseArchivedVersions { get; set; }

        // 当前支持的版本，不设置的话，默认为本体模式
        public string SupportedContent { get; set; } = "vanilla_id";

        // 当前支持的版本号，不设置的话，默认值为407446，即九宫格输入法里的i sign。
        public int LastWorkingBuild { get; set; } = 407446;

        // 临时文件夹，任务结束后，这个文件夹应该不存在
        private readonly string _tempWorkSpace = Path.Combine(
            Path.GetTempPath(),
            "undancer-oni-" + Path.GetRandomFileName()
        );

        public override bool Execute()
        {
            // 获取参数
            var supportedContentList = SupportedContent.Split(',', ';', '|');
            UseArchivedVersions = supportedContentList.Length > 1 || UseArchivedVersions;
            var lastWorkingBuild = LastWorkingBuild;

            // 创建临时文件夹
            Directory.CreateDirectory(_tempWorkSpace);

            Log.LogMessage($"InputFilePath {InputFilePath}");
            Log.LogMessage($"OutputFilePath {OutputFilePath}");
            Log.LogMessage($"UseArchivedVersions {UseArchivedVersions}");
            // 生成特定的目录结构
            foreach (var supportedContent in supportedContentList)
            {
                var targetDirectoryName = string
                    .Join("_", supportedContent, lastWorkingBuild)
                    .ToLower();
                var targetDirectory = Path.Combine(
                    new string[0]
                        .Concat(
                            new[]
                            {
                                _tempWorkSpace
                            }
                        )
                        .Concat(
                            new[]
                            {
                                "archived_versions",
                                targetDirectoryName
                            }.Where(input => UseArchivedVersions)
                        ).ToArray()
                );


                // 创建目标文件夹
                Directory.CreateDirectory(targetDirectory);
                Log.LogMessage($"TargetDirectory {targetDirectory}");
                // 创建mod_info.yaml文件
                using (var writer = new StreamWriter(Path.Combine(targetDirectory, "mod_info.yaml")))
                {
                    _serializer.Serialize(writer,
                        ImmutableDictionary.Create<string, object>()
                            .Add(nameof(supportedContent), supportedContent)
                            .Add(nameof(lastWorkingBuild), lastWorkingBuild)
                    );
                }

                // 复制dll到临时目录结构中
                CopyFiles(InputFilePath, targetDirectory, OutputFilePath);
            }

            // 从临时目录复制到输出目录
            CopyFiles(_tempWorkSpace, OutputFilePath);

            // 清理临时文件夹
            Directory.Delete(_tempWorkSpace, true);

            return true;
        }

        private static void CopyFiles(string inputPath, string outputPath, string excludePath = "")
        {
            var files = Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories)
                .Where(input => string.IsNullOrWhiteSpace(excludePath) || !input.StartsWith(excludePath))
                .Select(input => input.Remove(0, inputPath.Length).TrimStart('/', '\\'))
                .ToArray();
            foreach (var file in files)
            {
                var sourceFileName = Path.Combine(inputPath, file);
                var destFileName = Path.Combine(outputPath, file);

                var destDirectory = new FileInfo(destFileName).Directory?.ToString();
                if (destDirectory == null) continue;
                Directory.CreateDirectory(destDirectory);
                File.Copy(sourceFileName, destFileName, true);
            }
        }
    }
}