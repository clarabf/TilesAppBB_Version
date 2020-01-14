
namespace TilesApp.Rfid.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Services;
    using TechnologySolutions.Rfid;

    /// <summary>
    /// ViewModel for the ReadWriteView
    /// </summary>
    public class ReadWriteViewModel
        : ViewModelBase, ILifecycle
    {
        /// <summary>
        /// The instance used to read tag
        /// </summary>
        private readonly ITagReader tagReader;

        /// <summary>
        /// The instance used to write tag
        /// </summary>
        private readonly ITagWriter tagWriter;

        private IProgress<string> reportMessage;
        private IProgress<bool> reportReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadWriteViewModel"/> class.
        /// </summary>
        /// <param name="tagReader">used to read a tag</param>
        /// <param name="tagWriter">used to write a tag</param>
        /// <param name="dispatcher">used to dispatch to the UI thread</param>
        public ReadWriteViewModel(ITagReader tagReader, ITagWriter tagWriter)
        {
            this.reportMessage = new Progress<string>(this.AppendMessageToList);
            this.reportReader = new Progress<bool>(this.ReaderChanged);

            this.tagReader = tagReader;
            this.tagReader.ProgressUpdate += (sende, e) => { this.reportMessage.Report(e.Message); };

            this.tagWriter = tagWriter;

            // The operations reader/writer implements both interfaces so we only to register for updates once
            if (!object.ReferenceEquals(this.tagReader, this.tagWriter))
            {
                this.tagWriter.ProgressUpdate += (sende, e) => { this.reportMessage.Report(e.Message); };
            }

            this.ReadTagCommand = new RelayCommand(this.ExecuteReadTag, () => { return this.isIdle; });
            this.ClearCommand = new RelayCommand(this.ExecuteClear);
            this.WriteTagCommand = new RelayCommand(this.ExecuteWriteTag, this.CanExecuteWriteTag);

            //this.readerChangeNotication = new ReaderChangeNotification(this.ReaderConnected, () => { });

            this.HexIdentifier = string.Empty;
            this.SelectedMemoryBank = TechnologySolutions.Rfid.MemoryBank.Epc; // 1;
            this.WordAddress = 2;
            this.WordCount = 2;
            this.MinimumPower = 10;
            this.OutputPower = this.MaximumPower = 30;            

            this.IsIdle = true;
        }

        /// <summary>
        /// Gets the command to read the tag
        /// </summary>
        public ICommand ReadTagCommand { get; private set; }

        /// <summary>
        /// Gets the command to read the tag
        /// </summary>
        public ICommand ClearCommand { get; private set; }

        /// <summary>
        /// Gets the command to read the tag
        /// </summary>
        public ICommand WriteTagCommand { get; private set; }

        #region HexData
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public string HexData
        {
            get
            {
                return this.hexData;
            }

            set
            {
                this.Set(ref this.hexData, value);
                (this.WriteTagCommand as RelayCommand).RaiseCanExecuteChanged();
            }
        }        

        /// <summary>
        /// Backing store for the HexData property
        /// </summary>
        private string hexData;
        #endregion

        #region HexIdentifier
        /// <summary>
        /// Gets or sets the new EPC filter
        /// </summary>
        public string HexIdentifier
        {
            get
            {
                return this.hexIdentifier;
            }

            set
            {
                this.Set(ref this.hexIdentifier, value);
            }
        }        

        /// <summary>
        /// Backing store for the HexIdentifier property
        /// </summary>
        private string hexIdentifier;
        #endregion

        #region IsIdle
        /// <summary>
        /// Gets or sets a value indicating whether a long running task is not executing
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return this.isIdle;
            }

            set
            {
                this.Set(ref this.isIdle, value);
            }
        }

        /// <summary>
        /// Backing store for IsBusy property
        /// </summary>
        private bool isIdle;
        #endregion

        #region MemoryBank
        /// <summary>
        /// Gets or sets the memoryBank
        /// </summary>
        public int MemoryBankIndex
        {
            get
            {
                return this.memoryBankIndex;
            }

            set
            {
                if (this.Set(ref this.memoryBankIndex, value))
                {
                    this.RaisePropertyChanged("SelectedMemoryBank");
                }
            }
        }

        /// <summary>
        /// Backing store for the MemoryBank property
        /// </summary>
        private int memoryBankIndex;

        public MemoryBank SelectedMemoryBank
        {
            get
            {
                return (MemoryBank)this.MemoryBankIndex;
            }

            set
            {
                this.MemoryBankIndex = (int)value;
            }
        }        

        /// <summary>
        /// Gets the names of the memory banks sorted by enumeration order
        /// </summary>
        public List<string> MemeoryBanks
        {
            get
            {                
                return Enum.GetValues(typeof(MemoryBank))
                    .Cast<MemoryBank>()
                    .OrderBy(x => (int)x)
                    .Select(x => x.ToString())
                    .ToList();
            }
        }
        #endregion

        #region WordAddress
        /// <summary>
        /// Gets or sets the wordAddress
        /// </summary>
        public int WordAddress
        {
            get
            {
                return this.wordAddress;
            }

            set
            {
                this.Set(ref this.wordAddress, (int)value);
            }
        }

        /// <summary>
        /// Backing store for the WordAddress property
        /// </summary>
        private int wordAddress;
        #endregion

        #region WordCount
        /// <summary>
        /// Gets or sets the wordCount
        /// </summary>
        public int WordCount
        {
            get
            {
                return this.wordCount;
            }

            set
            {
                this.Set(ref this.wordCount, (int)value);
            }
        }

        /// <summary>
        /// Backing store for the WordCount property
        /// </summary>
        private int wordCount;
        #endregion

        #region MaximumPower
        /// <summary>
        /// Gets or sets the maximumPower
        /// </summary>
        public int MaximumPower
        {
            get
            {
                return this.maximumPower;
            }

            set
            {
                this.Set(ref this.maximumPower, value);
            }
        }

        /// <summary>
        /// Backing store for the MaximumPower property
        /// </summary>
        private int maximumPower;
        #endregion

        #region MinimumPower
        /// <summary>
        /// Gets or sets the minimumPower
        /// </summary>
        public int MinimumPower
        {
            get
            {
                return this.minimumPower;
            }

            set
            {
                this.Set(ref this.minimumPower, value);
            }
        }

        /// <summary>
        /// Backing store for the MinimumPower property
        /// </summary>
        private int minimumPower;
        #endregion

        #region OutputPower
        /// <summary>
        /// Gets or sets the outputPower
        /// </summary>
        public int OutputPower
        {
            get
            {
                return this.outputPower;
            }

            set
            {
                this.Set(ref this.outputPower, value);
            }
        }

        /// <summary>
        /// Backing store for the OutputPower property
        /// </summary>
        private int outputPower;
        #endregion

        /// <summary>
        /// Gets or sets the progress messages
        /// </summary>
        public ObservableCollection<string> Messages { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Life cycle event when view is hidden
        /// </summary>
        public void Hidden()
        {
        }

        /// <summary>
        /// Life cycle event when view is shown
        /// </summary>
        public void Shown()
        {
            //
            this.OutputPower = Math.Max(Math.Min(this.OutputPower, this.tagReader.MaximumOutputPower), this.tagReader.MinimumOutputPower);
            this.MinimumPower = this.tagReader.MinimumOutputPower;
            this.MaximumPower = this.tagReader.MaximumOutputPower;            
        }

        /// <summary>
        /// Performs the <see cref="ReadTagCommand"/>
        /// </summary>
        private async void ExecuteReadTag()
        {
            this.IsIdle = false;
            this.ReadTagCommand.RefreshCanExecute();
            this.WriteTagCommand.RefreshCanExecute();

            this.Messages.Clear();

            try
            {
                this.AppendMessage("Scanning for tags...");
                // Execute the potentially long running task off the UI thread
                int count = await this.tagReader.ReadTagsAsync(this.HexIdentifier, this.SelectedMemoryBank, this.WordAddress, this.WordCount, this.OutputPower);
                this.AppendMessage(string.Format("Transponders seen = {0}", count));
                this.AppendMessage("Done.");
            }
            catch (Exception ex)
            {
                this.AppendMessage(ex.Message);
            }

            
            //await Task.Run(() =>
            //{
            //    try
            //    {
            //        if (this.tagReader.ReadTags(this.HexIdentifier.ToLower(), this.SelectedMemoryBank, this.WordAddress, this.WordCount, this.OutputPower))
            //        {
            //            this.AppendMessage("Done.");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.AppendMessage(ex.Message);
            //    }
            //});

            this.IsIdle = true;
            this.ReadTagCommand.RefreshCanExecute();
            this.WriteTagCommand.RefreshCanExecute();
        }

        /// <summary>
        /// Performs the <see cref="ClearCommand"/>
        /// </summary>
        private void ExecuteClear()
        {
            this.Messages.Clear();
            this.AppendMessage("Cleared...");
        }

        /// <summary>
        /// Determines if the WriteTag command can execute
        /// </summary>
        /// <returns>true when the EPC can be changed</returns>
        private bool CanExecuteWriteTag()
        {
            return this.IsIdle && HexValidator.IsValidWordAlignedHex(this.HexData);
        }

        /// <summary>
        /// Performs the <see cref="WriteTagCommand"/>
        /// </summary>
        private async void ExecuteWriteTag()
        {
            this.IsIdle = false;
            this.Messages.Clear();

            // Execute the potentially long running task off the UI thread
            try
            {
                this.AppendMessage("Updating tags...");
                int  count = await this.tagWriter.WriteTagsAsync(this.HexIdentifier, this.SelectedMemoryBank, this.WordAddress, this.WordCount, this.HexData, this.OutputPower);
                this.AppendMessage(string.Format("Transponders seen = {0}", count));
                this.AppendMessage("Done.");
            }
            catch (Exception ex)
            {
                this.AppendMessage(ex.Message);
            }

            this.IsIdle = true;
            (this.ReadTagCommand as RelayCommand).RaiseCanExecuteChanged();
        }

        ///// <summary>
        ///// Respond to progress messages 
        ///// </summary>
        ///// <param name="sender">the source of the event</param>
        ///// <param name="e">the progress update arguments</param>
        //private void ReadWrite_ProgressUpdate(object sender, ProgressEventArgs e)
        //{
        //    this.AppendMessage(e.ProgressMessage);
        //}

        ///// <summary>
        ///// Handles change in reader state
        ///// </summary>
        ///// <param name="version">an executed version command for the connected reader</param>
        //private void ReaderConnected(VersionInformationCommand version)
        //{
        //    // Set the correct slider power limits for the currently connected reader
        //    this.MaximumPower = LibraryConfiguration.Current.MaximumOutputPower;
        //    this.MinimumPower = LibraryConfiguration.Current.MinimumOutputPower;

        //    // Set the reader to the maximum allowed power
        //    this.OutputPower = this.MaximumPower;
        //}
        private void ReaderChanged(bool connected)
        {
            if (connected)
            {
                this.OutputPower = this.MaximumPower = 34; // TODO:here
                this.MinimumPower = 0; // TODO:here
            }
            else
            {
                this.OutputPower = this.MaximumPower = 29;
                this.MinimumPower = 10;
            }
        }

        /// <summary>
        /// Appends the given string to the end of the message list
        /// </summary>
        /// <param name="message">The message to append</param>
        private void AppendMessage(string message)
        {
            this.reportMessage.Report(message);
        }

        private void AppendMessageToList(string message)
        {
            while (this.Messages.Count > 50)
            {
                this.Messages.RemoveAt(0);
            }

            this.Messages.Add(message);
        }
    }
}
