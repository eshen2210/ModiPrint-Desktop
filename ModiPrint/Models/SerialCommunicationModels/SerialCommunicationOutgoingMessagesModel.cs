using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.ViewModels.SerialCommunicationViewModels;

namespace ModiPrint.Models.SerialCommunicationModels
{
    /// <summary>
    /// Manages the message that need to be sent through the serial port.
    /// </summary>
    public class SerialCommunicationOutgoingMessagesModel
    {
        #region Fields and Properties 
        //List of message that are about to be sent through the serial port.
        private List<string> _prospectiveOutgoingMessageList = new List<string>();
        public List<string> ProspectiveOutgoingMessageList
        {
            get { return _prospectiveOutgoingMessageList; }
        }
        #endregion

        #region Constructor
        public SerialCommunicationOutgoingMessagesModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds an entry to the end of ProspectiveOutgoingMessageList.
        /// </summary>
        /// <param name="prospectiveOutgoingMessage"></param>
        public void AppendProspectiveOutgoingMessage(string prospectiveOutgoingMessage)
        {
            if (!String.IsNullOrWhiteSpace(prospectiveOutgoingMessage))
            { _prospectiveOutgoingMessageList.Add(prospectiveOutgoingMessage); }
        }

        /// <summary>
        /// Adds an entry to the beginning of ProspectiveOutgoingMessageList.
        /// </summary>
        /// <param name="prospectiveOutgoingMessage"></param>
        public void QueueNextProspectiveOutgoingMessage(string prospectiveOutgoingMessage)
        {
            if (!String.IsNullOrWhiteSpace(prospectiveOutgoingMessage))
            { _prospectiveOutgoingMessageList.Insert(0, prospectiveOutgoingMessage); }
        }

        /// <summary>
        /// Adds a series of entries tot he beginning of ProspectiveOutgoingMessageList.
        /// </summary>
        /// <param name="prospectiveOutgoingMessageList"></param>
        public void QueueNextProspectiveOutgoingMessage(List<string> prospectiveOutgoingMessages)
        {
            if ((prospectiveOutgoingMessages != null)
             && (prospectiveOutgoingMessages.Count > 0))
            {
                //Inserts the outgoing messages in reverse order to the front of the list.
                //This preserves the ordering of the outgoing messages while still shoving them all to the front.
                for (int i = prospectiveOutgoingMessages.Count - 1; i >= 0; i--)
                {
                    _prospectiveOutgoingMessageList.Insert(0, prospectiveOutgoingMessages[i]);
                }
            }
        }

        /// <summary>
        /// Returns the earliest entry of the ProspectiveOutgoingMessageList and then removes it from the list.
        /// </summary>
        /// <returns></returns>
        public string RetrieveNextProspectiveOutgoingMessage()
        {
            string prospectiveOutgoingMessage = "";

            prospectiveOutgoingMessage = _prospectiveOutgoingMessageList[0];
            System.Threading.Thread.Sleep(10); //To Do: Some hacked together solution because of weird out of range errors.
            _prospectiveOutgoingMessageList.Remove(prospectiveOutgoingMessage); //Using RemoveAt(0) somehow gives an out of range error.

            return prospectiveOutgoingMessage;
        }

        /// <summary>
        /// Clears the ProspectiveOutgoingMessageList.
        /// </summary>
        public void ClearProspectiveOutgoingMessages()
        {
            _prospectiveOutgoingMessageList.Clear();
        }
        #endregion
    }
}
