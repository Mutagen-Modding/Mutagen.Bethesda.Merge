using System;
using System.Collections.Generic;
using CommandLine;
using Mutagen.Bethesda;

namespace MutagenMerger.CLI
{
    public class Options
    {
        
        [Option("game", HelpText = "Game to mod (Default SkyrimSE)")]
        public GameRelease Game { get; set; } = GameRelease.SkyrimSE;

        [Option("data", HelpText = "Path to the data folder", Required = true)]
        public string DataFolder { get; set; } = string.Empty;
        
        [Option("merge", Min = 1, Max = 4096, HelpText = "Plugins to merge")]
        public IEnumerable<string> PluginsToMerge { get; set; } = Array.Empty<string>();

        [Option("mergefile", HelpText = "Get plugins to merge from file")]
        public string PluginsMergeTxt { get; set; } = string.Empty;
        
        [Option("output", Required = true, HelpText = "Output merge folder")]
        public string Output { get; set; } = string.Empty;
        
        [Option("mergename", Required = true, HelpText = "Name of Merge")]
        public string MergeName { get; set; } = string.Empty;
    }
}
