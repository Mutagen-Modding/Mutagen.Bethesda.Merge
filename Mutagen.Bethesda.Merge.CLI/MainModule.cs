using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda.Merge.Lib;

namespace Mutagen.Bethesda.Merge.CLI;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystem>().As<IFileSystem>()
            .SingleInstance();
        builder.RegisterModule<MergerModule>();
    }
}
