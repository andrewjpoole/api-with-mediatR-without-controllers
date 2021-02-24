using System;

namespace mediatr_test.StatisticsGatherer
{
    public interface IStatisticsQueuedHostedService 
    {
        TimeSpan AverageRequestDuration { get; }
        TimeSpan AverageTimeBetweenRequests { get; }
        DateTime LastResponseSent { get; }
        DateTime ServiceStarted { get; }
        int TotalRequestsReceived { get; }
        object GetStats();
    }
}
