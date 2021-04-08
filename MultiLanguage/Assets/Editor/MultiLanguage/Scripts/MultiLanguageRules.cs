﻿using System;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言配置文件
    /// </summary>
    public class MultiLanguageRules : ScriptableObject
    {
        [Header("文件目录相关设置（相对Assets目录）")]
        [Tooltip("原始文件收集目录，工具会收集该目录下所有文件（包括子目录）中的xlsx文件并收集语言合成基础基础语言表到ExportDirectory下")]
        public string rawDirectory = "Editor/MultiLanguage/RawAssets/";

        [Tooltip("资源导出目录")] public string exportDirectory = "Editor/MultiLanguage/ExportAssets/";
        [Header("基础语言设置")] public SupportLanguage baseLanguage = new SupportLanguage();
        [Header("支持语言列表配置")] public SupportLanguage[] supports = new SupportLanguage[0];
    }

    /// <summary>
    /// 支持语言
    /// </summary>
    [Serializable]
    public class SupportLanguage
    {
        /// <summary>
        /// 语言类型
        /// </summary>
        [Tooltip("支持语言")] public Language language = Language.Chinese;

        /// <summary>
        /// 导出文件后缀
        /// </summary>
        [Tooltip("简写用于导出文件后缀,如果为空字符串就直接用语言名字为文件后缀")]
        public string abbr = "";

        /// <summary>
        /// csv文件中显示的头
        /// </summary>
        [Tooltip("在总表AllLanguage@Raw.csv文件中语言头显示什么，如果控制符就以Language宏为标题")]
        public string csvHeader = "";
    }
}