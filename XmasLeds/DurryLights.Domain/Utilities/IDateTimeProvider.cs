using System;

namespace Rumr.DurryLights.Domain.Utilities
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}