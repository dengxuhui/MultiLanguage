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
        /// 是否导出翻译表
        /// </summary>
        private bool _exportTranslate = Config.DefaultExportTranslateTable;

        /// <summary>
        /// 是否更新tmp字符
        /// </summary>
        private bool _updateTMP = Config.DefaultUpdateTMP;

        /// <summary>
        /// 更新ui
        /// </summary>
        private bool _updateUI = true;

        /// <summary>
        /// 更新配置
        /// </summary>
        private bool _updateConfig = true;

        /// <summary>
        /// 打开入口
        /// </summary>
        [MenuItem("Window/MultiLanguage", false, 0)]
        static void Init()
        {
            GetWindow(typeof(MultiLanguageWindow));
        }

        #region ui逻辑

        private void SelectTranslateFile()
        {
            _translateFeedbackPath = EditorUtility.OpenFilePanelWithFilters("选择翻译反馈总表（xlsx文件）", _translateFolder,
                new string[] {"xlsx", "xlsx"});
            EditorPrefs.SetString(Config.TranslateFolderPrefsKey, _translateFeedbackPath);
        }

        private void OnGUI()
        {
            //----------------------------------------------------------

            #region 导出本地化

            _funcExport = EditorGUILayout.BeginToggleGroup("------------1.语言导出------------", _funcExport);
            _updateUI = EditorGUILayout.ToggleLeft("是否检查UI更新", _updateUI);
            _updateConfig = EditorGUILayout.ToggleLeft("是否检查Conf更新", _updateConfig);
            //翻译需求
            _exportTranslate = EditorGUILayout.ToggleLeft("是否导出翻译需求表", _exportTranslate);
            //TMP字符更新
            _updateTMP = EditorGUILayout.ToggleLeft("是否更新TMP", _updateTMP);

            if (GUILayout.Button("一键导出"))
            {
                Debug.Log("start build language....");
                FuncExport2Csv.Start(_exportTranslate, _updateTMP, _updateUI, _updateConfig);
                Debug.Log("complete build language....");
                EditorUtility.DisplayDialog("完成", "一键导出完成", "OK");
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