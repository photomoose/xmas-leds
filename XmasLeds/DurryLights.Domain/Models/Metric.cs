using System;
using Rumr.DurryLights.Domain.Commands;

namespace Rumr.DurryLights.Domain.Models
{
    public class Metric
    {
        public DateTime TimestampUtc { get; set; }
        public string RequestText { get; set; }
        public string Source { get; set; }
        public string From { get; set; }
        public string DisplayType { get; set; }
        public LightDisplay Command { get; set; }
        public DateTime? ScheduledAtUtc { get; set; }
        public double? ScheduledDelaySecs { get; set; }
        public bool IsValidCommand { get; set; }
        public bool IsScheduled { get; set; }
    }
}