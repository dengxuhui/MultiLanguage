using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts.tool
{
    /// <summary>
    /// 文件操作工具
    /// </summary>
    public static class FileTool
    {
        // public static 

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory">全路径目录</param>
        public static void MakeDir(string directory)
        {
            try
            {
                var parent = Directory.GetParent(directory).FullName;
                if (!Directory.Exists(parent))
                {
                    MakeDir(parent);
                }

                if (Directory.Exists(directory))
                {
                    return;
                }

                Directory.CreateDirectory(directory);
            }
            catch (Exception)
            {
                EditorUtility.DisplayDialog("创建目录时出错", "创建目录时出错", "OK");
            }
        }

        /// <summary>
        /// 获取全路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(string path)
        {
            return Path.Combine(Application.dataPath, path);
        }
    }
}