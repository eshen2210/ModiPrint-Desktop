using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrinterModels;

namespace ModiPrint.ViewModels.PrinterViewModels
{
    public class PrinterSettingsViewModel : ViewModel
    {
        #region Fields and Properties
        private PrinterSettingsModel _printerSettingsModel;

        #endregion

        #region Contructor
        public PrinterSettingsViewModel(PrinterSettingsModel PrinterSettingsModel)
        {
            _printerSettingsModel = PrinterSettingsModel;
        }
        #endregion
    }
}
