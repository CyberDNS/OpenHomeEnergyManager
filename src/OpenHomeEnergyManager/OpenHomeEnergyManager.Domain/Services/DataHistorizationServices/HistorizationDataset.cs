using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Domain.Services.DataHistorizationServices
{
    public class HistorizationDataset<TDataset>
    {
        private readonly TimeSpan _timeBetweenDatapoints;
        private readonly TimeSpan _memoryTimePeriod = TimeSpan.FromMinutes(60);
        private readonly int _countOfDatapoints;


        private Queue<TDataset> _data = new Queue<TDataset>();

        public HistorizationDataset(TimeSpan timeBetweenDatapoints)
        {
            _timeBetweenDatapoints = timeBetweenDatapoints;
            _countOfDatapoints = Convert.ToInt32(_memoryTimePeriod.TotalSeconds / _timeBetweenDatapoints.TotalSeconds);
        }

        public void AddDataset(TDataset dataset)
        {
            _data.Enqueue(dataset);
            if (_data.Count > _countOfDatapoints) { _data.Dequeue(); }
        }

        public IEnumerable<TDataset> GetData(TimeSpan timeSpan)
        {
            int countOfRequestedDatapoints = Convert.ToInt32(timeSpan.TotalSeconds / _timeBetweenDatapoints.TotalSeconds);
            return _data.TakeLast(countOfRequestedDatapoints).ToArray();
        }
    }
}
