using System.Data.Common;

namespace Storage.Data.LevelDB
{
    internal class LevelDbException : DbException
    {
        internal LevelDbException(string message)
            : base(message)
        {
        }
    }
}
