using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Rfid
{
    /// <summary>
    /// When implemented by a view provides methods to a ViewModel that are called for view events
    /// </summary>
    public interface ILifecycle
    {
        /// <summary>
        /// Called when the view is displayed
        /// </summary>
        void Shown();

        /// <summary>
        /// Called when the view is hidden
        /// </summary>
        void Hidden();
    }
}
