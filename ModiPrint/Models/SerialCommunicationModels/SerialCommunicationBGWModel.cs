using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ModiPrint.Models.SerialCommunicationModels.ReportingModels;
using ModiPrint.ViewModels.SerialCommunicationViewModels;

//To Do: This should solely be a class to implement the background worker. There is too much application logic here.
namespace ModiPrint.Models.SerialCommunicationModels
{
    //Event handler for the termination of the Serial Communication background thread.
    public delegate void SerialCommunicationBGWTerminatedEventHandler(object sender);
    
    /// <summary>
    /// Manages the thread that runs the serial port.
    /// </summary>
    public class SerialCommunicationBGWModel
    {
        #region Fields and Properties
        //Manages functionality related to serial communication.
        private SerialCommunicationMainModel _serialCommunicationMainModel;

        //All serial communication is run on a separate thread.
        private BackgroundWorker _bGWSerialCommunication;

        //Event for the termination of the Serial Communication background thread.
        public event SerialCommunicationBGWTerminatedEventHandler SerialCommunicationBGWTerminated;
        private void OnSerialCommunicationBGWTerminated()
        {
            if (SerialCommunicationBGWTerminated != null)
            { SerialCommunicationBGWTerminated(this); }
        }
        #endregion

        #region Contructor
        public SerialCommunicationBGWModel(SerialCommunicationMainModel SerialCommunicationMainModel)
        {
            _serialCommunicationMainModel = SerialCommunicationMainModel;

            _bGWSerialCommunication = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bGWSerialCommunication.DoWork += new DoWorkEventHandler(BGWSerialCommunication_DoWork);
            _bGWSerialCommunication.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWSerialCommunication_RunWorkerCompleted);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the background worker that manages serial communication.
        /// </summary>
        public void StartConnection(string portName)
        {
            if (_bGWSerialCommunication.IsBusy == false)
            {
                _bGWSerialCommunication.RunWorkerAsync(portName);
            }
        }

        /// <summary>
        /// Stops the background worker that manages serial communication.
        /// Disconnects the serial port.
        /// </summary>
        public void EndConnection()
        {
            //Disconnect the serial port before the thread close.
            //This is to let the program complete its last command before termination.
            _serialCommunicationMainModel.ShouldDisconnect = true;
            while(_serialCommunicationMainModel.IsPortOpen() == false) { }
            _serialCommunicationMainModel.ShouldDisconnect = false;
            _bGWSerialCommunication.CancelAsync();
        }

        /// <summary>
        /// Executes the background thread that manages serial communication.
        /// Checks the connection and 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGWSerialCommunication_DoWork(object sender, DoWorkEventArgs e)
        {
            //Start the serial connection.
            _serialCommunicationMainModel.SerialConnect((string)e.Argument);
        }

        /// <summary>
        /// Executes upon closing of the serial communication background thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BGWSerialCommunication_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnSerialCommunicationBGWTerminated();

            if (e.Cancelled)
            {
                //The thread was closed properly.
            }
            else if (e.Error != null)
            {
                // To Do: Error: There was an unexpected termination during the operation.
            }
            else
            {
                // To Do: Error: The operation completed its cycle...which shouldn't happen.
            }
        }
        #endregion
    }
}
