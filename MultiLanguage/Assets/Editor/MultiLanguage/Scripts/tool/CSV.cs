using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Editor.MultiLanguage.Scripts.tool
{
    /// <summary>
    /// csv文件操作工具
    /// </summary>
    public static class CsvOperater
    {
        /// <summary>
        /// 读取csv文件转换为CsvTable
        /// </summary>
        /// <param name="path"></param>
        /// <returns>CsvTable</returns>
        public static CsvTable Read(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(stream);
                var text = sr.ReadToEnd();
                //关闭流的时候会自动关闭stream
                sr.Close();
                sr.Dispose();

                string[] rows = text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                if (rows.Length <= 0)
                {
                    return null;
                }


                CsvTable tbl = new CsvTable();
                for (var i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    var kv = row.Split('\t');
                    if (kv.Length != 2 || kv[0] == "")
                    {
                        continue;
                    }

                    var fieldInfo = new CsvFieldInfo
                    {
                        Key = kv[0],
                        Value = kv[1]
                    };
                    tbl.AddField(fieldInfo);
                }

                return tbl;
            }
        }

        /// <summary>
        /// 在指定路径写csv文件，如果存在直接覆盖
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="path"></param>
        public static void Write(CsvTable tbl, string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.Unicode))
            {
                for (var i = 0; i < tbl.Count; i++)
                {
                    sw.WriteLine(tbl.ToString(i));
                }
                
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }
    }

    /// <summary>
    /// csv表数据结构
    /// </summary>
    public class CsvTable
    {
        public static readonly CsvFieldInfo Empty = new CsvFieldInfo();

        /// <summary>
        /// 所有字段信息
        /// </summary>
        private List<CsvFieldInfo> _fieldInfos;

        /// <summary>
        /// 获取字段信息
        /// </summary>
        /// <param name="index"></param>
        public CsvFieldInfo this[int index]
        {
            get
            {
                if (index >= 0 && index < _fieldInfos.Count)
                {
                    return _fieldInfos[index];
                }
                else
                {
                    return Empty;
                }
            }
            set => _fieldInfos[index] = value;
        }

        /// <summary>
        /// 获取行数
        /// </summary>
        public int Count => _fieldInfos?.Count ?? 0;

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fieldInfo"></param>
        public void AddField(CsvFieldInfo fieldInfo)
        {
            if (_fieldInfos == null)
            {
                _fieldInfos = new List<CsvFieldInfo>(64);
            }

            _fieldInfos.Add(fieldInfo);
        }

        /// <summary>
        /// 将行转换为字符串格式
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ToString(int index)
        {
            var sb = new StringBuilder();
            sb.Append(_fieldInfos[index].Key);
            sb.Append("\t");
            sb.Append(_fieldInfos[index].Value);
            return sb.ToString();
        }
    }
}