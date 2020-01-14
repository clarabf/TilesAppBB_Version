
namespace TilesApp.Rfid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using TechnologySolutions.Rfid.AsciiProtocol;

    /// <summary>
    /// Notifies when a barcode is received
    /// </summary>
    public interface IMonitorBarcode
    {
        /// <summary>
        /// Raised when a barcode scan is received from the reader
        /// </summary>
        event EventHandler<BarcodeEventArgs> BarcodeScanned;

        /// <summary>
        /// Gets or sets a value indicating whether barcodes should be reported
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
