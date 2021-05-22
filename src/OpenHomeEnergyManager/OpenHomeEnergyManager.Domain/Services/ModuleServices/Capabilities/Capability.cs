using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.ModuleServices.Capabilities
{
    public class Capability
    {
        public string Key { get; }
        public string Name { get; }

        public Capability(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Capability capability &&
                   Key == capability.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }
    }
}
