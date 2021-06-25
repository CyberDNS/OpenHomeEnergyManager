using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.DataHistorizationServices
{
    public class DataHistorizationService
    {
        private ConcurrentDictionary<Key, object> _historizationDatasets = new ConcurrentDictionary<Key, object>();

        public HistorizationDataset<TDataset> GetHistorizationDataset<TDataset>(int aggregateId, TimeSpan timeBetweenDatapoints)
        {
            Key key = new Key() { Type = typeof(TDataset), AggregateId =aggregateId };

            return (HistorizationDataset<TDataset>)_historizationDatasets.GetOrAdd(key, new HistorizationDataset<TDataset>(timeBetweenDatapoints));
        }

        private record Key
        {
            public Type Type { get; init; }
            public int AggregateId { get; set; }
        }
    }
}
