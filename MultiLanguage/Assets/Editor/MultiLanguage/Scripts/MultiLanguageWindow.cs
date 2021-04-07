using System.Collections.Generic;
using Editor.MultiLanguage.Scripts.func;
using UnityEditor;
using UnityEngine;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

namespace Editor.MultiLanguage.Scripts
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
        private string _translateFolder;

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

            _translateFolder = EditorPrefs.GetString(Config.TranslateFolderPrefsKey);

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
            _updateTMP = Config.DefaultUpdateTMP;
        }

        private void SelectTranslateFile()
        {
            _translateFeedbackPath = EditorUtility.OpenFilePanelWithFilters("选择翻译反馈总表（xlsx文件）", _translateFolder,
                new string[] {"xlsx", "xlsx"});
            EditorPrefs.SetString(Config.TranslateFolderPrefsKey, _translateFeedbackPath);
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

            if (GUILayout.Button("一键导出"))
            {
                //TODO
                Debug.Log("start export language....");
                FuncExport2Csv.Start();
            }

            EditorGUILayout.EndToggleGroup();

            #endregion

            #region 翻译回写

            _funcUpdateTranslate =
                EditorGUILayout.BeginToggleGroup("------------2.更新翻译------------", _funcUpdateTranslate);
            EditorGUILayout.LabelField("说明：以翻译中的字段为准，包括基础语言也直接覆盖，回写完成会输出一个未翻译列表到指定目录下");
            //选择总表
            GUILayout.BeginHorizontal();
            GUILayout.Label("反馈的翻译总表路径：", GUILayout.MaxWidth(120));
            _translateFeedbackPath = GUILayout.TextField(_translateFeedbackPath);
            if (GUILayout.Button("文件", GUILayout.MaxWidth(60)))
            {
                Debug.Log("start select feedback file");
                SelectTranslateFile();
            }

            GUILayout.EndHorizontal();


            if (GUILayout.Button("更新翻译"))
            {
                //TODO
                Debug.Log("start update translate....");
                FuncUpdateTranslate.Start(_translateFeedbackPath);
            }

            EditorGUILayout.EndToggleGroup();

            #endregion

            #region 拷贝到AssetPackage目录下

            _funcCopy =
                EditorGUILayout.BeginToggleGroup("------------3.拷贝到运行时------------", _funcCopy);
            EditorGUILayout.LabelField("说明：每次导出或更新翻译后需要执行一次拷贝到AssetPackage下，这样运行时才能正确加载到资源");
            if (GUILayout.Button("一键拷贝"))
            {
                //TODO
                Debug.Log("start copy csv 2 assetpackage....");
                FuncCopy2Runtime.Start();
            }

            EditorGUILayout.EndToggleGroup();

            #endregion
        }

        #endregion
    }
}