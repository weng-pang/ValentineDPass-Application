using System;
using System.Collections.Generic;
using System.Text;

namespace AttLogs
{
    class Connector
    {
        //Create Standalone SDK class dynamically.
        private zkemkeeper.CZKEMClass ATTENDANCE = new zkemkeeper.CZKEMClass();
        private bool connected = false;
        private static bool objectUsed = false;

        public Connector()
        {
            if (!objectUsed)
            {
                objectUsed = true;
            }
            else
            {
                throw  new NotSupportedException("Only a singel connection supported.");
            }
        }

        public bool isConnected()
        {  
            return connected;
        }

        public zkemkeeper.CZKEMClass getAttendance()
        {
            return ATTENDANCE;
        }

        public bool connect(string ipAddress, string port)
        {
            // Connection procedure
            connected = ATTENDANCE.Connect_Net(ipAddress, Convert.ToInt32(port));
            return connected;
            // This particular part may spend a lot of time...
        }

        public void disconnect()
        {
            ATTENDANCE.Disconnect();
            connected = false;
        }
    }
}
