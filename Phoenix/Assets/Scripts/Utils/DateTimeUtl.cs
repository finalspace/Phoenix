using System;
using UnityEngine;
using System.Collections;

public class DateTimeUtil
{
    public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnixTime(long unixTime)
    {
        return epoch.AddSeconds(unixTime);
    }

    public static DateTime FromUnitTimeMilliseconds(long unixTimeMilliseconds)
    {
        return epoch.AddMilliseconds(unixTimeMilliseconds);
    }

    public static int MillisecondsElapse(long unixTime)
    {
        return (int)(DateTime.UtcNow - FromUnixTime(unixTime)).TotalMilliseconds;
    }

    public static int MillisecondsElapseFromMilliseconds(long unixTimeMilliseconds)
    {
        return (int)(DateTime.UtcNow - FromUnitTimeMilliseconds(unixTimeMilliseconds)).TotalMilliseconds;
    }

    public static int SecondsElapse(long unixTime)
    {
        return (int)(DateTime.UtcNow - FromUnixTime(unixTime)).TotalSeconds;
    }

    public static int MillisecondsLeft(long unixTime)
    {
        return (int)(FromUnixTime(unixTime) - DateTime.UtcNow).TotalMilliseconds;
    }

    public static int GetUnixTime()
    {
        return (int)(DateTime.UtcNow - epoch).TotalSeconds;
    }

    public static long GetUnixTimeMilliseconds()
    {
        return (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
    }

}