using UnityEditor;
using UnityEngine;

//
// author:Aer
// https://github.com/dengxuhui/MultiLanguage

namespace Editor.MultiLanguage
{
    /// <summary>
    /// 多语言资源管理器
    /// </summary>
    public static class MultiLanguageAssetsManager
    {
        /// <summary>
        /// 多语言根目录
        /// </summary>
        public const string RootPath = "Assets/MultiLanguage";

        static MultiLanguageAssetsManager()
        {
            //初始化检验目录
        }

        internal static MultiLanguageRules GetRules()
        {
            return GetAsset<MultiLanguageRules>(RootPath + "MultiLangRules.asset");
        }

        private static T GetAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            //没有就创建一个资源出来
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }
}