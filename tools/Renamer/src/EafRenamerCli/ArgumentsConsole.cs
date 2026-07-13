using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace EafRenamerCli
{
    public class Arguments
    {
        private readonly StringDictionary Parameters;

        // Constructor
        public Arguments(string[] Args)
        {
            Parameters = new StringDictionary();
            Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(100));

            Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                TimeSpan.FromMilliseconds(100));

            string Parameter = null;

            foreach (string Txt in Args)
            {
                string[] Parts = Spliter.Split(Txt, 3);
                Parameter = ProcessParts(Parts, Parameter, Remover);
            }

            // In case a parameter is still waiting
            if (Parameter != null && !Parameters.ContainsKey(Parameter))
            {
                Parameters.Add(Parameter, "true");
            }
        }

        private string ProcessParts(string[] Parts, string Parameter, Regex Remover)
        {
            switch (Parts.Length)
            {
                case 1:
                    return ProcessValuePart(Parts, Parameter, Remover);
                case 2:
                    return ProcessParameterPart(Parts, Parameter, null, Remover);
                case 3:
                    return ProcessParameterPart(Parts, Parameter, Parts[2], Remover);
                default:
                    return Parameter;
            }
        }

        private string ProcessValuePart(string[] Parts, string Parameter, Regex Remover)
        {
            if (Parameter != null && !Parameters.ContainsKey(Parameter))
            {
                Parts[0] = Remover.Replace(Parts[0], "$1");
                Parameters.Add(Parameter, Parts[0]);
            }
            return null;
        }

        private string ProcessParameterPart(string[] Parts, string Parameter, string Value, Regex Remover)
        {
            if (Parameter != null && !Parameters.ContainsKey(Parameter))
                Parameters.Add(Parameter, "true");

            string newParameter = Parts[1];

            if (string.IsNullOrEmpty(Value))
                return newParameter;

            if (!Parameters.ContainsKey(newParameter))
            {
                Value = Remover.Replace(Value, "$1");
                Parameters.Add(newParameter, Value);
            }
            return null;
        }

        // Retrieve a parameter value if it exists
        // (overriding C# indexer property)
        public string this[string Param]
        {
            get
            {
                return (Parameters[Param]);
            }
        }
    }
}
