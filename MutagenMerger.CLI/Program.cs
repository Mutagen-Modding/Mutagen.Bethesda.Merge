using System.Diagnostics;
using Autofac;
using CommandLine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using MutagenMerger.Lib.DI;

namespace MutagenMerger.CLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(Run);
    }

    private static async Task Run(Options options)
    {
        var modsToMerge = options.PluginsMergeTxt != string.Empty
            ? (await File.ReadAllLinesAsync(options.PluginsMergeTxt))
            .Select(x => ModKey.FromNameAndExtension(x))
            .ToList()
            : options.PluginsToMerge
                .Select(x => ModKey.FromNameAndExtension(x))
                .ToList();

        if (Directory.Exists(options.Output)) Directory.Delete(options.Output, true);

        var sw = new Stopwatch();
        sw.Start();

        Type[] genericTypes;
        switch (options.Game.ToCategory())
        {
            case GameCategory.Oblivion:
                genericTypes = new Type[] { typeof(IOblivionMod), typeof(IOblivionModGetter),typeof(IOblivionMajorRecord), typeof(IOblivionMajorRecordGetter) };
                break;
            case GameCategory.Fallout4:
                genericTypes = new Type[] { typeof(IFallout4Mod), typeof(IFallout4ModGetter), typeof(IFallout4MajorRecord), typeof(IFallout4MajorRecordGetter) };
                break;
            case GameCategory.Skyrim:
            default:
                genericTypes = new Type[] { typeof(ISkyrimMod), typeof(ISkyrimModGetter), typeof(ISkyrimMajorRecord), typeof(ISkyrimMajorRecordGetter) };
                break;
        }

        ContainerBuilder builder = new();
        builder.RegisterModule<MainModule>();
        builder.RegisterInstance(
            new DataDirectoryInjection(options.DataFolder));
        builder.RegisterInstance(
            new GameReleaseInjection(options.Game));
        var container = builder.Build();
        var merger = container.Resolve(typeof(Merger<,,,>).MakeGenericType(genericTypes)) as IMerger;

        merger!.Merge(
            modsToMerge,
            ModKey.FromNameAndExtension(options.MergeName),
            options.Output);

        Console.WriteLine($"Merged {modsToMerge.Count} plugins in {sw.ElapsedMilliseconds}ms");
        sw.Stop();
    }
}
