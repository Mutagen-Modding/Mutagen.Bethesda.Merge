using Autofac;
using MutagenMerger.Lib;
using MutagenMerger.Lib.DI;

namespace MutagenMerger.CLI.Container;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Merger<,,,>)).AsSelf();
    }
}
