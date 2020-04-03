using PortableEquipment.ViewModels;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.TestParameters
{
    /// <summary>
    /// 变压器实验
    /// </summary>
    public struct Translator
    {
        public string TestId;
        public double RatedVoltage;
        public string RatedCapacity;
        public string WindingGroup;
        public double Temperature;
        public double Humidity;
        public string TestLocation;
        public string Tester;
        public double Frequency;
        public double Volate;
        public double Current;
        public BindableCollection<TransformerDataStep> DatagridData;
        public DateTime DateTime;
        public BindableCollection<TransformerTestData> TestResultData;
    }
    /// <summary>
    /// 互感器实验
    /// </summary>
    public struct MutualTranslator
    {
        public string TestId;
        public string TestLevel;
        public double Humidity;
        public double Temperature;
        public string TestLocation;
        public string Tester;
        public InducedOvervoltage InducedOvervoltageR;
        public ExcitationCharacteristic ExcitationCharacteristicR;
        public NoLoadCurrent NoLoadCurrentR;
        public DateTime DateTime;
    }
    /// <summary>
    /// 手动调压
    /// </summary>
    public struct TestByHand
    {
        public VolateKind Volate;
        public CurrentKind Current;
        public double OverVolate;
        public double OverCurrent;
        public double VariableThan;//变比
        public double Promotion;//荣升系数
        public DateTime DateTime;

    }

    public enum VolateKind
    {
        High=0,
        Low=1
    }
    public enum CurrentKind
    {
        Big = 0,
        Small = 1
    }
    /// <summary>
    /// 感应耐压
    /// </summary>
    public struct InducedOvervoltage
    {
        public bool Enable;
        public double OutgoingTestVoltage;
        public DetectionType detectionType;
        public double TestVoltage;
        public double HighOverVolate;//高压过压值
        public double TestFre;
        public double TestTime;
        public double VariableThan;//变比
        public double Promotion;//荣升系数
    }



    /// <summary>
    /// 励磁特性
    /// </summary>
    public struct ExcitationCharacteristic
    {
        public bool Enable;
        public double TestVolate;
        public double OverCurrent;
        public double[] VolateRange;
    }
    /// <summary>
    /// 空载电流
    /// </summary>
    public struct NoLoadCurrent
    {
        public bool Enable;
        public double TestVolate;
        public double OverCurrent;
    }
    public enum DetectionType
    {
        HighpressureResistanceTest = 0,
        LowessureResistanceTest = 1
    }
}
