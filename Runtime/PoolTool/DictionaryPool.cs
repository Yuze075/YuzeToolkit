using System.Collections.Generic;

namespace YuzeToolkit.PoolTool
{
    public class DictionaryPool<TKey, TValue> : 
        CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
    }
}