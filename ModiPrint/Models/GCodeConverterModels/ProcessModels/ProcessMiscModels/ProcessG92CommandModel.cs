using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessMiscModels
{
    public class ProcessG92CommandModel
    {
        #region Fields and Properties
        //Contains global variables for the GCodeConverter.
        private ParametersModel _parametersModel;
        #endregion

        #region Constructor
        public ProcessG92CommandModel(ParametersModel ParametersModel)
        {
            _parametersModel = ParametersModel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute RepRap G92 command.
        /// </summary>
        /// <param name="repRapLine"></param>
        public void ProcessG92Command(string[] repRapLine)
        {
            //If a coordinate is not a part of this GCode line, then assume it should be set to zero.
            bool xSet = false;
            bool ySet = false;
            bool zSet = false;
            bool eSet = false;

            //Read through the GCode line and parse coordinate values.
            for (int phrase = 1; phrase < repRapLine.Length; phrase++)
            {
                switch (repRapLine[phrase][0])
                {
                    case 'X':
                        _parametersModel.XCoord.NewCoord(GCodeStringParsing.ParseDouble(repRapLine[phrase]));
                        xSet = true;
                        break;
                    case 'Y':
                        _parametersModel.YCoord.NewCoord(GCodeStringParsing.ParseDouble(repRapLine[phrase]));
                        ySet = true;
                        break;
                    case 'Z':
                        _parametersModel.ZCoord.NewCoord(GCodeStringParsing.ParseDouble(repRapLine[phrase]));
                        zSet = true;
                        break;
                    case 'E':
                        _parametersModel.ERepRapCoord.NewCoord(GCodeStringParsing.ParseDouble(repRapLine[phrase]));
                        eSet = true;
                        break;
                }

                //If a coordinate is not a part of this GCode line, then assume it should be set to zero.
                if (xSet == false)
                {
                    _parametersModel.XCoord.NewCoord(0);
                }

                if (ySet == false)
                {
                    _parametersModel.YCoord.NewCoord(0);
                }

                if (zSet == false)
                {
                    _parametersModel.ZCoord.NewCoord(0);
                }

                if (eSet == false)
                {
                    _parametersModel.ERepRapCoord.NewCoord(0);
                }
            }
        }
        #endregion
    }
}
