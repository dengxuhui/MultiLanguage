using System;
using System.Collections.Generic;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 语言类型定义
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// 中文
        /// </summary>
        Chinese,

        /// <summary>
        /// 中文繁体
        /// </summary>
        ChineseTraditional,

        /// <summary>
        /// 英语
        /// </summary>
        English,

        /// <summary>
        /// 俄语
        /// </summary>
        Russian,

        /// <summary>
        /// 日语
        /// </summary>
        Japan,

        /// <summary>
        /// 泰语
        /// </summary>
        Thai,

        /// <summary>
        /// 意大利语
        /// </summary>
        Italian,

        /// <summary>
        /// 土耳其语
        /// </summary>
        Turkish,
    }

    /// <summary>
    /// 字段状态
    /// </summary>
    public static class FieldState
    {
        public const string Add = "Add";
        public const string Modify = "Modify";
        public const string Translated = "Translated";
    }

    /// <summary>
    /// csv中字段信息
    /// </summary>
    public class CsvFieldInfo
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Name;

        /// <summary>
        /// 列，有多少语言就有多少列
        /// </summary>
        private readonly Dictionary<Language, string> _contents = new Dictionary<Language, string>();

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="language"></param>
        /// <param name="content"></param>
        public void Add(Language language, string content)
        {
            _contents.Add(language, content);
        }

        public void TryGetValue(Language language, out string content)
        {
            _contents.TryGetValue(language, out content);
        }

        public string GetValue(Language language)
        {
            return _contents[language];
        }

        public void Walk(Action<Language, string> action)
        {
            if (action == null)
            {
                return;
            }
            foreach (var kv in _contents)
            {
                action.Invoke(kv.Key,kv.Value);
            }
        }
    }
}