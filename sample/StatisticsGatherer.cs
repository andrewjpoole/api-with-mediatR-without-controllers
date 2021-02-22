using System;
using System.Collections.Generic;
using System.Linq;

namespace mediatr_test
{
    public class StatisticsGatherer : IStatisticsGatherer
    {
        public TimeSpan AverageRequestDuration { get; private set; }
        public TimeSpan AverageTimeBetweenRequests { get; private set; }
        public DateTime LastResponseSent { get; private set; }
        public int TotalRequestsReceived { get; private set; }

        private readonly List<(long TimeSinceLastRequestTicks, long DurationinTicks)> _durations = new List<(long TimeSinceLastRequestTicks, long DurationinTicks)>();

        private const int windowSize = 10;
        
        public object GetStats()
        {
            //return $"AverageRequestDuration: {AverageRequestDuration:dd\\.hh\\:mm\\:ss\\:fff} AverageTimeBetweenRequests: {AverageTimeBetweenRequests:dd\\.hh\\:mm\\:ss\\:fff} (Averages over last {windowSize} requests) LastResponseSent: {LastResponseSent}";
            return new 
            {
                AverageRequestDuration = AverageRequestDuration.ToString("dd\\.hh\\:mm\\:ss\\:fff"),
                AverageTimeBetweenRequests = AverageTimeBetweenRequests.ToString("dd\\.hh\\:mm\\:ss\\:fff"),
                AveragingWindow = windowSize,
                LastResponseSent = LastResponseSent,
                TotalRequestsReceived = TotalRequestsReceived
            }; 
        }

        public void RecordRequestDuration(TimeSpan duration)
        {
            TotalRequestsReceived += 1;

            var now = DateTime.UtcNow; // ToDo use a DateTime abstraction

            var timeSinceLastRequest = LastResponseSent != DateTime.MinValue ? now - LastResponseSent : TimeSpan.FromMilliseconds(0);

            _durations.Add((timeSinceLastRequest.Ticks, duration.Ticks));
            if (_durations.Count > windowSize)
                _durations.RemoveAt(0);

            var doubleAverageRequestDurationTicks = _durations.Select(x => x.DurationinTicks).Average();
            var longAverageRequestDurationTicks = Convert.ToInt64(doubleAverageRequestDurationTicks);
            AverageRequestDuration = new TimeSpan(longAverageRequestDurationTicks);

            if (LastResponseSent != DateTime.MinValue)
            {            
                var doubleAverageTicks = _durations.Select(x => x.TimeSinceLastRequestTicks).Average();
                long longAverageTicks = Convert.ToInt64(doubleAverageTicks);
                AverageTimeBetweenRequests = new TimeSpan(longAverageTicks);
            }
            
            LastResponseSent = now;

            // ToDo add short term and long term windows and calculate trends?
        }        
    }
}
