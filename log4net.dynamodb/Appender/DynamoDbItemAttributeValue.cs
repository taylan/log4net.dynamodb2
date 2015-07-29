using Amazon.DynamoDBv2.Model;

namespace log4net.Appender
{
    public class DynamoDbItemAttributeValue
    {
        public string Name { get; private set; }
        public AttributeValue Value { get; private set; }

        public DynamoDbItemAttributeValue(string name, AttributeValue value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
