using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdig.Extensions.Xmd.CSCode
{
    internal class Globals
    {
        private static List<string> _globalScriptReferences = new() { "Markdig.Extensions.Xmd.dll" };
        private static List<string> _globalScriptUsings = new() { "Markdig.Extensions.Xmd" };

        public static string PreviousScript { get; internal set; }
        public static string GlobalScript => 
            $"#r \"{string.Join("\"\r\n#r \"", _globalScriptReferences)}\"\r\n" +
            $"using {string.Join(";\r\nusing ", _globalScriptUsings)};\r\n";
        public static void AddToGlobalScript(string[] references = null, string[] usings = null)
        {
            if (references != null)
            {
                _globalScriptReferences.AddRange(references);
                _globalScriptReferences = _globalScriptReferences.Distinct().ToList();
            }

            if (usings != null)
            {
                _globalScriptUsings.AddRange(usings);
                _globalScriptUsings = _globalScriptUsings.Distinct().ToList();
            }
        }
    }
}