using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace MutagenMerger.Lib.DI.GameSpecifications;

public interface ICopyOverride<TMod, TModGetter>
    where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
{
    ObjectKey ObjectKey { get; }
    public void HandleCopyFor(
        MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> context);
}

public abstract class ACopyOverride<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> : ICopyOverride<TMod, TModGetter>
    where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter
{
    public ObjectKey ObjectKey { get; } = LoquiRegistration.StaticRegister.GetRegister(typeof(TMajorRecord)).ObjectKey;
    
    public void HandleCopyFor(
        MergeState<TMod, TModGetter> state, 
        IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> context)
    {
        HandleCopyFor(
            state,
            context.AsType<TMod, TModGetter, IMajorRecord, IMajorRecordGetter, TMajorRecord, TMajorRecordGetter>());
    }

    public abstract void HandleCopyFor(MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context);
}
