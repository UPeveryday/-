using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.Servers.CommunicationProtocol
{
    public interface ICommunicationProtocol
    {
        StataTwo ReadStataTwo();
        Task<StataThree> ReadStataThree();
        Task<StataThree> ReadStataThree(int num);
        Task<bool> SetTestPra(TestKind testKind, byte ClickNum);
        Task<string> GetCgfVolate();
        Task<bool> ThicknessAdjustable(bool Adjustt);
        Task<bool> GetPowerStata();
        Task<double> GetCgfVolateDouble();
    }

    public struct StataTwo
    {
        public double ACurrent;
        public double AVolate;
        public double BCurrent;
        public double BVolate;
        public double CCurrent;
        public double CVolate;
        public Methonstata stata;
    }
    public struct StataThree
    {
        public double ACurrent;
        public double AVolate;
        public double APower;
        public double BCurrent;
        public double BVolate;
        public double BPower;
        public double CCurrent;
        public double CVolate;
        public double CPower;
        public double Fre;
        public bool Checked;
    }

    public enum Methonstata
    {
        Free = 0,
        AutoUpCurrent = 1,
        AutoDownCurrent = 2,
        LongTimeNoCurrent = 3,
        False = 4
    }
}
