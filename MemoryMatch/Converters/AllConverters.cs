using MemoryMatch.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryMatch.Converters
{
    public class StringEqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBool = value is bool;
            if (isBool)
            {
                bool boolValue = (bool)value;
                if (boolValue)
                {
                    return parameter;
                }
            }

            return Binding.DoNothing;
        }
    }
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBool = value is bool;
            bool isTrue = isBool && (bool)value;
            bool hasParameter = parameter != null;
            bool isEnum = targetType.IsEnum;
            
            if (isTrue && hasParameter && isEnum)
            {
                return Enum.Parse(targetType, parameter.ToString());
            }

            return Binding.DoNothing;
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBool = value is bool;
            if (isBool)
            {
                bool boolValue = (bool)value;
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBool = value is bool;
            if (isBool)
            {
                bool boolValue = (bool)value;
                return !boolValue;
            }
            return value;
        }
    }

    public class GameResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBool = value is bool;
            if (isBool)
            {
                bool isWon = (bool)value;
                if (isWon)
                {
                    return "Won";
                }
                else
                {
                    return "Lost";
                }
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
} 