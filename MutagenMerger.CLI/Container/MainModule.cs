using System.IO.Abstractions;
using Autofac;
using MutagenMerger.Lib.DI;
using Noggog.Autofac;
using Noggog.Autofac.Modules;

namespace MutagenMerger.CLI.Container;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystem>().As<IFileSystem>()
            .SingleInstance();
        builder.RegisterModule<NoggogModule>();
        builder.RegisterGeneric(typeof(Merger<,,,>)).AsSelf();
        builder.RegisterGeneric(typeof(CopyRecordProcessor<,>)).AsSelf();
        builder.RegisterAssemblyTypes(typeof(IMerger).Assembly)
            .InNamespacesOf(typeof(IMerger))
            .AsImplementedInterfaces();
    }
}
