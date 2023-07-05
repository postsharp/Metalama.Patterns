using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PostSharp.Patterns.Caching.Tests.LoadTests
{
    public class StringCounter
    {
        private readonly Dictionary<string, BigInteger> counters = new Dictionary<string, BigInteger>();

        private readonly Dictionary<string, List<string>> details = new Dictionary<string, List<string>>();

        public IReadOnlyDictionary<string, BigInteger> Counters => this.counters;

        public IReadOnlyDictionary<string, List<string>> Details => this.details;

        public void Increment(string name, string detail = null)
        {
            BigInteger count;

            if (!this.counters.TryGetValue(name, out count))
            {
                this.counters.Add(name, 1);
            }
            else
            {
                this.counters[name] = count + 1;
            }

            if (detail == null)
            {
                return;
            }

            List<string> thisDetails;

            if (!this.details.TryGetValue(name, out thisDetails))
            {
                thisDetails = new List<string>();
                this.details.Add(name, thisDetails);
            }

            thisDetails.Add(detail);
        }
    }
}
