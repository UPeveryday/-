using LiveCharts;
using LiveCharts.Defaults;
using PortableEquipment.ViewModels;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortableEquipment.TestParameters
{

    public enum TestLebel
    {
        _35KV=0,
        _66KV1,
        _110KV=2,
        _220KV=3
    }
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
        public int TestLevel;
        public double Humidity;
        public double Temperature;
        public string TestLocation;
        public string Tester;
        public InducedOvervoltage InducedOvervoltageR;
        public InducedOvervoltageResult inducedOvervoltageResult;
        public ExcitationCharacteristic ExcitationCharacteristicR;
        public ExcitationCharacteristicResult excitationCharacteristicResult;
        public NoLoadCurrent NoLoadCurrentR;
        public NoLoadCurrentResult noLoadCurrentResult;

        public DateTime DateTime;

        public ChartValues<ObservablePoint> Chartvalues;
        public BindableCollection<LcdatagridColletion> LcDatagrid;


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
        High = 0,
        Low = 1
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
        public double OverCurrent;

    }
    public struct InducedOvervoltageResult
    {
        public bool Finish;
        public double Volate;
        public double Cueent;
        public double Fre;
        public double TestTime;
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
        public double VariableThan;//变比
    }
    public struct ExcitationCharacteristicResult
    {
        public ChartValues<ObservablePoint> Chartvalues;
        public BindableCollection<LcdatagridColletion> LcDatagrid;
        public string GuaiVolate;
        public string GuaiCueent;
        public bool Finish;
    }
    /// <summary>
    /// 空载电流
    /// </summary>
    public struct NoLoadCurrent
    {
        public bool Enable;
        public double TestVolate;
        public double OverCurrent;
        public double VariableThan;//变比

    }
    public struct NoLoadCurrentResult
    {
        public double Volate;
        public double Current;
        public bool Finish;

    }
        public enum DetectionType
    {
        HighpressureResistanceTest = 0,
        LowessureResistanceTest = 1
    }




}
