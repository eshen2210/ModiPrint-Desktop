using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;

namespace ModiPrint.Models.GCodeConverterModels.CorneringModels
{
    //Associated with the MovementModel class.
    //States the velocity profile of the movement.
    public enum VelocityProfileType
    {
        Unset, 
        Linear, //Only acceleration/deceleration/flat velocity throughout the entirety of the movement.
        Triangle, //Acceleration followed immediately by deceleration.
        Trapezoid, //Acceleration, followed immediately by cruise, then followed immediately by deceleration.
    }
    
    /// <summary>
    /// Contains information on a movement.
    /// A movement is a single G command that involves any linear movement.
    /// </summary>
    public class MovementModel
    {
        #region Fields and Properties
        //Relative distances for this movement.
        //In mm.
        private double _x;
        public double X
        {
            get { return _x; }
        }
        private double _y;
        public double Y
        {
            get { return _y; }
        }
        private double _z;
        public double Z
        {
            get { return _z; }
        }
        private double _e;
        public double E
        {
            get { return _e; }
        }

        //Corresponding index of the ConvertedGCodeLineList where this movement stored.
        private int _gCodeIndex;
        public int GCodeIndex
        {
            get { return _gCodeIndex; }
        }

        //Maximum speed of this movement.
        //In units of mm / s.
        private double _maxSpeed = double.MaxValue;
        public double MaxSpeed
        {
            get { return _maxSpeed; }
            set { _maxSpeed = value; }
        }

        //Acceleration of this movement.
        //In units of mm / s^2.
        private double _acceleration = double.MaxValue;
        public double Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        //Exit speed of this movement.
        //Will be calculated based on junction speeds.
        //In units of mm / s.
        private double _exitSpeed = 0;
        public double ExitSpeed
        {
            get { return _exitSpeed; }
            set { _exitSpeed = value; }
        }

        //Total distance travelled by the movement.
        //In units of mm.
        //Absolute value.
        private double _totalDistance = 0;
        public double TotalDistance
        {
            get { return _totalDistance; }
        }

        //The shape of the velocity profile.
        private VelocityProfileType _velocityProfileType = VelocityProfileType.Unset;
        public VelocityProfileType VelocityProfileType
        {
            get { return _velocityProfileType; }
            set { _velocityProfileType = value; }
        }

        //Distance travelled until cruise needs to occur.
        //In mm.
        //Absolute value.
        private double _distanceToCruise;
        public double DistanceToCruise
        {
            get { return _distanceToCruise; }
            set { _distanceToCruise = value; }
        }

        //Distance travelled until deceleration needs to occur.
        //In mm.
        //Absolute value.
        private double _distanceToDeceleration;
        public double DistanceToDeceleration
        {
            get { return _distanceToDeceleration; }
            set { _distanceToDeceleration = value; }
        }
        #endregion

        #region Constructor
        public MovementModel(double X, double Y, double Z, double E, int GCodeIndex, MaterialModel MaterialModel)
        {
            _x = X;
            _y = Y;
            _z = Z;
            _e = E;
            _gCodeIndex = GCodeIndex;
            _totalDistance = Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2) + Math.Pow(_z, 2) + Math.Pow(_e, 2));
            CalculateSpeeds(MaterialModel, ref _maxSpeed, ref _acceleration);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculate max speed and acceleration such that these values do not exceed the invidual max speed and acceleration values of each Axis.
        /// </summary>
        /// <param name="materialModel"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="acceleration"></param>
        private void CalculateSpeeds(MaterialModel materialModel, ref double maxSpeed, ref double acceleration)
        {
            double[] distanceArr = new double[4];
            distanceArr[0] = Math.Abs(_x);
            distanceArr[1] = Math.Abs(_y);
            distanceArr[2] = Math.Abs(_z);
            distanceArr[3] = Math.Abs(_e);
            double[] maxSpeedArr = new double[4];
            maxSpeedArr[0] = materialModel.XPrintSpeed;
            maxSpeedArr[1] = materialModel.YPrintSpeed;
            maxSpeedArr[2] = materialModel.ZPrintSpeed;
            double[] accelerationArr = new double[4];
            accelerationArr[0] = materialModel.XPrintAcceleration;
            accelerationArr[1] = materialModel.YPrintAcceleration;
            accelerationArr[2] = materialModel.ZPrintAcceleration;
            if (materialModel.PrintheadModel.PrintheadType == PrintheadType.Motorized)
            {
                MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)materialModel.PrintheadModel.PrintheadTypeModel;
                maxSpeedArr[3] = motorizedPrintheadTypeModel.MaxSpeed;
                accelerationArr[3] = motorizedPrintheadTypeModel.MaxAcceleration;
            }
            else
            {
                maxSpeedArr[3] = 0;
                accelerationArr[3] = 0;
            }

            for (int k = 0; k < 4; k++)
            {
                if (maxSpeedArr[k] != 0)
                {
                    double scaledMaxSpeed = maxSpeedArr[k] * _totalDistance / distanceArr[k];
                    maxSpeed = (scaledMaxSpeed < maxSpeed) ? scaledMaxSpeed : maxSpeed;
                }

                if (accelerationArr[k] != 0)
                {
                    double scaledAcceleration = accelerationArr[k] * _totalDistance / distanceArr[k];
                    acceleration = (scaledAcceleration < acceleration) ? scaledAcceleration : acceleration;
                }
            }
        } 
        #endregion
    }
}
