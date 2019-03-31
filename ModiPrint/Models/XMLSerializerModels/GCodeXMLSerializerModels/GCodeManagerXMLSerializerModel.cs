using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ModiPrint.Models.XMLSerializerModels;
using ModiPrint.ViewModels.GCodeManagerViewModels;

namespace ModiPrint.Models.XMLSerializerModels.GCodeXMLSerializerModels
{
    public class GCodeManagerXMLSerializerModel
    {
        #region Constructor
        public GCodeManagerXMLSerializerModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Write GCodeManagerViewModel properties into XML. 
        /// </summary>
        /// <param name="gCodeManagerViewModel"></param>
        /// <returns></returns>
        public void SerializeGCodeManager(XmlWriter xmlWriter, GCodeManagerViewModel gCodeManagerViewModel)
        {
            //Outmost element should be "GCodeManager".
            xmlWriter.WriteStartElement("GCodeManager");

            //Slic3r GCode.
            xmlWriter.WriteElementString("Slic3rGCode", gCodeManagerViewModel.Slic3rGCodeModel.GCodeStr);

            //Slic3r GCode File Name.
            xmlWriter.WriteElementString("Slic3rGCodeFileName", gCodeManagerViewModel.Slic3rGCodeFileName);

            //ModiPrint GCode.
            xmlWriter.WriteElementString("ModiPrintGCode", gCodeManagerViewModel.ModiPrintGCodeModel.GCodeStr);

            //ModiPrint GCode File Name.
            xmlWriter.WriteElementString("ModiPrintGCodeFileName", gCodeManagerViewModel.ModiPrintGCodeFileName);

            //End "GCodeManager" outmost element.
            xmlWriter.WriteEndElement();
        }
        #endregion
    }
}
