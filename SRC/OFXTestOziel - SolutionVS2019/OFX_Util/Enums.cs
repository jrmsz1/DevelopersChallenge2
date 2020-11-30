using System;

namespace OFX_UTIL
{
    public static class Enums
    {

        public enum TPTRANSACTION
        {
            Credit = 0,
            Debit = 1
        }

        [Flags]
        public enum DAYS
        {
            MONDAY = 0b_0000_0000,   // 0
            TUESDAY = 0b_0000_0001,  // 1
            WESDNESDAY = 0b_0000_0010,    // 2
            THURSDAY = 0b_0000_0100,   // 4
            FRIDAY = 0b_0000_1000,   // 8
            SATURDAY = 0b_0001_0000,    // 16
            SUNDAY = 0b_0010_0000,   // 32
            WEEKEND = SATURDAY | SUNDAY,  // 64
        }

        public enum ORIGINS
        {
            CASH = 1,
            TRANSFER = 2,
            OFX_FILE = 3
        }


    }
}
