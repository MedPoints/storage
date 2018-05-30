using System;

namespace Storage.Core.Block.Header
{
    public class Header
    {
        public int Version { get; set; }
        public string PreviousHash { get; set; }
        public DateTime Time { get; set; }
    }
}
