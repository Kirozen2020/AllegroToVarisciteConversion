using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    static class LogManager
    {
        public enum LogLevel : int
        {
            Emergency = 0,      //System is unusable
            Alert = 1,          //Action must be taken immediately
            Critical = 2,       //Critical conditions
            Error = 3,          //Error conditions
            Warning = 4,        //Warning conditions
            Notice = 5,         //Normal but significant condition
            Informational = 6,  //Informational messages
            Debug = 7,          //Debug-level messages
        }

        private static StringBuilder logBody;
        private static int currentLogLevel;
        private static int currentErrorLevel;
        private static int errorCount;

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        /// <param name="requestedLogLevel">Minimal Log Level to be traced.</param>
        /// <param name="requestedErrorLevel">Minimal Log Level that counts as error.</param>
        public static void Init(LogLevel requestedLogLevel, LogLevel requestedErrorLevel)
        {
            logBody = new StringBuilder();
            currentLogLevel = (int)requestedLogLevel;
            currentErrorLevel = (int)requestedErrorLevel;
            errorCount = 0;
        }

        /*----------------- Help functions ------------------*/

        public static void AddComment(LogLevel eventLogLevel, string line)
        {
            if ((int)eventLogLevel <= currentLogLevel)
                logBody.Append(line);
            if ((int)eventLogLevel <= currentErrorLevel)
                errorCount++;
        }
        public static void AddCommentLine(LogLevel eventLogLevel, string line)
        {
            if ((int)eventLogLevel <= currentLogLevel)
                logBody.AppendLine(line);
            if ((int)eventLogLevel <= currentErrorLevel)
                errorCount++;
        }

        public static bool SaveLogToFile(string filename)
        {
            AddCommentLine(LogLevel.Emergency, "Errors count: " + errorCount.ToString());
            try
            {
                System.IO.File.WriteAllText(filename, logBody.ToString());
                AddCommentLine(LogLevel.Emergency, "File saved at: " + filename);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public static int GetCurrentErrorCount()
        {
            return errorCount;
        }
    }
}
