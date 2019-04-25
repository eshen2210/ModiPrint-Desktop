using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ModiPrint.Models.PrintModels.PrintStyleModels;
using ModiPrint.Models.XMLSerializerModels.PrintXMLSerializerModels.PrintStyleXMLSerializerModels;
using ModiPrint.ViewModels.PrintViewModels.MaterialViewModels;

namespace ModiPrint.Models.XMLSerializerModels.PrintXMLSerializerModels.MaterialXMLSerializerModels
{
    public class MaterialXMLSerializerModel
    {
        #region Constructor
        public MaterialXMLSerializerModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes MaterialViewModel properties into a XmlWriter.
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="materialViewModel"></param>
        public void SerializeMaterial(XmlWriter xmlWriter, MaterialViewModel materialViewModel)
        {
            //Outmost element should be "Material".
            xmlWriter.WriteStartElement("Material");

            //Outmost element should contain properties required for identification and instantiation.
            //Name.
            xmlWriter.WriteAttributeString("Name", materialViewModel.Name);

            //RepRap ID.
            xmlWriter.WriteElementString("RepRapID", materialViewModel.RepRapID);

            //Printhead.
            if (materialViewModel.PrintheadViewModel != null)
            { xmlWriter.WriteElementString("PrintheadName", materialViewModel.PrintheadViewModel.Name); }

            //Print Style.
            PrintStyleXMLSerializerModel printStyleXMLSerializerModel = null;
            switch (materialViewModel.PrintStyle)
            {
                case PrintStyle.Continuous:
                    printStyleXMLSerializerModel = new ContinuousPrintStyleXMLSerializerModel();
                    break;
                case PrintStyle.Droplet:
                    printStyleXMLSerializerModel = new DropletPrintStyleXMLSerializerModel();
                    break;
                case PrintStyle.Unset:
                    xmlWriter.WriteElementString("UnsetPrintStyle", "");
                    break;
            }
            if (printStyleXMLSerializerModel != null)
            { printStyleXMLSerializerModel.SerializePrintStyle(xmlWriter, materialViewModel.PrintStyleViewModel); }

            //Junction Deviation.
            xmlWriter.WriteElementString("JunctionDeviation", materialViewModel.JunctionDeviation.ToString());

            //Maximize Print Speeds.
            xmlWriter.WriteElementString("MaximizePrintSpeeds", materialViewModel.MaximizePrintSpeeds.ToString());

            //X Print Speed.
            xmlWriter.WriteElementString("XPrintSpeed", materialViewModel.XPrintSpeed.ToString());

            //Y Print Speed.
            xmlWriter.WriteElementString("YPrintSpeed", materialViewModel.YPrintSpeed.ToString());

            //Z Print Speed.
            xmlWriter.WriteElementString("ZPrintSpeed", materialViewModel.ZPrintSpeed.ToString());

            //X Print Acceleration.
            xmlWriter.WriteElementString("XPrintAcceleration", materialViewModel.XPrintAcceleration.ToString());

            //Y Print Acceleration.
            xmlWriter.WriteElementString("YPrintAcceleration", materialViewModel.YPrintAcceleration.ToString());

            //Z Print Acceleration.
            xmlWriter.WriteElementString("ZPrintAcceleration", materialViewModel.ZPrintAcceleration.ToString());

            //End "Material" outmost element.
            xmlWriter.WriteEndElement();
        }
        #endregion
    }
}
