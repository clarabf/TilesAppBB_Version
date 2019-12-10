
namespace TilesApp.Rfid.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Services;

    /// <summary>
    /// View model for the inventory of transponders and barcodes
    /// </summary>
    public class BarcodeInventory
    {
        /// <summary>
        /// Used to invoke actions on the user interface thread
        /// </summary>
        private IProgress<string> dispatcher;

        /// <summary>
        /// Initializes a new instance of the BarcodeInventory class
        /// </summary>
        /// <param name="barcode">Reports as barcodes are scanned</param>
        public BarcodeInventory(IMonitorBarcode barcode)
        {
            this.dispatcher = new Progress<string>(this.AddBarcode);

            barcode.BarcodeScanned += (sender, e) => { this.dispatcher.Report(e.Barcode); };

            this.Identifiers = new ObservableCollection<IdentifiedItem>();
        }

        /// <summary>
        /// Gets the barcodes scanned
        /// </summary>
        public ObservableCollection<IdentifiedItem> Identifiers { get; private set; }

        /// <summary>
        /// Add a barcode to the list of barcodes
        /// </summary>
        /// <param name="identifier">The barcode identifier to add</param>
        public void AddBarcode(string identifier)
        {
            IdentifiedItem item;

            item = this.Identifiers
                .Where(x => x.Identifier == identifier)
                .FirstOrDefault();

            if (item == null)
            {
                this.Identifiers.Add(item = new IdentifiedItem(identifier));
            }

            item.Seen(DateTime.Now);
        }
        /// <summary>
        /// Reset all of the statistics
        /// </summary>
        public void Clear()
        {
            this.Identifiers.Clear();
        }
    }
}
