// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Connectors.Xpo;

using System;

public static class DateTimeOffsetConversionExtensions
{
    /// <summary>
    /// Converts a DateTime to DateTimeOffset, handling different DateTimeKinds.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <returns>A DateTimeOffset based on the DateTimeKind of the input.</returns>
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Utc => new DateTimeOffset(dateTime, TimeSpan.Zero), // If it's UTC, use zero offset
            DateTimeKind.Local => new DateTimeOffset(dateTime), // If it's local, use the system's local offset
            DateTimeKind.Unspecified => new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime)), // If unspecified, assume local
            _ => throw new InvalidOperationException("Unknown DateTimeKind.")
        };
    }

    /// <summary>
    /// Converts a nullable DateTime to DateTimeOffset, handling different DateTimeKinds.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to convert.</param>
    /// <returns>A nullable DateTimeOffset or null if input is null.</returns>
    public static DateTimeOffset? ToDateTimeOffset(this DateTime? dateTime)
    {
        return dateTime.HasValue ? dateTime.Value.ToDateTimeOffset() : (DateTimeOffset?)null;
    }

    /// <summary>
    /// Converts a DateTimeOffset to DateTime, assuming the DateTime will be converted to local time.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset to convert.</param>
    /// <returns>A DateTime representing local time.</returns>
    public static DateTime ToDateTime(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.LocalDateTime;
    }

    /// <summary>
    /// Converts a nullable DateTimeOffset to DateTime, assuming the DateTime will be converted to local time.
    /// </summary>
    /// <param name="dateTimeOffset">The nullable DateTimeOffset to convert.</param>
    /// <returns>A nullable DateTime representing local time, or null if input is null.</returns>
    public static DateTime? ToDateTime(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset?.LocalDateTime;
    }
}

