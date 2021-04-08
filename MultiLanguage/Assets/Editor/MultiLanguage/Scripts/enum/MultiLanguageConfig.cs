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
        /// 翻译需求表
        /// </summary>
        public const string CsvNameSummaryTranslating = "翻译需求_Version@{0}.csv";
        /// <summary>
        /// 生成多语言文件名格式
        /// </summary>
        public const string BuildLanguageFormat = "AllLanguage{0}.csv";
        #endregion
    }
}