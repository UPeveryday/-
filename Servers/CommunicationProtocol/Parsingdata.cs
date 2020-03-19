using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CommunicationProtocol
{
    public class Parsingdata : Screen, IParsingdata
    {
        [Inject]
        StyletLogger.ILogger _logger;
        public StataTwo ParsingTwo(byte[] data)
        {
            StataTwo stataTwo = new StataTwo();
            try
            {
                stataTwo.AVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(2).Take(4).ToArray()));
                stataTwo.ACurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(7).Take(4).ToArray()));
                stataTwo.BVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(12).Take(4).ToArray()));
                stataTwo.BCurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(17).Take(4).ToArray()));
                stataTwo.CVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(22).Take(4).ToArray()));
                stataTwo.CCurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(27).Take(4).ToArray()));
                stataTwo.stata = Getstata(data[32]);
            }
            catch
            {
                stataTwo.stata = Methonstata.False;
                _logger.Writer("Parsingdata: " + new System.Diagnostics.StackTrace(1, true).GetFrame(0).GetFileLineNumber().ToString() + " 行" + "。 解析ParsingTwo出错");
            }
            return stataTwo;
        }

        public StataThree ParsingThree(byte[] data)
        {
            StataThree stataThree = new StataThree();
            try
            {
                stataThree.AVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(2).Take(4).ToArray()));
                stataThree.ACurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(7).Take(4).ToArray()));
                stataThree.APower = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(12).Take(4).ToArray()));


                stataThree.BVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(17).Take(4).ToArray()));
                stataThree.BCurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(22).Take(4).ToArray()));
                stataThree.BPower = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(27).Take(4).ToArray()));

                stataThree.CVolate = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(32).Take(4).ToArray()));
                stataThree.CCurrent = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(37).Take(4).ToArray()));
                stataThree.CPower = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(42).Take(4).ToArray()));

                stataThree.Fre = System.Convert.ToDouble(Encoding.Default.GetString(data.Skip(47).Take(4).ToArray()));
                stataThree.Checked = Models.StaticClass.Checkdata(data);
            }
            catch
            {
                _logger.Writer("Parsingdata: " + new System.Diagnostics.StackTrace(1, true).GetFrame(0).GetFileLineNumber().ToString() + " 行" + "。 解析ParsingThree出错");
            }
            return stataThree;
        }

        public Methonstata Getstata(byte stata)
        {
            if (stata == 0x00)
                return Methonstata.Free;
            else if (stata == 0x01)
                return Methonstata.AutoUpCurrent;
            else if (stata == 0x02)
                return Methonstata.AutoDownCurrent;
            else if (stata == 0x03)
                return Methonstata.LongTimeNoCurrent;
            else
            {
                return Methonstata.False;

            }
        }
    }
}
