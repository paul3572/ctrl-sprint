using cts.core.svc.application.Abstractions.Time;

namespace cts.core.svc.infrastructure.Time;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}