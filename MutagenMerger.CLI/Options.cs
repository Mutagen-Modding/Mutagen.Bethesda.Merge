using System;
using System.Collections.Generic;
using CommandLine;
using JetBrains.Annotations;

namespace MutagenMerger.CLI
{
    [PublicAPI]
    public class Options
    {
        [Option("data", HelpText = "Path to the data folder", Required = true)]
        public string DataFolder { get; set; } = string.Empty;

        [Option("plugins", Min = 1, Max = 256, HelpText = "Plugins to load")]
        public IEnumerable<string> Plugins { get; set; } = Array.Empty<string>();

        [Option("pluginslist", HelpText = "Load plugins from plugins.txt")]
        public string PluginsTxt { get; set; } = string.Empty;
        
        [Option("merge", Min = 1, Max = 256, Required = true, HelpText = "Plugins to merge")]
        public IEnumerable<string> PluginsToMerge { get; set; } = Array.Empty<string>();
        
        [Option("output", Required = true, HelpText = "Output file name")]
        public string Output { get; set; } = string.Empty;
    }
}
