#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AddBatchFilesToBuild : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {

        if (report.summary.result != BuildResult.Failed || report.summary.result != BuildResult.Cancelled)
        {
            var path = report.summary.outputPath;
            var dir = Directory.GetParent(path);

            string newPath = dir.FullName + "\\Run Shortcuts"; 
            if (!Directory.Exists(newPath))
            {
                FileUtil.CopyFileOrDirectory("Assets/Immersive Unity SDK/Run Shortcuts (DO NOT EDIT)", newPath);



                //Delete meta files
                dir = new DirectoryInfo(newPath);
                FileInfo[] files = dir.GetFiles("*.meta")
                                     .Where(p => p.Extension == ".meta").ToArray();
                foreach (FileInfo file in files)
                    try
                    {
                        file.Attributes = FileAttributes.Normal;
                        File.Delete(file.FullName);
                        
                    }
                    catch { }
                try
                {
                    File.Delete(newPath + "\\AddBatchFilesToBuild.cs");
                }
                catch { }

                AssetDatabase.Refresh();



            }
        }
    }
}
#endif