using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Helper
{
    public class Parameters
    {
        public string ParamKey { get; set; }
        public Object Value { get; set; }
        public Parameters()
        {

        }
        public Parameters(string paramKey,object value)
        {
            ParamKey = paramKey;
            Value = value;
        }
    }
}
