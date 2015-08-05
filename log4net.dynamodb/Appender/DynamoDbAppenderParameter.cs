using Amazon.DynamoDBv2.Model;
using log4net.Core;
using log4net.Layout;

namespace log4net.Appender
{
    /// <summary>
    /// Parameter type used by the <see cref="DynamoDbAppender"/>.
    /// </summary>
    public class DynamoDbAppenderParameter
    {
        /// <summary>
        /// DyanmoDb parameter types.
        /// </summary>
        public enum ParameterType
        {
            /// <summary>
            /// String type. This is the default parameter type.
            /// </summary>
            S = 0,

            /// <summary>
            /// Numeric type.
            /// </summary>
            N = 2,

            /// <summary>
            /// Binary type.
            /// </summary>
            B = 4,

            /// <summary>
            /// Boolean type. NULL values for this type will be treated as <c>false</c>.
            /// </summary>
            BOOL = 8
        }

        /// <summary>
        /// Gets or sets the name of this parameter.
        /// </summary>
        /// <value>
        /// The name of this parameter.
        /// </value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameter type. Synonymous with DynamoDb field type identifiers.
        /// </summary>
        /// <value>
        /// The type of this parameter.
        /// </value>
        public virtual ParameterType Type { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRawLayout"/> to use to render the logging event into an object for this parameter.
        /// </summary>
        /// <value>
        /// The <see cref="IRawLayout"/> used to render the logging event into an object for this parameter.
        /// </value>
        /// <remarks>
        /// <para>
        /// The <see cref="IRawLayout"/> that renders the value for this parameter.
        /// </para>
        /// <para>
        /// The <see cref="RawLayoutConverter"/> can be used to adapt any <see cref="ILayout"/> into a <see cref="IRawLayout"/> for use in the property.
        /// </para>
        /// </remarks>
        public IRawLayout Layout { get; set; }

        /// <summary>
        /// Specifies whether null values should be written to DynamoDb or whether they should be ignored.
        /// If this is set to <c>true</c>, and the value for this parameter is null, this property will be saved to DynamoDb and will have type 'NULL: true'.
        /// If this is set to <c>false</c>, this property will not be saved to DynamoDb when its value is null.
        /// Please see <see cref="http://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DataModel.html">DynamoDb Data Model documentation</see> for more info.
        /// </summary>
        public virtual bool IncludeNullValues { get; set; }

        public virtual DynamoDbItemAttribute GetItemAttribute(LoggingEvent loggingEvent)
        {
            object formattedValue = Layout.Format(loggingEvent);
            return new DynamoDbItemAttribute(this.Name, this.BuildAttributeValue(formattedValue));
        }

        protected AttributeValue BuildAttributeValue(object logItem)
        {
            DynamoDbAttributeBuilder builder = new DynamoDbAttributeBuilder(this.IncludeNullValues);
            switch (Type)
            {
                case ParameterType.B:
                    return builder.BuildAttributeForTypeBinary(logItem);
                case ParameterType.N:
                    return builder.BuildAttributeForTypeNumeric(logItem);
                case ParameterType.BOOL:
                    return builder.BuildAttributeForTypeBoolean(logItem);
                default:
                    return builder.BuildAttributeForTypeString(logItem);
            }
        }
    }
}
