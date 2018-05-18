using System;
using static Storage.Data.LevelDB.NativeWrapper;

namespace Storage.Data.LevelDB
{
    public class Snapshot
    {
        public IntPtr Db, Handle;

        internal Snapshot(IntPtr db)
        {
            Db = db;
            Handle = leveldb_create_snapshot(db);
        }

        public void Dispose()
        {
            if (Handle == IntPtr.Zero) return;

            leveldb_release_snapshot(Db, Handle);
            Handle = IntPtr.Zero;
        }
    }
}
