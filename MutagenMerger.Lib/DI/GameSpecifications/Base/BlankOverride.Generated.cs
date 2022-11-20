
using Mutagen.Bethesda.Plugins.Cache;
using SkyrimRecord = Mutagen.Bethesda.Skyrim;
using Fallout4Record = Mutagen.Bethesda.Fallout4;
using OblivionRecord = Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;

namespace MutagenMerger.Lib.DI.GameSpecifications;



public class SkyrimPlacedObjectOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedObject, SkyrimRecord.IPlacedObjectGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedObject,  SkyrimRecord.IPlacedObjectGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedNpcOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedNpc, SkyrimRecord.IPlacedNpcGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedNpc,  SkyrimRecord.IPlacedNpcGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedArrowOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedArrow, SkyrimRecord.IPlacedArrowGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedArrow,  SkyrimRecord.IPlacedArrowGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedBarrierOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedBarrier, SkyrimRecord.IPlacedBarrierGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedBarrier,  SkyrimRecord.IPlacedBarrierGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedBeamOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedBeam, SkyrimRecord.IPlacedBeamGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedBeam,  SkyrimRecord.IPlacedBeamGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedConeOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedCone, SkyrimRecord.IPlacedConeGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedCone,  SkyrimRecord.IPlacedConeGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedFlameOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedFlame, SkyrimRecord.IPlacedFlameGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedFlame,  SkyrimRecord.IPlacedFlameGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedHazardOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedHazard, SkyrimRecord.IPlacedHazardGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedHazard,  SkyrimRecord.IPlacedHazardGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedMissileOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedMissile, SkyrimRecord.IPlacedMissileGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedMissile,  SkyrimRecord.IPlacedMissileGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimPlacedTrapOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IPlacedTrap, SkyrimRecord.IPlacedTrapGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IPlacedTrap,  SkyrimRecord.IPlacedTrapGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimDialogResponsesOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.IDialogResponses, SkyrimRecord.IDialogResponsesGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.IDialogResponses,  SkyrimRecord.IDialogResponsesGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class SkyrimNavigationMeshOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, SkyrimRecord.INavigationMesh, SkyrimRecord.INavigationMeshGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter,  SkyrimRecord.INavigationMesh,  SkyrimRecord.INavigationMeshGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedObjectOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedObject, Fallout4Record.IPlacedObjectGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedObject,  Fallout4Record.IPlacedObjectGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedNpcOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedNpc, Fallout4Record.IPlacedNpcGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedNpc,  Fallout4Record.IPlacedNpcGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedArrowOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedArrow, Fallout4Record.IPlacedArrowGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedArrow,  Fallout4Record.IPlacedArrowGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedBarrierOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedBarrier, Fallout4Record.IPlacedBarrierGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedBarrier,  Fallout4Record.IPlacedBarrierGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedBeamOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedBeam, Fallout4Record.IPlacedBeamGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedBeam,  Fallout4Record.IPlacedBeamGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedConeOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedCone, Fallout4Record.IPlacedConeGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedCone,  Fallout4Record.IPlacedConeGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedFlameOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedFlame, Fallout4Record.IPlacedFlameGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedFlame,  Fallout4Record.IPlacedFlameGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedHazardOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedHazard, Fallout4Record.IPlacedHazardGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedHazard,  Fallout4Record.IPlacedHazardGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedMissileOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedMissile, Fallout4Record.IPlacedMissileGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedMissile,  Fallout4Record.IPlacedMissileGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4PlacedTrapOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IPlacedTrap, Fallout4Record.IPlacedTrapGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IPlacedTrap,  Fallout4Record.IPlacedTrapGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4NavigationMeshOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.INavigationMesh, Fallout4Record.INavigationMeshGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.INavigationMesh,  Fallout4Record.INavigationMeshGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4SceneOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IScene, Fallout4Record.ISceneGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IScene,  Fallout4Record.ISceneGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4DialogTopicOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IDialogTopic, Fallout4Record.IDialogTopicGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IDialogTopic,  Fallout4Record.IDialogTopicGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4DialogResponsesOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.IDialogResponses, Fallout4Record.IDialogResponsesGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.IDialogResponses,  Fallout4Record.IDialogResponsesGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class Fallout4LandscapeOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, Fallout4Record.ILandscape, Fallout4Record.ILandscapeGetter>
{
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter,  Fallout4Record.ILandscape,  Fallout4Record.ILandscapeGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionPlacedObjectOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.IPlacedObject, OblivionRecord.IPlacedObjectGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.IPlacedObject,  OblivionRecord.IPlacedObjectGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionPlacedNpcOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.IPlacedNpc, OblivionRecord.IPlacedNpcGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.IPlacedNpc,  OblivionRecord.IPlacedNpcGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionPlacedCreatureOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.IPlacedCreature, OblivionRecord.IPlacedCreatureGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.IPlacedCreature,  OblivionRecord.IPlacedCreatureGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionPathGridOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.IPathGrid, OblivionRecord.IPathGridGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.IPathGrid,  OblivionRecord.IPathGridGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionLandscapeOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.ILandscape, OblivionRecord.ILandscapeGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.ILandscape,  OblivionRecord.ILandscapeGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    

public class OblivionDialogItemOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, OblivionRecord.IDialogItem, OblivionRecord.IDialogItemGetter>
{
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter,  OblivionRecord.IDialogItem,  OblivionRecord.IDialogItemGetter> context)
    {
        // Do nothing.  Handled elsewhere
    }
}

    