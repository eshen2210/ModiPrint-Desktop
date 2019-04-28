using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.Models.SerialCommunicationModels.ReportingModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.SerialCommunicationModels
{
    //Handles event that is fired when serial connection is connected or disconnected.
    public delegate void SerialConnectionChangedEventHandler(object sender);

    //Handles event that is fired with the last Prospective Outgoing Message is sent.
    public delegate void SerialCommunicationCompletedEventHandler(object sender);

    //Event handler that's executed when a line of serial message is sent.
    public delegate void SerialCommunicationMessageSentEventHandler(object sender, SerialMessageEventArgs serialMessageEventArgs);

    //Event handler that's executed when a line of serial message is received.
    public delegate void SerialCommunicationMessageReceivedEventHandler(object sender, SerialMessageEventArgs serialMessageEventArgs);

    /// <summary>
    /// SerialConnectionModel manages the basic functionalities of System.IO.Ports.SerialPort.
    /// </summary>
    public class SerialCommunicationMainModel
    {
        #region Fields and Properties
        //Holds list of serial messages.
        private SerialCommunicationOutgoingMessagesModel _serialCommunicationOutgoingMessagesModel;

        //Contains functions to interpret and execute serial command sets.
        private SerialCommunicationCommandSetsModel _serialCommunicationCommandSetsModel;

        //Contains response information from the microcontroller.
        private RealTimeStatusDataModel _realTimeStatusDataModel;

        //SerialCommunication will pass errors to this object.
        private ErrorReporterViewModel _errorReporterViewModel;

        //Serial port where messages will be sent to and received from the microcontroller..
        private SerialPort _serialPort;

        //BaudRate in bits per second.
        public int BaudRate
        {
            get { return _serialPort.BaudRate; }
        }

        //ReadTimeout in seconds.
        private int _readTimeout;
        public int ReadTimeout
        {
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }

        //WriteTimeout in seconds.
        private int _writeTimeout;
        public int WriteTimeout
        {
            get { return _writeTimeout; }
            set { _writeTimeout = value; }
        }

        //Returns a string array of availible ports.
        public string[] PortNames
        {
            get { return SerialPort.GetPortNames(); }
        }

        //Flagged true when the serial communication protocol needs to be shut down.
        private bool _shouldDisconnect = false;
        public bool ShouldDisconnect
        {
            set { _shouldDisconnect = value; }
        }

        //Flagged true when the microcontroller is ready to receive another message.
        //Toggled when a task queued message is received or when a new message is sent.
        private bool _shouldSend = true;

        //If flagged true, send a serial message to pause hardware operations.
        private bool _shouldPause = false;
        public bool ShouldPause
        {
            set { _shouldPause = value; }
            get { return _shouldPause; }
        }

        //If flagged true, send a serial message to resume hardware operations.
        private bool _shouldResume = false;
        public bool ShouldResume
        {
            set { _shouldResume = value; }
            get { return _shouldResume; }
        }
        #endregion

        #region Events
        //Event that is fired when serial connection is connected or disconnected.
        public event SerialConnectionChangedEventHandler SerialConnectionChanged;
        private void OnSerialConnectionChanged()
        {
            if (SerialConnectionChanged != null)
            { SerialConnectionChanged(this); }
        }

        //Event that is fired with the last Prospective Outgoing Message is sent.
        public event SerialCommunicationCompletedEventHandler SerialCommunicationCompleted;
        private void OnSerialCommunicationCompleted()
        {
            if (SerialCommunicationCompleted != null)
            { SerialCommunicationCompleted(this); }
        }

        //Event that fires when serial comms sends a message.
        public event SerialCommunicationMessageSentEventHandler SerialCommunicationMessageSent;
        private void OnSerialCommunicationMessageSent(SerialMessageEventArgs serialMessageEventArgs)
        {
            if (SerialCommunicationMessageSent != null)
            { SerialCommunicationMessageSent(this, serialMessageEventArgs); }
        }

        //Event that fires after serial comms receives a message.
        public event SerialCommunicationMessageReceivedEventHandler SerialCommunicationMessageReceived;
        private void OnSerialCommunicationMessageReceived(SerialMessageEventArgs serialMessageEventArgs)
        {
            if (SerialCommunicationMessageReceived != null)
            { SerialCommunicationMessageReceived(this, serialMessageEventArgs); }
        }
        #endregion

        #region Constructors
        public SerialCommunicationMainModel(SerialCommunicationOutgoingMessagesModel SerialCommunicationOutgoingMessagesModel, SerialCommunicationCommandSetsModel SerialCommunicationCommandSetsModel, 
            RealTimeStatusDataModel RealTimeStatusDataModel, ErrorListViewModel ErrorListViewModel)
        {
            _serialCommunicationOutgoingMessagesModel = SerialCommunicationOutgoingMessagesModel;
            _serialCommunicationCommandSetsModel = SerialCommunicationCommandSetsModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _errorReporterViewModel = new ErrorReporterViewModel(ErrorListViewModel);

            InitializeSerialPort();

            _realTimeStatusDataModel.RecordLimitExecuted += new RecordLimitExecutedEventHandler(LimitSwitchHit);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes and sets parameters for the serial port.
        /// </summary>
        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort();
            _serialPort.BaudRate = GlobalValues.BaudRate;
            _serialPort.Parity = Parity.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.NewLine = SerialMessageCharacters.SerialTerminalCharacter.ToString();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceiveMessage); //Subscribe to DataReceived event handler.
        }
        
        /// <summary>
        /// Continually sends outgoing messages through the serial port.
        /// </summary>
        public void SerialCommunicate()
        {
            try
            {
                //Check that the device connected contains the appropriate functions.
                string connectionCheckMessage = SerialCommands.CheckConnection + " Connected";
                _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(connectionCheckMessage);

                //Waits until there is a message to send then sends the message.
                while (_serialPort.IsOpen == true)
                {
                    //Disconnects here if applicable.
                    if (_shouldDisconnect == true)
                    {
                        SerialDisconnect();
                        break;
                    }
                    IdleProtocol();

                    //If there is a new message that needs to be sent...
                    if ((_serialCommunicationOutgoingMessagesModel.ProspectiveOutgoingMessageList.Count > 0)
                     && (_shouldSend == true))
                    {
                        //If the message is a command set...
                        //Command sets are a set of commands that are calculated when their position in the task queue is reached.
                        if ((!String.IsNullOrWhiteSpace(_serialCommunicationOutgoingMessagesModel.ProspectiveOutgoingMessageList[0]))
                         && (_serialCommunicationOutgoingMessagesModel.ProspectiveOutgoingMessageList[0][0] == SerialMessageCharacters.SerialCommandSetCharacter))
                        {
                            //Command sets were designed to queue tasks (such as setting position values) after other tasks were complete.
                            //The parameters in command sets are usually reliant on the information provided by previous serial communications.
                            //Wait until all tasks are complete before executing command sets.
                            while (Application.Current.Dispatcher.Invoke(() =>
                                    _serialCommunicationCommandSetsModel.CanInterpret == false))
                            {
                                //Disconnects here if applicable.
                                if (_shouldDisconnect == true)
                                {
                                    SerialDisconnect();
                                    break;
                                }
                                IdleProtocol();
                            }

                            string[] commandSet = _serialCommunicationCommandSetsModel.InterpretCommandSet(_serialCommunicationOutgoingMessagesModel.RetrieveNextProspectiveOutgoingMessage());

                            if (commandSet != null)
                            {
                                for (int i = (commandSet.Length - 1); i >= 0; i--)
                                {
                                    _serialCommunicationOutgoingMessagesModel.QueueNextProspectiveOutgoingMessage(commandSet[i]);
                                }
                            }
                        }
                        //If the outgoing message is not a command set, then send it as normal.
                        else if (!String.IsNullOrWhiteSpace(_serialCommunicationOutgoingMessagesModel.ProspectiveOutgoingMessageList[0])) 
                        {                        
                            //Sends a message through the serial port.
                            string outgoingMessage;
                            outgoingMessage = _serialCommunicationOutgoingMessagesModel.RetrieveNextProspectiveOutgoingMessage();
                            SendMessage(outgoingMessage);
                        }

                        //Notify subscribers if all messages were sent.
                        if (_serialCommunicationOutgoingMessagesModel.ProspectiveOutgoingMessageList.Count == 0)
                        {
                            OnSerialCommunicationCompleted();
                        }
                    }
                }
            }
            catch when (_serialPort.IsOpen == false)
            {
                //Should never reach this point.
                _errorReporterViewModel.ReportError("Serial Communication", "Serial Port Is Not Open");
                SerialDisconnect();
            }
            catch (TimeoutException exception)
            {
                _errorReporterViewModel.ReportError("Serial Communication", "Serial Port Timeout");
                SerialDisconnect();
            }
            catch (Exception exception)
            {
                _errorReporterViewModel.ReportError("Serial Communication", exception.Message);
                SerialDisconnect();
            }
        }

        /// <summary>
        /// A series of checks to perform while the main Serial loop is idle. 
        /// </summary>
        private void IdleProtocol()
        {
            //Do not stream more messages if the system is paused.
            if (_shouldPause == true)
            {
                //Send a message to the microcontroller to stop all hardware operations.
                SendMessage(SerialMessageCharacters.SerialPauseHardwareCharacter.ToString());

                //Wait until told to resume.
                while ((_shouldPause == true) && (_shouldResume == false)) { }

                //Send a message to the microcontroller to resume all hardware operations.
                SendMessage(SerialMessageCharacters.SerialResumeHardwareCharacter.ToString());
                _shouldPause = false;
                _shouldResume = false;
            }

            //Resume if the system is paused.
            if (_shouldResume == true)
            {
                //Send a message to the microcontroller to resume all hardware operations.
                SendMessage(SerialMessageCharacters.SerialResumeHardwareCharacter.ToString());
                _shouldResume = false;
            }
        }

        /// <summary>
        /// Triggered when a serial message is received.
        /// Designates the appropriate event handler for the type of serial message received.
        /// </summary>
        /// <returns></returns>
        private void ReceiveMessage(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (_serialPort.BytesToRead != 0)
                {
                    //Reads a message coming from the serial port.
                    string incomingMessage = _serialPort.ReadLine();

                    if (!String.IsNullOrWhiteSpace(incomingMessage))
                    {
                        //If the micrcontroller has interpreted the previous command, then go ahead and send it new commands.
                        //To Do: Pause on error?
                        if ((incomingMessage[0] == SerialMessageCharacters.SerialTaskQueuedCharacter) || (incomingMessage[0] == SerialMessageCharacters.SerialErrorCharacter))
                        {
                            _shouldSend = true;
                        }

                        //Triggers the appropriate message received event.
                        SerialMessageEventArgs serialMessageEventArgs = new SerialMessageEventArgs(incomingMessage);

                        Application.Current.Dispatcher.Invoke(() =>
                        OnSerialCommunicationMessageReceived(serialMessageEventArgs));
                    }
                }
            }
            catch when (_serialPort.IsOpen == false)
            {
                //Should never reach this point.
                _errorReporterViewModel.ReportError("Serial Communication", "Serial Port Is Not Open");
                SerialDisconnect();
            }
            catch (TimeoutException exception)
            {
                _errorReporterViewModel.ReportError("Serial Communication", "Serial Port Timeout");
                SerialDisconnect();
            }
            catch (Exception exception)
            {
                _errorReporterViewModel.ReportError("Serial Communication", exception.Message);
                SerialDisconnect();
            }
        }

        /// <summary>
        /// Sends an outgoing message through the serial port.
        /// </summary>
        /// <param name="outgoingMessage"></param>
        private void SendMessage(string outgoingMessage)
        {
            //Remove all comments (characters after the terminal character) from the outgoing message).
            if (!String.IsNullOrWhiteSpace(outgoingMessage))
            {
                outgoingMessage = outgoingMessage.Trim();

                //If message is not a comment...
                if (!(outgoingMessage[0] == SerialMessageCharacters.SerialTerminalCharacter))
                {
                    //Remove comments from the message.
                    int terminalIndex = outgoingMessage.IndexOf(SerialMessageCharacters.SerialTerminalCharacter);
                    if (terminalIndex > 0)
                    {
                        outgoingMessage = outgoingMessage.Substring(0, terminalIndex + 1);
                    }

                    //Wait until the microcontroller inteprets the sent message before sending a new message.
                    //Pause and resume messages do not need a reply.
                    if (!((outgoingMessage[0] == SerialMessageCharacters.SerialPauseHardwareCharacter)
                       || (outgoingMessage[0] == SerialMessageCharacters.SerialResumeHardwareCharacter)
                       || (outgoingMessage[0] == SerialMessageCharacters.SerialMovementBufferClearCharacter)))
                    {
                        _shouldSend = false;
                    }

                    //Sends a message through the serial ports.
                    _serialPort.WriteLine(outgoingMessage);

                    //Triggers the message sent event.
                    SerialMessageEventArgs serialMessageEventArgs = new SerialMessageEventArgs(outgoingMessage);
                    Application.Current.Dispatcher.Invoke(() =>
                    OnSerialCommunicationMessageSent(serialMessageEventArgs));
                }
            }
        }

        /// <summary>
        /// Fires when a limit switch status message is received.
        /// Pauses serial communications.
        /// </summary>
        private void LimitSwitchHit()
        {
            _shouldPause = true;
        }

        /// <summary>
        /// Attempt a connection to a serial port.
        /// Begins the serial protocol after connecting.
        /// </summary>
        public void SerialConnect(string portName)
        {
            //Set port name.
            _serialPort.PortName = portName;

            //Connect to port.
            if (_serialPort.IsOpen == false)
            {
                ClearBuffer();
                _serialPort.Open();
                OnSerialConnectionChanged();

                //Wait for the Serial Port on both ends to be fully open.
                Thread.Sleep(1000);

                //Handles the continual sending and receiving of messages.
                SerialCommunicate();
            }
        }

        /// <summary>
        /// Disconnect from a serial port and wipes the buffer.
        /// </summary>
        public void SerialDisconnect()
        {
            ClearBuffer();
            _serialPort.Close();
            _serialCommunicationOutgoingMessagesModel.ClearProspectiveOutgoingMessages();
            OnSerialConnectionChanged();
        }

        /// <summary>
        /// Is the Serial Port open?
        /// </summary>
        /// <returns></returns>
        public bool IsPortOpen()
        {
            return _serialPort.IsOpen ? true : false;
        }

        /// <summary>
        /// Wipes the in and out serial port buffer.
        /// </summary>
        public void ClearBuffer()
        {
            if(_serialPort.IsOpen == true)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
            }
        }
        #endregion
    }
}
