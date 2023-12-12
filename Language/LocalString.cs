using System;

namespace Language
{
    [Serializable]
    public class LocalString
    {
        public string Code { get; private set; }
        
        public string Text => Localization.Instance.Localize(Code);

        public LocalString(string code)
        {
            Code = code;
        }
    }

}
