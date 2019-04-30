#if UNITY_EDITOR
namespace BTEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    /// <summary>
    /// AI调试工具
    /// </summary>
    public class AIMenuEditor : Editor
    {
        public const string ScenePathPrefix = "Assets/Scenes/";
        public const string SceneName = "AIDebug";


		[MenuItem("策划工具/AI/打开AI调试场景", false, 212)]
        public static void OpenEditSkill()
        {
            EditorSceneManager.OpenScene(string.Format("{0}{1}.unity", ScenePathPrefix, SceneName));
        }

        [MenuItem("策划工具/AI/生成前端AI加载列表文件", false, 213)]
        public static void GenerateAILoadList()
        {
            string aiClientPath = Application.dataPath + "/EditTools/Data/BehaviorTree/Lua";
            string generateName = "BevTrees.lua";

            StringBuilder result = new StringBuilder();
            result.Append("local BevTrees =\n{\n");

            string[] files = Directory.GetFiles(aiClientPath, "*.lua", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(generateName))
                {
                    continue;
                }

                int startIndex = aiClientPath.Length;
                string shortName = file.Substring(aiClientPath.Length + 1);
                shortName = shortName.Replace("\\", "/");
                string[] temp = shortName.Split('.');

                result.Append("\t\"").Append(temp[0]).Append("\",").Append("\n");
            }

            result.Append("}\n\nreturn BevTrees");

            // Write
            string path = aiClientPath + "/" + generateName;
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            File.WriteAllText(path, result.ToString());

            //打开文件夹
            // System.Diagnostics.Process.Start(aiClientPath);
        }
    }
}
#endif