using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ONI_ModInfo_Task
{
    public class ModInfoTask : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Task !!");
            return true;
        }
    }
}