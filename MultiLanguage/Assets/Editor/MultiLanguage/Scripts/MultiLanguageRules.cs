﻿using System;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言配置文件
    /// </summary>
    public class MultiLanguageRules : ScriptableObject
    {
        // [Header("文件目录相关设置（相对Assets目录）")] 
        
        [Header("基础语言设置")] public SupportLanguage baseLanguage = new SupportLanguage();
        [Header("支持语言列表配置")] public SupportLanguage[] supports = new SupportLanguage[0];

        #region 隐藏属性
        /// <summary>
        /// 当前翻译的版本号，用于回写文件时进行比对，高版本号可以冲掉低版本号的翻译
        /// </summary>
        [HideInInspector]
        public int translateVersion = 0;
        [HideInInspector]
        public string rawDirectory = "Editor/MultiLanguage/Assets/Raw/";
        [HideInInspector]
        public string buildDirectory = "Editor/MultiLanguage/Assets/Build/";
        [HideInInspector]
        public string summaryDirectory = "Editor/MultiLanguage/Assets/Summary/";
        [HideInInspector]
        public string translatingDirectory = "Editor/MultiLanguage/Assets/Translating/";
        #endregion
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
    }
}