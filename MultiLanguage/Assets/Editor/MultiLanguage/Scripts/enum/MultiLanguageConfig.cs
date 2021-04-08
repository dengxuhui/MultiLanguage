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
        public const string CsvNameMergeUsing = "MergeUsing@Raw.csv";
        /// <summary>
        /// 已翻译的语言总档
        /// </summary>
        public const string CsvNameMergeTranslated = "MergeTranslated@Raw.csv";
        /// <summary>
        /// 翻译需求表
        /// </summary>
        public const string CsvNameMergeTranslating = "翻译需求_Version@{0}.csv";
        /// <summary>
        /// 导出文件名格式
        /// </summary>
        public const string ExportLanguageFormat = "AllLanguage{0}.csv";
        #endregion
    }
}