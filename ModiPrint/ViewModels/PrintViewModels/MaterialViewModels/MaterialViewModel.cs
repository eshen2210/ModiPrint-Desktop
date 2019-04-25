using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModiPrint.Models.PrintModels.PrintStyleModels;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels;
using ModiPrint.ViewModels.PrintViewModels.PrintStyleViewModels;

namespace ModiPrint.ViewModels.PrintViewModels.MaterialViewModels
{
    /// <summary>
    /// ViewModel that interfaces MaterialModel with the GUI.
    /// </summary>
    public class MaterialViewModel : ViewModel
    {
        #region Fields and Properties
        //Contains parameters for the Material that will be printed.
        private MaterialModel _materialModel;
        public MaterialModel MaterialModel
        {
            get { return _materialModel; }
        }

        //Name of this Material.
        public string Name
        {
            get { return _materialModel.Name; }
        }

        //How RepRap identifies this material (e.g. "T0" for the first material).
        public string RepRapID
        {
            get { return _materialModel.RepRapID; }
            set
            {
                _materialModel.RepRapID = value;
                OnPropertyChanged("RepRapID");
            }
        }

        //Which printhead will be dispensing this material?
        private PrintheadViewModel _printheadViewModel;
        public PrintheadViewModel PrintheadViewModel
        {
            get { return _printheadViewModel; }
            set
            {
                if (value != null) //To Do: This value is null sometimes when I'm at the MaterialView and switch to another View
                {
                    _materialModel.PrintheadModel = value.PrintheadModel;
                    _printheadViewModel = value;
                    OnPropertyChanged("PrintheadViewModel");
                }
            }
        }

        //How is this Material being printed?
        //Type specific properties are contained in the PrintStyle object.
        private PrintStyleViewModel _printStyleViewModel;
        public PrintStyleViewModel PrintStyleViewModel
        {
            get { return _printStyleViewModel; }
            set { _printStyleViewModel = value; }
        }

        //How is this Material being printed?
        public PrintStyle PrintStyle
        {
            get { return _materialModel.PrintStyle; }
            set
            {
                _materialModel.PrintStyle = value;
                switch (value)
                {
                    case PrintStyle.Continuous:
                        _printStyleViewModel = new ContinuousPrintStyleViewModel((ContinuousPrintStyleModel)_materialModel.PrintStyleModel);
                        break;
                    case PrintStyle.Droplet:
                        _printStyleViewModel = new DropletPrintStyleViewModel((DropletPrintStyleModel)_materialModel.PrintStyleModel);
                        break;
                    case PrintStyle.Unset:
                        _printStyleViewModel = null;
                        break;
                }
                OnPropertyChanged("PrintStyle");
            }
        }

        //Returns values of PrintStyle that are bindable to a control's Itemsource property.
        public IEnumerable<PrintStyle> PrintStyleValues
        {
            get { return Enum.GetValues(typeof(PrintStyle)).Cast<PrintStyle>(); }
        }

        //If true, then print speeds and accelerations are set to the max values of the Axes.
        //If true, prevents the print speeds and accelerations from being manually set (logic for this is in MaterialModel). 
        public bool MaximizePrintSpeeds
        {
            get { return _materialModel.MaximizePrintSpeeds; }
            set
            {
                _materialModel.MaximizePrintSpeeds = value;
                OnPropertyChanged("XPrintSpeed");
                OnPropertyChanged("YPrintSpeed");
                OnPropertyChanged("ZPrintSpeed");
                OnPropertyChanged("XPrintAcceleration");
                OnPropertyChanged("YPrintAcceleration");
                OnPropertyChanged("ZPrintAcceleration");
            }
        }

        //Printing speed for each axis.
        public double XPrintSpeed
        {
            get { return _materialModel.XPrintSpeed; }
            set
            {
                _materialModel.XPrintSpeed = value;
                OnPropertyChanged("XPrintSpeed");
            }
        }

        public double YPrintSpeed
        {
            get { return _materialModel.YPrintSpeed; }
            set
            {
                _materialModel.YPrintSpeed = value;
                OnPropertyChanged("YPrintSpeed");
            }
        }

        public double ZPrintSpeed
        {
            get { return _materialModel.ZPrintSpeed; }
            set
            {
                _materialModel.ZPrintSpeed = value;
                OnPropertyChanged("ZPrintSpeed");
            }
        }

        //Printing acceleration for each axis.
        public double XPrintAcceleration
        {
            get { return _materialModel.XPrintAcceleration; }
            set
            {
                _materialModel.XPrintAcceleration = value;
                OnPropertyChanged("XPrintAcceleration");
            }
        }

        public double YPrintAcceleration
        {
            get { return _materialModel.YPrintAcceleration; }
            set
            {
                _materialModel.YPrintAcceleration = value;
                OnPropertyChanged("YPrintAcceleration");
            }
        }

        public double ZPrintAcceleration
        {
            get { return _materialModel.ZPrintAcceleration; }
            set
            {
                _materialModel.ZPrintAcceleration = value;
                OnPropertyChanged("ZPrintAcceleration");
            }
        }

        //Junction deviation determines junction speed of movemements.
        //The higher the value, the faster the junction speeds.
        //However, high values can cause harsh transitions in between movements and missed steps.
        //Value must be between 0 to 1 and is usually on the very low end (< 0.1).
        public double JunctionDeviation
        {
            get { return _materialModel.JunctionDeviation; }
            set { _materialModel.JunctionDeviation = value; }
        }

        //Are all of the parameters set correctly such that printing can occur?
        public bool ReadyToPrint()
        {
            return _materialModel.ReadyToPrint();
        }
        #endregion

        #region Constructor
        public MaterialViewModel(MaterialModel MaterialModel)
        {
            _materialModel = MaterialModel;
        }
        #endregion
    }
}
