
namespace TilesApp.Rfid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using TechnologySolutions.Rfid;

    /// <summary>
    /// Notifies the progress of a command that returns transponders
    /// </summary>
    public interface IMonitorTransponders
    {
        /// <summary>
        /// Raised for each transponder in the response
        /// </summary>
        event EventHandler<TranspondersEventArgs> TranspondersReceived;

        /// <summary>
        /// Gets or sets a value indicating whether transponders should be reported
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
