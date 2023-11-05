using System.Collections.Generic;

namespace YuzeToolkit.BindableTool
{
    public struct ModifyStateComparer : IComparer<ModifyState>
    {
        public static ModifyStateComparer Comparer = new();
        public int Compare(ModifyState x, ModifyState y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;


            return x switch
            {
                OrModifyState => y is OrModifyState ? 0 : -1,
                AndModifyState => y is AndModifyState ? 0 : 1,
                _ => 0
            };
        }
    }
}