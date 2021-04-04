using System;
using UnityEngine;

namespace Editor.MultiLanguage
{
    /// <summary>
    /// 多语言配置文件
    /// </summary>
    public class MultiLanguageRules : ScriptableObject
    {
        [Header("基础语言设置")]
        public SupportLanguage baseLanguage = new SupportLanguage();
        [Header("支持语言列表配置")]
        public SupportLanguage[] supports = new SupportLanguage[0];
    }

    /// <summary>
    /// 支持语言
    /// </summary>
    [Serializable]
    public class SupportLanguage
    {
        [Tooltip("支持语言")] public Language language = Language.Chinese;
    }
}