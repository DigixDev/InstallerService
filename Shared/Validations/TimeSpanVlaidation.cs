using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shared.Validations
{
    public class TimeSpanVlaidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = value.ToString();
            var isValid=TimeSpan.TryParse(text, out var timeSpan);
            return new ValidationResult(isValid, "Data format is not valid");
        }
    }
}
