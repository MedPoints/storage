using System;
using static Storage.Data.LevelDB.NativeWrapper;

namespace Storage.Data.LevelDB
{
    public class DatabaseContext : IDisposable
    {
        private IntPtr handle;

        /// <summary>
        /// Return true if haven't got valid handle
        /// </summary>
        public bool IsDisposed => handle == IntPtr.Zero;

        private DatabaseContext(IntPtr handle)
        {
            this.handle = handle;
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                leveldb_close(handle);
                handle = IntPtr.Zero;
            }
        }

        public void Delete(WriteOptions options, Slice key)
        {
            IntPtr error;
            leveldb_delete(handle, options.handle, key.Buffer, (UIntPtr)key.Buffer.Length, out error);
            NativeHelper.CheckError(error);
        }

        public Slice Get(ReadOptions options, Slice key)
        {
            UIntPtr length;
            IntPtr error;
            IntPtr value = leveldb_get(handle, options.handle, key.Buffer, (UIntPtr)key.Buffer.Length, out length, out error);
            try
            {
                NativeHelper.CheckError(error);
                if (value == IntPtr.Zero)
                    throw new LevelDbException("not found");
                return new Slice(value, length);
            }
            finally
            {
                if (value != IntPtr.Zero) leveldb_free(value);
            }
        }

        public Snapshot GetSnapshot()
        {
            return new Snapshot(handle);
        }

        public Iterator NewIterator(ReadOptions options)
        {
            return new Iterator(leveldb_create_iterator(handle, options.handle));
        }

        public static DatabaseContext Open(string name)
        {
            return Open(name, Options.Default);
        }

        public static DatabaseContext Open(string name, Options options)
        {
            IntPtr error;
            IntPtr handle = leveldb_open(options.Handle, name, out error);
            NativeHelper.CheckError(error);
            return new DatabaseContext(handle);
        }

        public void Put(WriteOptions options, Slice key, Slice value)
        {
            IntPtr error;
            leveldb_put(handle, options.handle, key.Buffer, (UIntPtr)key.Buffer.Length, value.Buffer, (UIntPtr)value.Buffer.Length, out error);
            NativeHelper.CheckError(error);
        }

        public bool TryGet(ReadOptions options, Slice key, out Slice value)
        {
            UIntPtr length;
            IntPtr error;
            IntPtr v = leveldb_get(handle, options.handle, key.Buffer, (UIntPtr)key.Buffer.Length, out length, out error);
            if (error != IntPtr.Zero)
            {
                leveldb_free(error);
                value = default(Slice);
                return false;
            }
            if (v == IntPtr.Zero)
            {
                value = default(Slice);
                return false;
            }
            value = new Slice(v, length);
            leveldb_free(v);
            return true;
        }

        public void Write(WriteOptions options, WriteBatch write_batch)
        {
            // There's a bug in .Net Core.
            // When calling DB.Write(), it will throw LevelDbException sometimes.
            // But when you try to catch the exception, the bug disappears.
            // We shall remove the "try...catch" clause when Microsoft fix the bug.
            byte retry = 0;
            while (true)
            {
                try
                {
                    IntPtr error;
                    leveldb_write(handle, options.handle, write_batch.Handle, out error);
                    NativeHelper.CheckError(error);
                    break;
                }
                catch (LevelDbException ex)
                {
                    if (++retry >= 4) throw;
                    System.IO.File.AppendAllText("leveldb.log", ex.Message + "\r\n");
                }
            }
        }
    }
}
