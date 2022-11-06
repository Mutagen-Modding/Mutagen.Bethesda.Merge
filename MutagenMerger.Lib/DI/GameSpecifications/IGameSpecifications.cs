using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace MutagenMerger.Lib.DI.GameSpecifications;

public interface IGameSpecifications<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>
    where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter
{
    public IReadOnlyCollection<ObjectKey> BlacklistedCopyTypes { get; }

    public void HandleCopyFor(
        MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec);
}
