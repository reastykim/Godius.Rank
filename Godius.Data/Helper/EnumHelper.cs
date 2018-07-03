using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Godius.Data.Helper
{
    public class EnumHelper
    {
        public static string GetEnumDescription<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false
            );

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static T ParseDescriptionToEnum<T>(string description)
        {
            Array array = Enum.GetValues(typeof(T));
            var list = new List<T>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                list.Add((T)array.GetValue(i));
            }

            var dict = list.Select(v => new {
                Value = v,
                Description = GetEnumDescription(v)
            }
                       )
                           .ToDictionary(x => x.Description, x => x.Value);
            return dict[description];
        }

        public static string GetEnumDisplay<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes =
                (DisplayAttribute[])fi.GetCustomAttributes(
                    typeof(DisplayAttribute),
                    false
            );

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].Name;
            }
            else
            {
                return value.ToString();
            }
        }

        public static T ParseDisplayToEnum<T>(string display)
        {
            Array array = Enum.GetValues(typeof(T));
            var list = new List<T>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                list.Add((T)array.GetValue(i));
            }

            var dict = list.Select(v => new { Value = v, Display = GetEnumDisplay(v) })
                           .ToDictionary(x => x.Display, x => x.Value);
            return dict[display];
        }
    }
}
