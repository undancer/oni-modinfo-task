using System.IO;
using Microsoft.Build.Framework;
using BaseTask = Microsoft.Build.Utilities.Task;
using ONI_ModInfo_Task.Model;
using YamlDotNet.Serialization;

namespace ONI_ModInfo_Task
{
    public class ModInfoTask : BaseTask
    {
        private readonly ISerializer _serializer = new SerializerBuilder()
            .Build();

        public bool UseArchivedVersions { get; set; }

        public string SupportedContent { get; set; }

        public int LastWorkingBuild { get; set; }

        [Output] 
        public string OutputDirectory { get; set; }

        public override bool Execute()
        {
            OutputDirectory = Path.Combine(
                Path.GetTempPath(),
                "undancer-oni-" + Path.GetRandomFileName()
            );

            foreach (var supportedContent in SupportedContent.Split(','))
            {
                var directory = Path.Combine(OutputDirectory,
                    UseArchivedVersions
                        ? Path.Combine(
                            "archived_versions",
                            $"{supportedContent.ToLower()}_{LastWorkingBuild}"
                        )
                        : ""
                );

                Directory.CreateDirectory(directory);

                using (var writer = new StreamWriter(Path.Combine(directory, "mod_info.yaml")))
                {
                    _serializer.Serialize(writer,
                        new ModInfo
                        {
                            supportedContent = supportedContent,
                            lastWorkingBuild = LastWorkingBuild
                        }
                    );
                }
            }

            return true;
        }
    }
}