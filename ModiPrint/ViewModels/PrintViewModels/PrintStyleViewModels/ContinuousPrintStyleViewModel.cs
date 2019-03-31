﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrintModels.PrintStyleModels;

namespace ModiPrint.ViewModels.PrintViewModels.PrintStyleViewModels
{
    /// <summary>
    /// Interface between ContinuousPrintStyleModel and the GUI.
    /// </summary>
    public class ContinuousPrintStyleViewModel : PrintStyleViewModel
    {
        #region Fields and Proeprties
        //Model counterpart.
        private ContinuousPrintStyleModel _continuousPrintStyleModel;
        public ContinuousPrintStyleModel ContinuousPrintStyleModel
        {
            get { return _continuousPrintStyleModel; }
        }

        //The parameter used by the Motorized Printhead while printing with this Continuous Style.
        //This parameters specifies the distance (in milimeters) of Printhead motor movement per milimeter of Axis movement.
        public double MotorizedDispenseDistancePermm
        {
            get { return _continuousPrintStyleModel.MotorizedDispenseDistancePermm; }
            set
            {
                _continuousPrintStyleModel.MotorizedDispenseDistancePermm = value;
                OnPropertyChanged("MotorizedDispenseDistancePermm");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Writes ContinuousPrintStyleViewModel properties into XML.
        /// </summary>
        /// <param name="ContinuousPrintStyleModel"></param>
        public ContinuousPrintStyleViewModel(ContinuousPrintStyleModel ContinuousPrintStyleModel) : base(ContinuousPrintStyleModel)
        {
            _continuousPrintStyleModel = ContinuousPrintStyleModel;
        }
        #endregion
    }
}
