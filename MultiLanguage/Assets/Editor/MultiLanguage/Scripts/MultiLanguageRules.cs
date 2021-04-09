using System;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言配置文件
    /// </summary>
    public class MultiLanguageRules : ScriptableObject
    {
        [Header("文件目录相关设置（相对Assets目录）")]
        [Tooltip("各种语言存储的原始文件")]
        public string rawDirectory = "Editor/MultiLanguage/Assets/Raw/";
        [Tooltip("各个语言生成后导出的目录")] public string buildDirectory = "Editor/MultiLanguage/Assets/Build/";
        [Tooltip("汇总文件目录")] public string summaryDirectory = "Editor/MultiLanguage/Assets/Summary/";
        [Tooltip("翻译中的目录")]
        public string translatingDirectory = "Editor/MultiLanguage/Assets/Translating/";
        [Header("基础语言设置")] public SupportLanguage baseLanguage = new SupportLanguage();
        [Header("支持语言列表配置")] public SupportLanguage[] supports = new SupportLanguage[0];

        /// <summary>
        /// 当前翻译的版本号，用于回写文件时进行比对，高版本号可以冲掉低版本号的翻译
        /// </summary>
        [HideInInspector] public int translateVersion = 0;
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