using System.IO.Abstractions;
using Autofac;
using MutagenMerger.Lib;

namespace MutagenMerger.CLI;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystem>().As<IFileSystem>()
            .SingleInstance();
        builder.RegisterModule<MergerModule>();
    }
}
