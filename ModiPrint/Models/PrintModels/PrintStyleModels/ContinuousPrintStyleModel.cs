using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.Models.PrintModels.PrintStyleModels
{
    /// <summary>
    /// Extends the MaterialModel class with parameters related to contiuous printing.
    /// </summary>
    public class ContinuousPrintStyleModel : PrintStyleModel
    {
        #region Fields and Properties
        //The parameter used by the Motorized Printhead while printing with this Continuous Style.
        //This parameters specifies the distance (in milimeters) of Printhead motor movement per milimeter of Axis movement.
        private double _motorizedDispenseDistancePermm = 0;
        public double MotorizedDispenseDistancePermm
        {
            get { return _motorizedDispenseDistancePermm; }
            set
            {
                if (0 <= value)
                { _motorizedDispenseDistancePermm = value; }
                else
                { _motorizedDispenseDistancePermm = 0; }
            }
        }
        #endregion

        #region Constructor
        public ContinuousPrintStyleModel() : base()
        {

        }
        #endregion
    }
}
