using System;

namespace Rumr.DurryLights.Domain.Models
{
    public class LightsResponse
    {
        public bool IsScheduled { get; set; }
        public DateTime? ScheduledForUtc { get; set; }
        public bool IsSuccess { get; set; }

        private LightsResponse()
        {
        }

        public static LightsResponse Scheduled(DateTime scheduledForUtc)
        {
            return new LightsResponse
            {
                IsSuccess = true,
                IsScheduled = true,
                ScheduledForUtc = scheduledForUtc
            };
        }

        public static LightsResponse Success()
        {
            return new LightsResponse
            {
                IsSuccess = true
            };
        }

        public static LightsResponse Failure()
        {
            return new LightsResponse
            {
                IsSuccess = false
            };
        }
    }
}