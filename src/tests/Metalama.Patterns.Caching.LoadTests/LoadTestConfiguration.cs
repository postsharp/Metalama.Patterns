using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.LoadTests
{
    public class LoadTestConfiguration
    {
        public int ClientsCount { get; set; }

        public Interval ValueKeyLenght { get; set; }

        public int ValueKeysCount { get; set; }

        public Interval ValueKeyExpiry { get; set; }

        public Interval ValueLength { get; set; }

        public Interval DependencyKeyLenght { get; set; }

        public int DependencyKeysCount { get; set; }

        public Interval DependenciesPerValueCount { get; set; }

        public Interval ValuesPerSharedDependency { get; set; }
    }
}
