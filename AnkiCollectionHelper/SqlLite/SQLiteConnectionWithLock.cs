using System;
using System.Threading;

namespace AnkiCollectionHelper.SqlLite
{
    class SQLiteConnectionWithLock : SQLiteConnection
    {
        readonly object _lockPoint = new object ();

        public SQLiteConnectionWithLock (SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
            : base (connectionString.DatabasePath, openFlags, connectionString.StoreDateTimeAsTicks)
        {
        }

        public IDisposable Lock ()
        {
            return new LockWrapper (_lockPoint);
        }

        private class LockWrapper : IDisposable
        {
            object _lockPoint;

            public LockWrapper (object lockPoint)
            {
                _lockPoint = lockPoint;
                Monitor.Enter (_lockPoint);
            }

            public void Dispose ()
            {
                Monitor.Exit (_lockPoint);
            }
        }
    }
}