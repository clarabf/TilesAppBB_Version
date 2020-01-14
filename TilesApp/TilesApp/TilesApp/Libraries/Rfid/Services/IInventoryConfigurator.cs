
namespace TilesApp.Rfid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a method to configure a reader to match a <see cref="Models.InventoryConfiguration"/>
    /// </summary>
    public interface IInventoryConfigurator
    {
        /// <summary>
        /// Configures Inventory operations to respect the current configuration parameters
        /// </summary>
        /// <returns>
        /// The task to configure the reader
        /// </returns>
        Task ConfigureAsync();
    }
}
