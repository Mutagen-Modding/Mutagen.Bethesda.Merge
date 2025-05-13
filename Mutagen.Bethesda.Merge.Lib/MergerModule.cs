using Autofac;
using Mutagen.Bethesda.Merge.Lib.DI;
using Noggog.Autofac;
using Noggog.Autofac.Modules;

namespace Mutagen.Bethesda.Merge.Lib;

public class MergerModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<NoggogModule>();
        builder.RegisterGeneric(typeof(AssetMerge<,,,>)).AsSelf();
        builder.RegisterGeneric(typeof(Merger<,,,>)).AsSelf();
        builder.RegisterGeneric(typeof(CopyRecordProcessor<,>)).AsSelf();
        builder.RegisterAssemblyTypes(typeof(IMerger).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
    }
}
