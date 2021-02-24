using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mediatr_test.StatisticsGatherer
{
    public class StatisticsQueuedHostedService : BackgroundService, IStatisticsQueuedHostedService
    {
        public TimeSpan AverageRequestDuration { get; private set; }
        public TimeSpan AverageTimeBetweenRequests { get; private set; }
        public DateTime LastResponseSent { get; private set; }
        public DateTime ServiceStarted { get; private set; }
        public int TotalRequestsReceived { get; private set; }

        private readonly ILogger<StatisticsQueuedHostedService> _logger;
        private IStatisticsTaskQueue _taskQueue { get; }
        private readonly List<(long TimeSinceLastRequestTicks, long DurationinTicks)> _durations = new List<(long TimeSinceLastRequestTicks, long DurationinTicks)>();

        private const int windowSize = 10;

        public StatisticsQueuedHostedService(IStatisticsTaskQueue taskQueue, ILogger<StatisticsQueuedHostedService> logger)
        {
            ServiceStarted = DateTime.UtcNow;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        public object GetStats()
        {            
            return new
            {
                FormatOfTimeSpans = "Days.Hours:Minutes:Seconds:Miliseconds",
                AverageRequestDuration = AverageRequestDuration.ToString("dd\\.hh\\:mm\\:ss\\:fff"),
                AverageTimeBetweenRequests = AverageTimeBetweenRequests.ToString("dd\\.hh\\:mm\\:ss\\:fff"),
                AveragingWindow = windowSize,
                LastResponseSent = LastResponseSent,
                TotalRequestsReceived = TotalRequestsReceived,
                ServiceStartedAt = ServiceStarted,
                UpTime = (DateTime.UtcNow - ServiceStarted).ToString("dd\\.hh\\:mm\\:ss\\:fff")
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"StatisticsQueuedHostedService is running");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var durationRecording = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    // do stuff here
                    TotalRequestsReceived += 1;
                    
                    var timeSinceLastRequest = LastResponseSent != DateTime.MinValue ? durationRecording.ResponseSent - LastResponseSent : TimeSpan.FromMilliseconds(0);

                    _durations.Add((timeSinceLastRequest.Ticks, durationRecording.durationInTicks));
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

                    LastResponseSent = durationRecording.ResponseSent;

                    // ToDo add short term and long term windows and calculate trends?

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing durationRecording.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StatisticsQueuedHostedService is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
