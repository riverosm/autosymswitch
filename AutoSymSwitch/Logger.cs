using System;
using System.IO;


namespace AutoSymSwitch
{
    public class Logger
    {
        public void WriteToFile(string Message, bool isError = false)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            string level;
            if (isError)
                level = "ERROR";
            else
                level = "INFO";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = path + "\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";


            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " - " + level + " - " + Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " - " + level + " - " + Message);
                }
            }

            Console.WriteLine(DateTime.Now + " - " + level + " - " + Message);
        }
    }
}
