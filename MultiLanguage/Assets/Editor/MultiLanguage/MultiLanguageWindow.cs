﻿using System.Collections.Generic;
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
        //------------------功能模块标志位
        //功能导出
        private bool _funcExport = false;

        //更新翻译
        private bool _funcUpdateTranslate = false;
        //拷贝资源
        private bool _funcCopy = false;
        //-------------------

        #region 翻译导入相关

        private string _translateFeedbackPath;

        #endregion

        /// <summary>
        /// 导出语言开关
        /// </summary>
        private Dictionary<Language, bool> _exportSwitchDic;

        /// <summary>
        /// 选定指定语言
        /// </summary>
        private bool _selectExportLang;

        /// <summary>
        /// 是否导出翻译表
        /// </summary>
        private bool _exportTranslate;

        /// <summary>
        /// 是否更新tmp字符
        /// </summary>
        private bool _updateTMP;

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
            //开发阶段使用判断
            if (_exportSwitchDic != null)
            {
                return;
            }

            _funcExport = false;

            MultiLanguageRules rules = MultiLanguageAssetsManager.GetRules();
            _exportSwitchDic = new Dictionary<Language, bool>();
            var supportLangs = rules.supports;
            _selectExportLang = Config.DefaultSelectExportLang;
            //默认全量导出
            for (var i = 0; i < supportLangs.Length; i++)
            {
                _exportSwitchDic.Add(supportLangs[i].language, !_selectExportLang);
            }

            _exportTranslate = Config.DefaultExportTranslateTable;
        }

        private void OnGUI()
        {
            InitDataByRule();
            //----------------------------------------------------------

            #region 导出本地化

            _funcExport = EditorGUILayout.BeginToggleGroup("------------1.语言导出------------", _funcExport);
            //选择导出语言
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
            //翻译需求
            _exportTranslate = EditorGUILayout.ToggleLeft("是否导出翻译需求表", _exportTranslate);
            //TMP字符更新
            _updateTMP = EditorGUILayout.ToggleLeft("是否更新TMP", _updateTMP);

            if (GUILayout.Button("一键导出", GUILayout.Width(500)))
            {
                Debug.Log("start export language....");
            }

            EditorGUILayout.EndToggleGroup();

            #endregion

            #region 翻译回写

            _funcUpdateTranslate =
                EditorGUILayout.BeginToggleGroup("------------2.更新翻译------------", _funcUpdateTranslate);

            if (GUILayout.Button("更新翻译", GUILayout.Width(500)))
            {
                Debug.Log("start update translate....");
            }
            EditorGUILayout.EndToggleGroup();

            #endregion

            #region 拷贝到AssetPackage目录下
            
            _funcCopy =
                EditorGUILayout.BeginToggleGroup("------------3.拷贝到运行时------------", _funcCopy);
            EditorGUILayout.LabelField("说明：每次导出或更新翻译后需要执行一次拷贝到AssetPackage下，这样运行时才能正确加载到资源");
            if (GUILayout.Button("一键拷贝", GUILayout.Width(500)))
            {
                Debug.Log("start copy csv 2 assetpackage....");
            }
            
            EditorGUILayout.EndToggleGroup();
            #endregion
        }

        #endregion
    }
}