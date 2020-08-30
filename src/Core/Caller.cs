using System;
using System.Collections.Generic;
using System.Text;

namespace Arbee.StructuredLogging.Core
{
    /// <summary>
    /// Represents information about the member that requested logging.
    /// </summary>
    public class Caller
    {
        /// <summary>
        /// The member that logged.
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// The file the member is defined in.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The line-number of the calling member.
        /// </summary>
        public int? Line { get; set; }
    }
}
