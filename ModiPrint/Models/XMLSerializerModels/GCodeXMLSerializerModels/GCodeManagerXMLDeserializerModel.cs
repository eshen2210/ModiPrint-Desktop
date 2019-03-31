using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ModiPrint.Models.XMLSerializerModels;
using ModiPrint.ViewModels;
using ModiPrint.ViewModels.GCodeManagerViewModels;

namespace ModiPrint.Models.XMLSerializerModels.GCodeXMLSerializerModels
{
    public class GCodeManagerXMLDeserializerModel : XMLConverterModel
    {
        #region Constructor
        public GCodeManagerXMLDeserializerModel(ErrorListViewModel ErrorListViewModel) : base(ErrorListViewModel)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads GCodeManagerViewModel properties from XML and loads a GCodeManagerViewModel with said properties.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="gCodeManagerViewModel"></param>
        public void DeserializeGCode(XmlReader xmlReader, GCodeManagerViewModel gCodeManagerViewModel)
        {
            //Read through each element of the XML string and populate each property of the GCodeManager.
            while (xmlReader.Read())
            {
                //Skip through newlines (this program's XML Writer uses newlines).
                if ((xmlReader.Name != "\n") && (!String.IsNullOrWhiteSpace(xmlReader.Name)))
                {
                    //End method if the end of "GCodeManager" element is reached.
                    if ((xmlReader.Name == "GCodeManager") && (xmlReader.NodeType == XmlNodeType.EndElement))
                    {
                        return;
                    }

                    switch (xmlReader.Name)
                    {
                        case "Slic3rGCode":
                            gCodeManagerViewModel.Slic3rGCode = xmlReader.ReadElementContentAsString();
                            break;
                        case "Slic3rGCodeFileName":
                            gCodeManagerViewModel.Slic3rGCodeFileName = xmlReader.ReadElementContentAsString();
                            break;
                        case "ModiPrintGCode":
                            gCodeManagerViewModel.ModiPrintGCode = xmlReader.ReadElementContentAsString();
                            break;
                        case "ModiPrintGCodeFileName":
                            gCodeManagerViewModel.ModiPrintGCodeFileName = xmlReader.ReadElementContentAsString();
                            break;
                        default:
                            base.ReportErrorUnrecognizedElement(xmlReader);
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
