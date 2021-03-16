using System.Collections.Generic;
using CommandLine;
using JetBrains.Annotations;

namespace MutagenMerger.CLI
{
    [PublicAPI]
    public class Options
    {
        [Option("plugins", Min = 1, Max = 256, Required = true)]
        public IEnumerable<string>? Plugins { get; set; }
    }
}
