using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using MutagenMerger.CLI.Container;
using MutagenMerger.Lib;
using MutagenMerger.Lib.DI;

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

            Type[] genericTypes;
            switch (options.Game.ToCategory())
            {
                case GameCategory.Oblivion:
                    genericTypes = new Type[] { typeof(IOblivionModGetter), typeof(IOblivionMod), typeof(IOblivionMajorRecord), typeof(IOblivionMajorRecordGetter) };
                    break;
                case GameCategory.Fallout4:
                    genericTypes = new Type[] { typeof(IFallout4ModGetter), typeof(IFallout4Mod), typeof(IFallout4MajorRecord), typeof(IFallout4MajorRecordGetter) };
                    break;
                case GameCategory.Skyrim:
                default:
                    genericTypes = new Type[] { typeof(ISkyrimModGetter), typeof(ISkyrimMod), typeof(ISkyrimMajorRecord), typeof(ISkyrimMajorRecordGetter) };
                    break;
            }
            
            ContainerBuilder builder = new();
            builder.RegisterModule<MainModule>();
            var container = builder.Build();
            var merger = container.Resolve(typeof(Merger<,,,>).MakeGenericType(genericTypes)) as IMerger;

            merger!.Merge(options.DataFolder, plugins, modsToMerge,
                ModKey.FromNameAndExtension(options.MergeName), options.Output, options.Game);
            
            Console.WriteLine($"Merged {modsToMerge.Count} plugins in {sw.ElapsedMilliseconds}ms");
            sw.Stop();
        }
    }
}
