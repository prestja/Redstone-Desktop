using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prestja
{
    public static class PacketUtils
    {
        private static int requestID = 0;
        public static int nextRequestID
        {
            get
            {
                return requestID += 1;
            }
        }
    }
}
