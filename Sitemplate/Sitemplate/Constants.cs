using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitemplate
{
    public static class Constants
    {
        public static class Tag
        {
            public const string Inject = "inject";

            public const string Declare = "var";
            public const string Set = "set";

            public const string If = "if";
            public const string Else = "<else>";

            public const string For = "for";
        }

        // decided to skip special handling of variables starting with $ for now.
        //public const char VariablePrefix = '$';

        public const char MoustashePrefix = '{';
        public const string MoustasheStart = "{{";
        public const string MoustasheEnd = "}}";
    }
}
