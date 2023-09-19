using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class LogManager
    {
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
        /// The error counter
        /// </summary>
        public int error_counter {  get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            ClearCoordsLog();
            ClearPlacementLog();
            this.error_counter = 0;
        }

        /*
        Menu :

        0 - Info 
        1 - Debug
        2 - Error
         */

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
                            logTextInfoPlacementCoordinates.Append(comment);
                        }
                        else if(categoty == "placement")
                        {
                            logTextInfoPlacementCoordinates.Append(comment);
                        }
                        break;
                    case 1:
                        if (categoty == "coords")
                        {
                            logTextDebugPlacementCoordinates.Append(comment);
                        }
                        else if (categoty == "placement")
                        {
                            logTextDebugPlacementReport.Append(comment);
                        }
                        break;
                    case 2:
                        if (categoty == "coords")
                        {
                            logTextErrorPlacementCoordinates.Append(comment);
                            this.error_counter++;
                        }
                        else if (categoty == "placement")
                        {
                            logTextErrorPlacementReport.Append(comment);
                            this.error_counter++;
                        }
                        break;
                }
            }
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
