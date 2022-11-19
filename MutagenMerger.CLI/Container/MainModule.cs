using Autofac;
using MutagenMerger.Lib.DI;
using MutagenMerger.Lib.DI.GameSpecifications.Fallout4;
using MutagenMerger.Lib.DI.GameSpecifications.Oblivion;
using MutagenMerger.Lib.DI.GameSpecifications.Skyrim;
using Noggog.Autofac;

namespace MutagenMerger.CLI.Container;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Merger<,,,>)).AsSelf();
        builder.RegisterGeneric(typeof(CopyRecordProcessor<,>)).AsSelf();
        builder.RegisterAssemblyTypes(typeof(SkyrimSpecifications).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
        builder.RegisterAssemblyTypes(typeof(OblivionSpecifications).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
        builder.RegisterAssemblyTypes(typeof(Fallout4Specifications).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
    }
}
