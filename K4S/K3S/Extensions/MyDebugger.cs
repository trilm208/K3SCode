using System;

namespace Extensions
{
    public static class MyDebugger
    {
        public static void WriteLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now+ " :" + message);
        }
    }
}
