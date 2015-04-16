using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;

namespace SneezeDetector
{
    public static class NTP
    {
        public static bool UpdateTimeFromNTPServer(string server)
        {
            try
            {
                DateTime currentTime = NTP.GetNTPTime(server);
                Microsoft.SPOT.Hardware.Utility.SetLocalTime(currentTime);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Get DateTime from NTP Server 
        /// Based on:
        /// http://weblogs.asp.net/mschwarz/archive/2008/03/09/wrong-datetime-on-net-micro-framework-devices.aspx
        /// </summary>
        /// <param name="TimeServer">TimeServer</param>
        /// <returns>Local NTP Time</returns>
        private static DateTime GetNTPTime(String TimeServer)
        {
            // Find endpoint for TimeServer
            IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(TimeServer).AddressList[0], 123);

            // Make send/receive buffer
            byte[] ntpData = new byte[48];

            // Connect to TimeServer
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                // Set 10s send/receive timeout and connect
                s.SendTimeout = s.ReceiveTimeout = 10000; // 10,000 ms
                s.Connect(ep);

                // Set protocol version
                ntpData[0] = 0x1B;

                // Send Request
                s.Send(ntpData);

                // Receive Time
                s.Receive(ntpData);

                // Close the socket
                s.Close();
            }

            byte offsetTransmitTime = 40;

            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
                intpart = (intpart << 8) | ntpData[offsetTransmitTime + i];

            for (int i = 4; i <= 7; i++)
                fractpart = (fractpart << 8) | ntpData[offsetTransmitTime + i];

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);


            TimeSpan TimeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            DateTime dateTime = new DateTime(1900, 1, 1);
            dateTime += TimeSpan;

            TimeSpan offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            DateTime networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }
    }
}
