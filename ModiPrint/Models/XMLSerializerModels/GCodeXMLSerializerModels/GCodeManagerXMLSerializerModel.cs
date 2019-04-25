﻿using System;
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

            //RepRap GCode.
            xmlWriter.WriteElementString("RepRapGCode", gCodeManagerViewModel.RepRapGCodeModel.GCodeStr);

            //ModiPrint GCode.
            xmlWriter.WriteElementString("ModiPrintGCode", gCodeManagerViewModel.ModiPrintGCodeModel.GCodeStr);

            //GCode File Name.
            xmlWriter.WriteElementString("GCodeFileName", gCodeManagerViewModel.GCodeFileName);

            //End "GCodeManager" outmost element.
            xmlWriter.WriteEndElement();
        }
        #endregion
    }
}
