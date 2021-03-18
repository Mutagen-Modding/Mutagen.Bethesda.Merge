using Mutagen.Bethesda;

namespace MutagenMerger.Lib
{
    public static class Utils
    {
        public static BinaryWriteParameters SafeBinaryWriteParameters => new()
        {
                MasterFlag = BinaryWriteParameters.MasterFlagOption.ChangeToMatchModKey,
                ModKey = BinaryWriteParameters.ModKeyOption.CorrectToPath,
                RecordCount = BinaryWriteParameters.RecordCountOption.Iterate,
                LightMasterLimit = BinaryWriteParameters.LightMasterLimitOption.ExceptionOnOverflow,
                MastersListContent = BinaryWriteParameters.MastersListContentOption.Iterate,
                FormIDUniqueness = BinaryWriteParameters.FormIDUniquenessOption.Iterate,
                NextFormID = BinaryWriteParameters.NextFormIDOption.Iterate
        };
    }
}
