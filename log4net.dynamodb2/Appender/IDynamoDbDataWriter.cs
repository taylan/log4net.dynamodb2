using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace log4net.Appender
{
    public interface IDynamoDbDataWriter : IDisposable
    {
        /// <summary>
        /// Writes the specified items to DynamoDb with a batch write.
        /// </summary>
        /// <param name="items">The items.</param>
        void WriteItems(List<Dictionary<string, AttributeValue>> items);
    }
}