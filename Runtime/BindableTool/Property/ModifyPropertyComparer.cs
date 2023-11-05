using System.Collections.Generic;

namespace YuzeToolkit.BindableTool
{
    public struct ModifyPropertyComparer : IComparer<ModifyProperty>
    {
        public static ModifyPropertyComparer Comparer = new();

        public int Compare(ModifyProperty x, ModifyProperty y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;

            return x switch
            {
                AddModifyProperty => y is AddModifyProperty ? 0 : -1,
                _ => y is MultModifyProperty ? 0 : 1
            };
        }
    }
}