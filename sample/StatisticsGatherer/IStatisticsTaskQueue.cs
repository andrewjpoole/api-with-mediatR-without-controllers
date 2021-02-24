﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace mediatr_test.StatisticsGatherer
{
    public interface IStatisticsTaskQueue
    {
        void QueueStatisticsWorkItem((DateTime ResponseSent, long durationInTicks) durationRecording);

        Task<(DateTime ResponseSent, long durationInTicks)> DequeueAsync(CancellationToken cancellationToken);
    }
}
