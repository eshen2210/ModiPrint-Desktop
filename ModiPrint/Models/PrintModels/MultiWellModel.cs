using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrinterModels;

namespace ModiPrint.Models.PrintModels
{
    /// <summary>
    /// Manages a collection of WellModels.
    /// </summary>
    public class MultiWellModel
    {
        #region Fields and Properties
        //Contains Print parameters for the Wells.
        private PrinterModel _printerModel;

        //Number of rows and columns of the entire multiwell.
        //Example: A 96 well has 8 rows and 12 columns. A non-multiwell object has 1 row and 1 column.
        private int _rows = 1;
        public int Rows
        {
            get { return _rows; }
            set
            {
                UpdateWellModelArray(value, _columns);
                _rows = value;
            }
        }

        private int _columns = 1;
        public int Columns
        {
            get { return _columns; }
            set
            {
                UpdateWellModelArray(_rows, value);
                _columns = value;
            }
        }

        //Sets the distances between each well.
        //In units of mm.
        private int _rowDistance = 0;
        public int RowDistance
        {
            get { return _rowDistance; }
            set { _rowDistance = value; }
        }

        private int _columnDistance = 0;
        public int ColumnDistance
        {
            get { return _columnDistance; }
            set { _columnDistance = value; }
        }

        //Contains references to each WellModel.
        private WellModel[ , ] _wellModelArray = new WellModel[1, 1];
        public WellModel[ , ] WellModelArray
        {
            get { return _wellModelArray; }
        }
        #endregion

        #region Constructor
        public MultiWellModel(PrinterModel PrinterModel)
        {
            _printerModel = PrinterModel;

            _wellModelArray[0, 0] = new WellModel(PrinterModel, this, 0, 0);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates or removes WellModels from the WellModelArray based on changes to the Rows and Columns members.
        /// </summary>
        /// <param name="newRow"></param>
        /// <param name="newColumn"></param>
        private void UpdateWellModelArray(int newRow, int newColumn)
        {
            WellModel[,] newWellModelArray = new WellModel[newRow, newColumn];

            for(int i = 0; i < newWellModelArray.GetLength(0); i++)
            {
                for (int j = 0; j < newWellModelArray.GetLength(1); j++)
                {
                    if ((i < _wellModelArray.GetLength(0)) && (j < _wellModelArray.GetLength(1)))
                    {
                        newWellModelArray[i, j] = _wellModelArray[i, j];
                    }
                    else
                    {
                        newWellModelArray[i, j] = new WellModel(_printerModel, this, i + 1, j + 1);
                    }
                }
            }

            _wellModelArray = newWellModelArray;
        }

        /// <summary>
        /// Copies one Well's parameters to all of the other Wells.
        /// </summary>
        public void CopyWellParameters(int row, int column)
        {
            if ((row < _wellModelArray.GetLength(0)) && (column < _wellModelArray.GetLength(1)))
            {
                for (int i = 0; i < _wellModelArray.GetLength(0); i++)
                {
                    for (int j = 0; j < _wellModelArray.GetLength(1); j++)
                    {
                        _wellModelArray[i, j] = _wellModelArray[row, column];
                    }
                }
            }
        }
        #endregion
    }
}
