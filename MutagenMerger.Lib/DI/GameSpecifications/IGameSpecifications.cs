using Mutagen.Bethesda.Plugins.Records;

namespace MutagenMerger.Lib.DI.GameSpecifications;

public interface IGameSpecifications<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>
    where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter
{
    // Nothing needed anymore, due to ICopyOverride existing
    // Maybe we'll have some mechanics later, otherwise can delete
}
