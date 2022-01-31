using System.Collections.Generic;

namespace OntologyCSharpSDK.Wallet
{
    public class Identity
    {
        public string Label { get; set; }
        public string Ontid { get; set; }
        public bool IsDefault { get; set; }
        public bool Lock { get; set; } = false;
        public IList<Control> Controls { get; set; }
        public object Extra { get; set; }

        public Identity()
        {
            Label = string.Empty;
            Ontid = string.Empty;
            Controls = new List<Control>();
        }
    }
}
