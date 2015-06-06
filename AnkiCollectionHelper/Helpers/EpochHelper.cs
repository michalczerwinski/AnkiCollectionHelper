using System;

namespace AnkiCollectionHelper.Helpers
{
    public static class EpochHelper
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromEpochTime(this long unixTime)
        {
            return _epoch.AddSeconds(unixTime);
        }

        public static long ToEpochTime(this DateTime date)
        {
            return Convert.ToInt64((date.ToUniversalTime() - _epoch).TotalSeconds);
        }
    }
}