using System;
using UnityEngine;

namespace YuzeToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FilePathAttribute : PropertyAttribute
    {
        /// <summary>
        /// 默认从Unity项目的Project文件夹开始索引，例如：索引最开始从SampleScene，路径：Assets/Scenes/SampleScene.unity
        /// </summary>
        /// <param name="relativePath">设置绝对路径，从什么地方开始索引。例如设置为"Assets",索引最开始从SampleScene，路径：Scenes/SampleScene.unity</param>
        public FilePathAttribute(string relativePath = null)
        {
            RelativePath = relativePath;
        }

        public string RelativePath { get; }
    }
}