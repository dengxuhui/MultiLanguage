﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

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

        /// <summary>
        /// 通过文件名生成唯一key
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GenerateUniqueKeyByFileName(string filename, string key)
        {
            Config.FieldFormatDic.TryGetValue(filename, out var format);
            if (string.IsNullOrEmpty(format))
            {
                return filename + "_" + key;
            }
            else
            {
                return string.Format(format, key);
            }
        }

        /// <summary>
        /// 获取目录下所有csv文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetAllCsvFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.csv", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取所有ui文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetAllUiFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取所有配置文件集合
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetAllConfigFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.xlsx", SearchOption.AllDirectories);
        }
    }
}