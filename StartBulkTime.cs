using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration
{
    class StartBulkTime
    {
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public bool isDone { get; set; }

        public StartBulkTime(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
            isDone = false;
        }
    }
}
