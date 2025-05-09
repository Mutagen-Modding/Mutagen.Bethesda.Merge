using Autofac;
using MutagenMerger.Lib.DI;
using Noggog.Autofac;

namespace MutagenMerger.CLI.Container;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Merger<,,,>)).AsSelf();
        builder.RegisterGeneric(typeof(CopyRecordProcessor<,>)).AsSelf();
        builder.RegisterAssemblyTypes(typeof(IMerger).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
    }
}
