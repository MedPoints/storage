using System;
using static Storage.Data.LevelDB.NativeWrapper;

namespace Storage.Data.LevelDB
{
    public class Options
    {
        public static readonly Options Default = new Options();
        internal readonly IntPtr Handle = leveldb_options_create();

        public bool CreateIfMissing
        {
            set => leveldb_options_set_create_if_missing(Handle, value);
        }

        public bool ErrorIfExists
        {
            set => leveldb_options_set_error_if_exists(Handle, value);
        }

        public bool ParanoidChecks
        {
            set => leveldb_options_set_paranoid_checks(Handle, value);
        }

        public int WriteBufferSize
        {
            set => leveldb_options_set_write_buffer_size(Handle, (UIntPtr)value);
        }

        public int MaxOpenFiles
        {
            set => leveldb_options_set_max_open_files(Handle, value);
        }

        public int BlockSize
        {
            set => leveldb_options_set_block_size(Handle, (UIntPtr)value);
        }

        public int BlockRestartInterval
        {
            set => leveldb_options_set_block_restart_interval(Handle, value);
        }

        public CompressionType Compression
        {
            set => leveldb_options_set_compression(Handle, value);
        }

        public IntPtr FilterPolicy
        {
            set => leveldb_options_set_filter_policy(Handle, value);
        }

        ~Options()
        {
            leveldb_options_destroy(Handle);
        }
    }
}
