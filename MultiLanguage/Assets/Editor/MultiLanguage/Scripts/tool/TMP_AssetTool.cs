// ReSharper disable All

using UnityEngine;

namespace Editor.MultiLanguage.Scripts.tool
{
    /// <summary>
    /// text mesh pro字体相关工具
    /// </summary>
    public static class TMP_AssetTool
    {
        /// <summary>
        /// 获取语言在设置中的字体文件设置
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static TMP_Font GetTMP_Font(Language language)
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            if (rules == null || rules.supports.Length <= 0)
            {
                Debug.LogError("Get sdf font config error,language rules config not exist supports language array");
                return TMP_Font.Common;
            }

            var supports = rules.supports;
            for (var i = 0; i < supports.Length; i++)
            {
                if (supports[i].language == language)
                {
                    return supports[i].tmpFont;
                }
            }
            Debug.LogError($"Can not find [{language.ToString()}] in multi language support language array!!!!");
            return TMP_Font.Common;
        }
    }
}