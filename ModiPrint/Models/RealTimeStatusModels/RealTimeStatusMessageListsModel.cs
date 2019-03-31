using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;

using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusPrintheadModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusAxisModels;

namespace ModiPrint.Models.RealTimeStatusModels
{
    /// <summary>
    /// Contains lists logging the various types of serial incoming messages.
    /// Raw serial messages should be interpreted into readable phrases before storage into these lists.
    /// </summary>
    public class RealTimeStatusMessageListsModel
    {
        #region Fields and Properties
        //Tasks that are in the process of execution by the microcontroller.
        //These tasks are removed from the list as task completed messages are received.
        private ObservableCollection<string> _taskQueuedMessagesList = new ObservableCollection<string>();
        public ObservableCollection<string> TaskQueuedMessagesList
        {
            get { return _taskQueuedMessagesList; }
        }

        //Messages that arise out of the normal order of command -> execution (emergency messages, etc.)
        private ObservableCollection<string> _statusMessagesList = new ObservableCollection<string>();
        public ObservableCollection<string> StatusMessagesList
        {
            get { return _statusMessagesList; }
        }

        //Messages for errors reported by the micrcontroller.
        private ObservableCollection<string> _errorMessagesList = new ObservableCollection<string>();
        public ObservableCollection<string> ErrorMessagesList
        {
            get { return _errorMessagesList; }
        }
        #endregion

        #region Constructor
        public RealTimeStatusMessageListsModel()
        {

        }
        #endregion
    }
}
