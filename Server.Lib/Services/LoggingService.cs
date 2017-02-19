using System;

namespace Server.Lib.Services
{
    class LoggingService : ILoggingService
    {
        public void Info(string str, params object[] strFormat)
        {
            Console.WriteLine("INFO - {0}", string.Format(str, strFormat));
        }

        public void Error(string str, params object[] strFormat)
        {
            Console.WriteLine("ERROR - {0}", string.Format(str, strFormat));
        }

        public void Exception(Exception ex, string str, params object[] strFormat)
        {
            Console.WriteLine("EXCEPTION - {0} - {1}", string.Format(str, strFormat), ex.ToString());
        }
    }
}