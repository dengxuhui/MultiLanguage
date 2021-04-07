namespace Editor.MultiLanguage.Scripts
{
    /// <summary>
    /// 多语言工具静态配置
    /// </summary>
    public static class MultiLanguageConfig
    {
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
        /// <summary>
        /// 默认的翻译表文件夹路径
        /// </summary>
        public const string TranslateFolderPrefsKey = "TranslateFBPrefsKey";
    }
}