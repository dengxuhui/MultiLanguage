using System.Collections.Generic;
using UnityEngine.Experimental.PlayerLoop;

namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言工具静态配置
    /// </summary>
    public static class MultiLanguageConfig
    {
        #region 窗口设置

        /// <summary>
        /// 是否默认可选导出语言，不可选就默认全部导出
        /// </summary>
        public static bool DefaultSelectExportLang = false;

        /// <summary>
        /// 默认是否导出翻译需求表
        /// </summary>
        public static bool DefaultExportTranslateTable = false;

        /// <summary>
        /// 默认是否更新TMP
        /// </summary>
        public static bool DefaultUpdateTMP = false;

        #endregion

        /// <summary>
        /// 默认的翻译表文件夹路径
        /// </summary>
        public const string TranslateFolderPrefsKey = "TranslateFBPrefsKey";

        #region 导出文件相关设置

        /// <summary>
        /// 当前使用中的语言总档
        /// </summary>
        public const string CsvNameSummaryUsing = "Using@Summary.csv";

        /// <summary>
        /// 已翻译的语言总档
        /// </summary>
        public const string CsvNameSummaryTranslated = "Translated@Summary.csv";

        /// <summary>
        /// 被丢弃字段缓存
        /// </summary>
        public const string CsvNameDiscardCache = "DiscardCache@Summary.csv";

        /// <summary>
        /// 翻译需求表
        /// </summary>
        public const string CsvNameSummaryTranslating = "翻译需求_Version@{0}.csv";

        /// <summary>
        /// 生成多语言文件名格式
        /// </summary>
        public const string BuildLanguageFormat = "AllLanguage{0}.csv";

        /// <summary>
        /// ui csv源文件
        /// </summary>
        public const string CsvNameRawUI = "UILanguage.csv";

        #endregion

        #region 前缀设置

        /// <summary>
        /// 字段格式化字符串，主要用于兼用老版本的多语言导出工具
        /// </summary>
        public static readonly Dictionary<string, string> FieldFormatDic = new Dictionary<string, string>()
        {
            {
                "AllConfLanguage", "AllConfLanguage_mKeyValue_{0}"
            },
            {
                "ShowMessage", "ShowMessage_mMessage_{0}"
            },
            {
                "UILanguage", "UILanguage_mKeyValue_{0}"
            }
        };

        /// <summary>
        /// 历史遗留问题 创建一个黑名单来排除原始文件中出现这几个key值直接删除
        /// </summary>
        public static readonly List<string> BlackRawKey = new List<string>()
        {
            "Key Name", "KeyName", "string"
        };

        #endregion
    }
}