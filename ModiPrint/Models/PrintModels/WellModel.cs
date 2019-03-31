using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrinterModels;

namespace ModiPrint.Models.PrintModels
{ 
    public class WellModel
    {
        #region Fields and Properties
        //Contains Print parameters for this Well.
        private PrinterModel _printerModel;
        private PrintModel _printModel;
        public PrintModel PrintModel
        {
            get { return _printModel; }
            set { _printModel = value; }
        }
        
        //Used to calculate the offset of this Well.
        private MultiWellModel _multiWellModel;

        //The index of this Well within the MultiWell.
        //Index starts at 1.
        private int _rowNumber;
        private int _columnNumber;

        //The position of this Well relative to Well (1, 1).
        public int XOffSet
        {
            get { return (_rowNumber - 1) * _multiWellModel.RowDistance; }
        }

        public int YOffSet
        {
            get { return (_columnNumber - 1) * _multiWellModel.ColumnDistance; }
        }
        #endregion

        #region Constructor
        public WellModel(PrinterModel PrinterModel, MultiWellModel MultiWellModel, int RowNumber, int ColumnNumber)
        {
            _printModel = new PrintModel(PrinterModel);

            _multiWellModel = MultiWellModel;
            _rowNumber = RowNumber;
            _columnNumber = ColumnNumber;
        }
        #endregion

        #region Methods

        #endregion
    }
}
