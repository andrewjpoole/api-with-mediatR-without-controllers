using System;

namespace mediatr_test
{
    public interface IStatisticsGatherer
    {
        void RecordRequestDuration(TimeSpan duration);
        object GetStats();
    }
}
