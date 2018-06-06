using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Data.Blockchain
{
    public class DataEntryPrefix
    {
        public const byte DATA_Block = 0x01;
        public const byte DATA_Transaction = 0x02;

        public const byte SYS_CurrentBlock = 0xc0;
        public const byte SYS_CurrentHeader = 0xc1;
        public const byte SYS_Version = 0xf0;
    }
}
