﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.ViewModels.ManualControlViewModels.AxesPrintStyles
{
    /// <summary>
    /// Contains display elements for the AxesPrintStyle parameter on ManualControlViewModel.
    /// </summary>
    public class ContinuousPrintStyle : AxesPrintStyle
    {
        #region Constructor
        public ContinuousPrintStyle() : base()
        {
            base._styleID = "Continuous Print";
            base._imageSource = "/Resources/General/Continuous.png";
            base._displayString1 = "Continuous";
            base._displayString2 = "Print";
        }
        #endregion
    }
}
