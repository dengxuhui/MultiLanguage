using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Config = Editor.MultiLanguage.MultiLanguageConfig;

namespace Editor.MultiLanguage
{
    /// <summary>
    /// 多语言控制面板
    /// author by Aer @2021.04.02
    /// </summary>
    public class MultiLanguageWindow : EditorWindow
    {
        /// <summary>
        /// 导出语言开关
        /// </summary>
        private Dictionary<Language, bool> _exportSwitchDic;

        /// <summary>
        /// 选定指定语言
        /// </summary>
        private bool _selectExportLang;
        
        /// <summary>
        /// 打开入口
        /// </summary>
        [MenuItem("Window/MultiLanguage", false, 0)]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(MultiLanguageWindow));
        }

        #region ui逻辑

        /// <summary>
        /// 通过rule初始化数据
        /// </summary>
        private void InitDataByRule()
        {
            if (_exportSwitchDic == null)
            {
                MultiLanguageRules rules = MultiLanguageAssetsManager.GetRules();
                _exportSwitchDic = new Dictionary<Language, bool>();
                var supportLangs = rules.supports;
                _selectExportLang = false;
                //默认全量到处
                for (var i = 0; i < supportLangs.Length; i++)
                {
                    _exportSwitchDic.Add(supportLangs[i].language, !_selectExportLang);
                }
            }
        }

        private void OnGUI()
        {
            InitDataByRule();
            //----------------------------------------------------------

            #region 选择导出语言
            GUILayout.Label("请选择导出语言", EditorStyles.boldLabel);
            _selectExportLang = EditorGUILayout.BeginToggleGroup("导出指定语言（默认全导）", _selectExportLang);
            var tDci = new Dictionary<Language, bool>();
            foreach (var langExportsKv in _exportSwitchDic)
            {
                var value = EditorGUILayout.Toggle(langExportsKv.Key.ToString(), langExportsKv.Value);
                tDci.Add(langExportsKv.Key, value);
            }
            foreach (var langExportsKv in tDci)
            {
                _exportSwitchDic[langExportsKv.Key] = langExportsKv.Value;
            }
            EditorGUILayout.EndToggleGroup();
            #endregion
        }

        #endregion
    }
}