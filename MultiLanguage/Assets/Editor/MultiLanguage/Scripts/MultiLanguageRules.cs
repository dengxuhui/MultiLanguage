﻿using System;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言配置文件
    /// </summary>
    public class MultiLanguageRules : ScriptableObject
    {
        [Header("文件目录相关设置（相对Assets父级目录）")]
        [Tooltip("ui根目录")]
        public string uiPrefabDirectory = "";
        [Tooltip("配置根目录")]
        public string configDirectory = "";
        /// <summary>
        /// 忽略数据
        /// </summary>
        public IgnoreData[] ignoreDataArray = new IgnoreData[0];

        [Header("支持语言列表配置")] public SupportLanguage[] supports = new SupportLanguage[0];
        [Header("基础语言在Supports数组中索引")] public int basicSupportIndex = 0;
        
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
        [HideInInspector]
        public string fontDirectory = "Editor/MultiLanguage/Assets/Font/";

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
        /// <summary>
        /// 导出的font  xxx.asset
        /// </summary>
        [Tooltip("分类字体，新增字体需要定义宏，然后在Config中配置sdf字体文件名")]
        public SdfFont sdfFont;
    }

    /// <summary>
    /// 文件忽略
    /// </summary>
    [Serializable]
    public class IgnoreData
    {
        /// <summary>
        /// 类型
        /// </summary>
        public IgnoreType ignoreType;
        /// <summary>
        /// 路径
        /// </summary>
        public string path;
    }

    /// <summary>
    /// 忽略类型
    /// </summary>
    public enum IgnoreType
    {
        /// <summary>
        /// 按目录忽略
        /// </summary>
        Directory,
        /// <summary>
        /// 按文件忽略
        /// </summary>
        File,
    }

    /// <summary>
    /// sdf字体导出文件
    /// </summary>
    public enum SdfFont
    {
        /// <summary>
        /// 通用字体
        /// </summary>
        Common,
        /// <summary>
        /// 泰语
        /// </summary>
        Thai,
    }
}