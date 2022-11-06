using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
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
                    .Where(x => !string.IsNullOrEmpty(x) && x[0] != '#' && (options.Game != 0 ? x[0] == '*' : true))
                    .Select(x => ModKey.FromNameAndExtension(options.Game != 0 ? x.AsSpan()[1..] : x))
                    .ToList()
                : options.Plugins
                    .Select(x => ModKey.FromNameAndExtension(x))
                    .ToList();

            var modsToMerge = options.PluginsMergeTxt != string.Empty
                ? (await File.ReadAllLinesAsync(options.PluginsMergeTxt))
                    .Select(x => ModKey.FromNameAndExtension(x))
                    .ToList()
                : options.PluginsToMerge
                    .Select(x => ModKey.FromNameAndExtension(x))
                    .ToList();

            if(Directory.Exists(options.Output)) Directory.Delete(options.Output, true);
            var sw = new Stopwatch();
            sw.Start();

            IMerger merger;
            switch (options.Game)
            {
                case GameRelease.Oblivion:
                    merger = new Merger<IOblivionModGetter, IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter>();
                    break;
                case GameRelease.Fallout4:
                    merger = new Merger<IFallout4ModGetter, IFallout4Mod, IFallout4MajorRecord, IFallout4MajorRecordGetter>();
                    break;
                default:
                    merger = new Merger<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>();
                    break;
            }

            merger.Merge(options.DataFolder, plugins, modsToMerge,
                ModKey.FromNameAndExtension(options.MergeName), options.Output, options.Game);
            
            Console.WriteLine($"Merged {modsToMerge.Count} plugins in {sw.ElapsedMilliseconds}ms");
            sw.Stop();
        }
    }
}
