using System;
using static Storage.Data.LevelDB.NativeWrapper;

namespace Storage.Data.LevelDB
{
    public class WriteBatch
    {
        internal readonly IntPtr Handle = leveldb_writebatch_create();

        ~WriteBatch()
        {
            leveldb_writebatch_destroy(Handle);
        }

        public void Clear()
        {
            leveldb_writebatch_clear(Handle);
        }

        public void Delete(Slice key)
        {
            leveldb_writebatch_delete(Handle, key.buffer, (UIntPtr)key.buffer.Length);
        }

        public void Put(Slice key, Slice value)
        {
            leveldb_writebatch_put(Handle, key.buffer, (UIntPtr)key.buffer.Length, value.buffer, (UIntPtr)value.buffer.Length);
        }
    }
}
