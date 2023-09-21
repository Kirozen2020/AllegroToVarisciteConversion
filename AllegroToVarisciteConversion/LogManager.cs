using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class LogManager
    {
        /*----------------- Variables ------------------*/

        /// <summary>
        /// The log text
        /// </summary>
        private StringBuilder logTextInfoPlacementCoordinates;
        /// <summary>
        /// The log text information placement report
        /// </summary>
        private StringBuilder logTextInfoPlacementReport;
        /// <summary>
        /// The log text debug
        /// </summary>
        private StringBuilder logTextDebugPlacementCoordinates;
        /// <summary>
        /// The log text debug placement report
        /// </summary>
        private StringBuilder logTextDebugPlacementReport;
        /// <summary>
        /// The log text error
        /// </summary>
        private StringBuilder logTextErrorPlacementCoordinates;
        /// <summary>
        /// The log text error placement report
        /// </summary>
        private StringBuilder logTextErrorPlacementReport;
        /// <summary>
        /// Gets or sets the global log.
        /// </summary>
        /// <value>
        /// The global log.
        /// </value>
        private StringBuilder global_log { get; set; }
        /// <summary>
        /// The error counter
        /// </summary>
        public int error_counter {  get; set; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            ClearCoordsLog();
            ClearPlacementLog();
            this.error_counter = 0;
        }

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="categoty">The categoty.</param>
        public void AddComment(string comment, List<int> id, string categoty)
        {
            for (int i = 0; i < id.Count; i++)
            {
                switch (id[i])
                {
                    case 0:
                        if(categoty == "coords")
                        {
                            logTextInfoPlacementCoordinates.AppendLine(comment);
                        }
                        else if(categoty == "placement")
                        {
                            logTextInfoPlacementReport.AppendLine(comment);
                        }
                        break;
                    case 1:
                        if (categoty == "coords")
                        {
                            logTextDebugPlacementCoordinates.AppendLine(comment);
                        }
                        else if (categoty == "placement")
                        {
                            logTextDebugPlacementReport.AppendLine(comment);
                        }
                        break;
                    case 2:
                        if (categoty == "coords")
                        {
                            logTextErrorPlacementCoordinates.AppendLine(comment);
                            this.error_counter++;
                        }
                        else if (categoty == "placement")
                        {
                            logTextErrorPlacementReport.AppendLine(comment);
                            this.error_counter++;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Gets the full log message.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="outputString">The output string.</param>
        /// <returns></returns>
        public StringBuilder GetFullLogMessage(int id, string outputString)
        {
            this.global_log = new StringBuilder();
            switch (id)
            {
                case 0:
                    this.global_log.Append(this.logTextInfoPlacementCoordinates.ToString());
                    this.global_log.AppendLine();
                    this.global_log.Append(this.logTextInfoPlacementReport.ToString());
                    this.global_log.AppendLine("\nErrors count: " + this.error_counter);
                    this.global_log.AppendLine("\nFile saved at " + outputString);
                    break;
                case 1:
                    this.global_log.Append(this.logTextDebugPlacementCoordinates.ToString());
                    this.global_log.AppendLine();
                    this.global_log.Append(this.logTextDebugPlacementReport.ToString());
                    this.global_log.AppendLine("\nErrors count: " + this.error_counter);
                    this.global_log.AppendLine("\nFile saved at " + outputString);
                    break;
                case 2:
                    this.global_log.Append(this.logTextErrorPlacementCoordinates.ToString());
                    this.global_log.Append(this.logTextErrorPlacementReport.ToString());
                    this.global_log.AppendLine("\nErrors count: " + this.error_counter);
                    this.global_log.AppendLine("\nFile saved at " + outputString);
                    break;
            }
            return this.global_log;
        }
        /// <summary>
        /// Clears the placement log.
        /// </summary>
        public void ClearPlacementLog()
        {
            this.logTextInfoPlacementReport = new StringBuilder();
            this.logTextDebugPlacementReport = new StringBuilder();
            this.logTextErrorPlacementReport = new StringBuilder();
        }
        /// <summary>
        /// Clears the coords log.
        /// </summary>
        public void ClearCoordsLog()
        {
            this.logTextInfoPlacementCoordinates = new StringBuilder();
            this.logTextErrorPlacementCoordinates = new StringBuilder();
            this.logTextDebugPlacementCoordinates = new StringBuilder();
        }
    }
}
