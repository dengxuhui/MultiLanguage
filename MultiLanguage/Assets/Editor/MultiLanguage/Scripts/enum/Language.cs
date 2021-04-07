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
    /// xlsx中的字段信息
    /// </summary>
    public struct ExcelFieldInfo
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate;
        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifyDate;
        /// <summary>
        /// 字段状态
        /// </summary>
        public string FieldState;
        /// <summary>
        /// 键
        /// </summary>
        public string Key;
        /// <summary>
        /// 值
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// csv中字段信息
    /// </summary>
    public struct CsvFieldInfo
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key;
        /// <summary>
        /// 值
        /// </summary>
        public string Value;
    }
}