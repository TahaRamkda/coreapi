using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Helper
{
    public class PropertyByName<T>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="func">Feature property access</param>
        public PropertyByName(string propertyName, Func<T, object> func = null)
        {
            PropertyName = propertyName;
            GetProperty = func;

            PropertyOrderPosition = 1;
        }

        /// <summary>
        /// Property order position
        /// </summary>
        public int PropertyOrderPosition { get; set; }

        /// <summary>
        /// Feature property access
        /// </summary>
        public Func<T, object> GetProperty { get; private set; }

        /// <summary>
        /// Property name
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Property value
        /// </summary>
        public object PropertyValue { get; set; }

        /// <summary>
        /// Converted property value to Int32
        /// </summary>
        public int IntValue
        {
            get
            {
                int rez;
                if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out rez))
                    return default;
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to bool
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                bool rez;
                if (PropertyValue == null || !bool.TryParse(PropertyValue.ToString(), out rez))
                    return default;
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to string
        /// </summary>
        public string StringValue
        {
            get { return PropertyValue == null ? string.Empty : Convert.ToString(PropertyValue); }
        }

        /// <summary>
        /// Converted property value to decimal
        /// </summary>
        public decimal DecimalValue
        {
            get
            {
                decimal rez;
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out rez))
                    return default;
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to decimal?
        /// </summary>
        public decimal? DecimalValueNullable
        {
            get
            {
                decimal rez;
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out rez))
                    return null;
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to double
        /// </summary>
        public double DoubleValue
        {
            get
            {
                double rez;
                if (PropertyValue == null || !double.TryParse(PropertyValue.ToString(), out rez))
                    return default;
                return rez;
            }
        }

        /// <summary>
        /// Converted property value to DateTime?
        /// </summary>
        public DateTime? DateTimeNullable
        {
            get { return PropertyValue == null ? null : DateTime.Parse(PropertyValue.ToString()) as DateTime?; }
        }

        public override string ToString()
        {
            return PropertyName;
        }
    }
}
