using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace log4net.Appender
{
    /// <summary>
    /// Data writer for persisting data to Amazon DynamoDb tables.
    /// </summary>
    public class DynamoDbDataWriter : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Default service endpoint. Will be used if none is specified.
        /// </summary>
        protected const string DefaultServiceEndpoint = "https://dynamodb.us-east-1.amazonaws.com";

        /// <summary>
        /// Amazon DynamoDb client.
        /// </summary>
        protected AmazonDynamoDBClient DynamoDbClient;

        /// <summary>
        /// Table
        /// </summary>
        protected Table LogTable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDbDataWriter"/> class.
        /// </summary>
        public DynamoDbDataWriter() : this(DefaultServiceEndpoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDbDataWriter"/> class.
        /// </summary>
        /// <param name="endpoint">The AWS DynamoDb service endpoint.</param>
        /// <param name="tableName">Name of the table used for logging.</param>
        public DynamoDbDataWriter(string endpoint, string tableName="")
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                endpoint = DefaultServiceEndpoint;
            }

            DynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig {ServiceURL = endpoint});
            LogTable = Table.LoadTable(DynamoDbClient, tableName);
        }

        /// <summary>
        /// Writes the specified items to DynamoDb with a batch write.
        /// </summary>
        /// <param name="items">The items.</param>
        public virtual void WriteItems(List<Dictionary<string, AttributeValue>> items)
        {
            DocumentBatchWrite batchWrite = this.LogTable.CreateBatchWrite();
            items.ForEach(i => batchWrite.AddDocumentToPut(Document.FromAttributeMap(i)));
            batchWrite.Execute();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (!disposing)
                return;

            if(this.DynamoDbClient != null)
                this.DynamoDbClient.Dispose();
            disposed = true;
        }
    }
}