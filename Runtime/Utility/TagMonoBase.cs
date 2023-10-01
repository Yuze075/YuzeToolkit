using UnityEngine;
using YuzeToolkit.MonoDriver;

namespace YuzeToolkit.Utility
{
    public abstract class TagMonoBase : MonoBase
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [SerializeField] [Attributes.HelpBox(nameof(Info), Attributes.InfoType.Normal)]
        private string str;

        [SerializeField] [Attributes.HelpBox(nameof(WarningInfo))]
        private string strWarning;
        [TextArea] [SerializeField] private string detail;
#endif

        #endregion

        public virtual string Info => null;
        public virtual string WarningInfo => null;
    }
}