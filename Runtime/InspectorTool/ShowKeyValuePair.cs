using System;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public struct ShowKeyValuePair
    {
        public static ShowKeyValuePair GetKeyValuePair<TKey, TValue>(TKey key, TValue value) => new()
        {
            _key = IShowValue.GetShowValue(key),
            _value = IShowValue.GetShowValue(value)
        };

        // ReSharper disable once NotAccessedField.Local
        [Line] [IgnoreParent] [SerializeReference]
        private IShowValue _key;

        // ReSharper disable once NotAccessedField.Local
        [IgnoreParent] [SerializeReference] private IShowValue _value;
    }
}