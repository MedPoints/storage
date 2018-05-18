using System;
using static Storage.Data.LevelDB.NativeWrapper;

namespace Storage.Data.LevelDB
{
    class ReadOptions
    {
        public static readonly ReadOptions Default = new ReadOptions();
        internal readonly IntPtr handle = leveldb_readoptions_create();

        public bool VerifyChecksums
        {
            set => leveldb_readoptions_set_verify_checksums(handle, value);
        }

        public bool FillCache
        {
            set => leveldb_readoptions_set_fill_cache(handle, value);
        }

        public Snapshot Snapshot
        {
            set => leveldb_readoptions_set_snapshot(handle, value.Handle);
        }

        ~ReadOptions()
        {
            leveldb_readoptions_destroy(handle);
        }
    }
}
