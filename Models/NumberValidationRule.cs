using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PortableEquipment.Models
{
    public class NumberValidationRule : ValidationRule
    {
        private int _min;
        private int _max;

        public NumberValidationRule()
        {

        }

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int age = 0;
            try
            {
                if (((string)value).Length > 0)
                    age = int.Parse((string)value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "输入的数字无效！");
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false,
                  "输入的年龄范围必须在: " + Min + " - " + Max + "之间");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
