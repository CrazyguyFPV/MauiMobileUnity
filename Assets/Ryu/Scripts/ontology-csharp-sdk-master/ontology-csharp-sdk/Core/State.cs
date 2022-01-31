using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using OntologyCSharpSDK.Common;

namespace OntologyCSharpSDK.Core
{
    public class State
    {
        public string from { get; set; }
        public string to { get; set; }
        public long value { get; set; }

        public State()
        {
        } 
    }
}
