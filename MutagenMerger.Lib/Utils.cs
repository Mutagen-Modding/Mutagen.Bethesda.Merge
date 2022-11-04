using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Parameters;

namespace MutagenMerger.Lib
{
    public static class Utils
    {
        public static BinaryWriteParameters SafeBinaryWriteParameters => new()
        {
                MasterFlag = MasterFlagOption.ChangeToMatchModKey,
                ModKey = ModKeyOption.CorrectToPath,
                RecordCount = RecordCountOption.Iterate,
                LightMasterLimit = LightMasterLimitOption.ExceptionOnOverflow,
                MastersListContent = MastersListContentOption.Iterate,
                FormIDUniqueness = FormIDUniquenessOption.Iterate,
                NextFormID = NextFormIDOption.Iterate
        };
    }
}
