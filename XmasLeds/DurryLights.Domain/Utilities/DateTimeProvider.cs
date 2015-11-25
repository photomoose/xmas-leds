using System;

namespace Rumr.DurryLights.Domain.Utilities
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}