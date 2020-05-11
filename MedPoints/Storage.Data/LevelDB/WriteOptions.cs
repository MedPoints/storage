using System;

namespace Storage.Data.LevelDB
{
    class WriteOptions
    {
        public static readonly WriteOptions Default = new WriteOptions();
        internal readonly IntPtr handle = NativeWrapper.leveldb_writeoptions_create();

        public bool Sync
        {
            set => NativeWrapper.leveldb_writeoptions_set_sync(handle, value);
        }

        ~WriteOptions()
        {
            NativeWrapper.leveldb_writeoptions_destroy(handle);
        }
    }
}
