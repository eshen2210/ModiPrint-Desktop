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

using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusPrintheadModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusAxisModels;

namespace ModiPrint.ViewModels.RealTimeStatusViewModels
{
    public class RealTimeStatusMessageListsViewModel
    {
        #region Fields and Properties
        //Model counterpart to this class.
        RealTimeStatusMessageListsModel _realTimeStatusMessageListsModel;

        //Tasks that are in the process of execution by the microcontroller.
        //These tasks are removed from the list as task completed messages are received.
        public ObservableCollection<string> TaskQueuedMessagesList
        {
            get { return _realTimeStatusMessageListsModel.TaskQueuedMessagesList; }
        }

        //Messages that arise out of the normal order of command -> execution (emergency messages, etc.)
        public ObservableCollection<string> StatusMessagesList
        {
            get { return _realTimeStatusMessageListsModel.StatusMessagesList; }
        }

        //Messages for errors reported by the micrcontroller.
        public ObservableCollection<string> ErrorMessagesList
        {
            get { return _realTimeStatusMessageListsModel.ErrorMessagesList; }
        }
        #endregion

        #region Constructor
        public RealTimeStatusMessageListsViewModel(RealTimeStatusMessageListsModel RealTimeStatusMessageListsModel)
        {
            _realTimeStatusMessageListsModel = RealTimeStatusMessageListsModel;
        }
        #endregion
    }
}
