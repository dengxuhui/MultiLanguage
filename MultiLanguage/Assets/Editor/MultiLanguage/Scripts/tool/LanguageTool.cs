using UnityEngine;

namespace Editor.MultiLanguage.Scripts.tool
{
    public static class LanguageTool
    {
        public static TMP_Font GetSdfFontByLanguage(MultiLanguageRules rules,Language language)
        {
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