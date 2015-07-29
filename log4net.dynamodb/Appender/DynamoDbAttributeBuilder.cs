using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Amazon.DynamoDBv2.Model;
using Microsoft.VisualBasic;
using log4net.Core;

namespace log4net.Appender
{
    /// <summary>
    /// Builds DynamoDb attributes for <see cref="LoggingEvent"/> items based on the persistence field type.
    /// </summary>
    public class DynamoDbAttributeBuilder
    {
        private readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Builds the attribute for a binary field type.
        /// </summary>
        /// <param name="logItem">The item that will be logged.</param>
        /// <returns>A properly configured <see cref="AttributeValue"/> containing the serialized item.</returns>
        public virtual AttributeValue BuildAttributeForTypeBinary(object logItem)
        {
            if (logItem == null)
                return new AttributeValue { NULL = true };

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, logItem);
                return new AttributeValue { B = ms };
            }
        }

        /// <summary>
        /// Builds the attribute for a string field type.
        /// </summary>
        /// <param name="logItem">The item that will be logged.</param>
        /// <returns>A properly configured <see cref="AttributeValue"/> containing the string representation of the item.</returns>
        public virtual AttributeValue BuildAttributeForTypeString(object logItem)
        {
            if (logItem == null)
                return new AttributeValue { NULL = true };

            return new AttributeValue { S = logItem.ToString() };
        }

        /// <summary>
        /// Builds the attribute for a numeric field type.
        /// </summary>
        /// <param name="logItem">The item that will be logged.</param>
        /// <returns>A properly configured <see cref="AttributeValue"/> containing the numeric representation of the item.</returns>
        public virtual AttributeValue BuildAttributeForTypeNumeric(object logItem)
        {
            if (logItem == null)
                return new AttributeValue { NULL = true };

            decimal logItemValue;
            if(!decimal.TryParse(logItem.ToString(), NumberStyles.Float, this.invariantCulture, out logItemValue))
            {
                throw new ArgumentException(Properties.Resources.ItemNotNumeric);
            }

            int decimalPlaces = Math.Floor(logItemValue) == logItemValue
                ? 0
                : logItemValue.ToString(this.invariantCulture).Split('.')[0].TrimEnd('0').Length;

            return new AttributeValue { N = logItemValue.ToString(string.Format("F{0}", decimalPlaces), this.invariantCulture) };
        }
    }
}