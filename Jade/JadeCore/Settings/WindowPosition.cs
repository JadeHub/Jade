using System;
using System.Configuration;
using System.ComponentModel;

namespace JadeCore.Settings
{
    public class WindowPositionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {                
                string[] parts = ((string)value).Split(new char[] { ':' });
                if(parts.Length != 5)
                {
                    throw new ArgumentException("Can't parse WindowPosition.");
                }
                WindowPosition pos = new WindowPosition();
                pos.Origin.X = Convert.ToInt32(parts[0]);
                pos.Origin.Y = Convert.ToInt32(parts[1]);
                pos.Size.Width = Convert.ToInt32(parts[2]);
                pos.Size.Height = Convert.ToInt32(parts[3]);
                pos.Maximised = Convert.ToBoolean(parts[4]);
                return pos;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                WindowPosition p = value as WindowPosition;                
                return string.Format("{0}:{1}:{2}:{3}:{4}", p.Origin.X, p.Origin.Y, p.Size.Width, p.Size.Height, p.Maximised.ToString());
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(WindowPositionConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public class WindowPosition
    {
        public WindowPosition()
        {
            Origin.X = Origin.Y = Size.Width = Size.Height = 0;
            Maximised = false;
        }

        public System.Drawing.Point Origin;
        public System.Drawing.Size Size;
        public bool Maximised;
    }
}
