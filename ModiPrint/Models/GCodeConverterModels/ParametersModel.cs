using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.Models.PrintModels.PrintStyleModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.GCodeConverterModels
{
    /// <summary>
    /// Pseudo-global parameters for the GCodeConverter that are passed around various GCodeConverter classes.
    /// </summary>
    public class ParametersModel
    {
        #region Fields and Properties
        //Contains parameters for the Printer.
        PrinterModel _printerModel;
        
        //The positions of the coordinates are kept in these classes.
        //References are set in the constructor.
        //The XYZ positions in the GCode.
        private CoordinateModel _xCoord;
        public CoordinateModel XCoord
        {
            get { return _xCoord; }
        }
        private CoordinateModel _yCoord;
        public CoordinateModel YCoord
        {
            get { return _yCoord; }
        }
        private CoordinateModel _zCoord;
        public CoordinateModel ZCoord
        {
            get { return _zCoord; }
        }

        //Used if a Print Style is set to Droplet.
        //Keeps track of the position of the last droplet and ensures the interpolate distance parameters is not violated.
        private DropletModel _dropletModel;
        public DropletModel DropletModel
        {
            get { return _dropletModel; }
        }

        //If true, then this line should use G01/G02/G11/G12, etc.
        //If false, then this line should use G00.
        private bool _isPrinting;
        public bool IsPrinting
        {
            get { return _isPrinting; }
            set { _isPrinting = value; }
        }

        //The E command in Slic3r represents extrusion rate.
        //Here it may be used to determine if printing is suppose to occur.
        private CoordinateModel _eSlic3rCoord;
        public CoordinateModel ESlic3rCoord
        {
            get { return _eSlic3rCoord; }
        }

        //The XYZ coordinates for Slic3r should match the XYZ coordinates in ModiPrint.
        //However, the E coordinates are different for the different software.
        //Therefore, this program needs to track the E coordinates for each of its printheads.
        private List<CoordinateModel> _eModiPrintCoordList = new List<CoordinateModel>();
        public List<CoordinateModel> EModiPrintCoordList
        {
            get { return _eModiPrintCoordList; }
        }

        //True = absolute coordinate system for Axis movement interpretation where the origin (0, 0, 0, 0) is set to the position at the start of this Print.
        //False = relative coordinate system for Axis movement interpretation.
        private bool _absCoordAxis = true;
        public bool AbsCoordAxis
        {
            get { return _absCoordAxis; }
            set { _absCoordAxis = value; }
        }

        //True = absolute coordinate system for extruder movement interpretation.
        //False = relative coordinate system for extruder movement interpretation.
        private bool _absCoordExtruder = true;
        public bool AbsCoordExtruder
        {
            get { return _absCoordAxis; }
            set { _absCoordExtruder = value; }
        }

        //Contains functions required to report errors to the GUI.
        private ErrorReporterViewModel _errorReporterViewModel;
        public ErrorReporterViewModel ErrorReporterViewModel
        {
            get { return _errorReporterViewModel; }
        }

        //The current line of Slic3r GCode that is being processed.
        private int _slic3rLine = 0;
        public int Slic3rLine
        {
            get { return _slic3rLine; }
            set { _slic3rLine = value; }
        }
        #endregion

        #region Constructor
        public ParametersModel(PrinterModel PrinterModel, ErrorListViewModel ErrorListViewModel)
        {
            _printerModel = PrinterModel;

            _xCoord = new CoordinateModel(CoordinateType.X, true, this, 0, 0, 0);
            _yCoord = new CoordinateModel(CoordinateType.Y, true, this, 0, 0, 0);
            _zCoord = new CoordinateModel(CoordinateType.Z, true, this, 0, 0, 0);
            _eSlic3rCoord = new CoordinateModel(CoordinateType.S, false, this, 0, 0, 0);

            // _dropletModel is not set yet. A new material must be set before this class is instantiated.

            //This program needs to track the E coordinates for each of its printheads.
            //Some printheads, such as the valve-based ones, do not need to track E.
            //It's easier to just create an E coordinate for every printhead though.
            //The index of the E coordinate list equals the index of the Printer's Printheads list.
            foreach (PrintheadModel printheadModel in _printerModel.PrintheadModelList)
            { _eModiPrintCoordList.Add(new CoordinateModel(CoordinateType.E, true, this, 0, 0, 0)); }

            _errorReporterViewModel = new ErrorReporterViewModel(ErrorListViewModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a new Coordinate Model for an Axis.
        /// Typically used when switching Axes or setting new Axis parameters.
        /// </summary>
        /// <param name="axisID"></param>
        /// <param name="position"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        public void SetNewXYZCoord(char axisID, double position, double minPosition, double maxPosition)
        {
            switch(axisID)
            {
                case 'X':
                    _xCoord = new CoordinateModel(CoordinateType.X, true, this, position, minPosition, maxPosition);
                    break;
                case 'Y':
                    _yCoord = new CoordinateModel(CoordinateType.Y, true, this, position, minPosition, maxPosition);
                    break;
                case 'Z':
                    _zCoord = new CoordinateModel(CoordinateType.Z, true, this, position, minPosition, maxPosition);
                    break;
            }
        }

        /// <summary>
        /// Create a new Coordinate Model for a Motorized Printhead.
        /// Typically used when switching Printheads.
        /// </summary>
        /// <param name="printheadModel"></param>
        /// <param name="position"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        public void SetNewECoord(PrintheadModel printheadModel, double position, double minPosition, double maxPosition)
        {
            _eModiPrintCoordList[FindEModiPrintCoordIndex(printheadModel)] = new CoordinateModel(CoordinateType.E, true, this, position, minPosition, maxPosition);
        }

        /// <summary>
        /// Reset parameters when a new Material is used.
        /// </summary>
        /// <param name="materialModel"></param>
        public void SetNewMaterial(MaterialModel materialModel)
        {
            //Reset DropletModel.
            if (materialModel.PrintStyle == PrintStyle.Droplet)
            {
                DropletPrintStyleModel dropletPrintStyleModel = (DropletPrintStyleModel)materialModel.PrintStyleModel;
                _dropletModel = new DropletModel(dropletPrintStyleModel);
            }
        }

        /// <summary>
        /// Returns an index for eModiPrintCoordList which contains the eModiPrintCoord associated with the current eSlic3rCoord.
        /// </summary>
        /// <param name="currentMaterial"></param>
        /// <returns></returns>
        public int FindEModiPrintCoordIndex(PrintheadModel printheadModel)
        {
            //The List index of eModiPrintCoordList.
            //This same index in the Printer's PrintheadModelList has the CoordinateModel's associated Printhead.
            int eModiPrintCoordIndex = 0;
            for (eModiPrintCoordIndex = 0; eModiPrintCoordIndex < (_printerModel.PrintheadModelList.Count - 1); eModiPrintCoordIndex++)
            {
                if (printheadModel == _printerModel.PrintheadModelList[eModiPrintCoordIndex])
                { break; }
            }
            return eModiPrintCoordIndex;
        }
        #endregion
    }
}
