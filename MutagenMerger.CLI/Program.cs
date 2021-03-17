using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Mutagen.Bethesda;
using MutagenMerger.Lib;

namespace MutagenMerger.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(Run);
        }

        private static async Task Run(Options options)
        {
            var plugins = options.PluginsTxt != string.Empty 
                ? (await File.ReadAllLinesAsync(options.PluginsTxt))
                    .Where(x => x[0] == '*')
                    .Select(x => ModKey.FromNameAndExtension(x.AsSpan()[1..]))
                    .ToList()
                : options.Plugins
                    .Select(x => ModKey.FromNameAndExtension(x))
                    .ToList();
            var modsToMerge = options.PluginsToMerge
                .Select(x => ModKey.FromNameAndExtension(x))
                .ToList();

            var sw = new Stopwatch();
            sw.Start();
            using (var merger = new Merger(options.DataFolder, plugins, modsToMerge,
                ModKey.FromNameAndExtension(options.Output)))
            {
                merger.Merge();
            }
            
            Console.WriteLine($"Merged {modsToMerge.Count} in {sw.ElapsedMilliseconds}ms");
            sw.Stop();
        }
    }
}
