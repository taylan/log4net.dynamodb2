﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace log4net.Appender
{
    /// <summary>
    /// A log4net appender that writes log output to an Amazon Web Services DynamoDb table.
    /// </summary>
    public class DynamoDbAppender : BufferingAppenderSkeleton, IDisposable
    {
        /// <summary>
        /// Default table name. Will be used if <c>TableName</c> is not specified in configuration.
        /// </summary>
        protected const string DefaultTableName = "log4net";

        /// <summary>
        /// Amazon DynamoDb data writer.
        /// </summary>
        protected DynamoDbDataWriter DataWriter;

        /// <summary>
        /// Gets or sets the DynamoDb domain/table name. Defaults to "log4net".
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which DynamoDb service endpoint to use. Uses the <see cref="DynamoDbDataWriter"/> default if none is specified.
        /// </summary>
        /// <value>The DynamoDb service endpoint (with the "http://" or "https://" protocol specifier).</value>
        public virtual string ServiceEndpoint { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<DynamoDbAppenderParameter> Parameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDbAppender"/> class.
        /// </summary>
        /// <remarks>Empty default constructor</remarks>
        public DynamoDbAppender()
        {
            Parameters = new List<DynamoDbAppenderParameter>();
            TableName = DefaultTableName;
        }

        /// <summary>
        /// Adds a parameter to the query.
        /// </summary>
        /// <param name="parameter">The parameter to add to the query.</param>
        public void AddParameter(DynamoDbAppenderParameter parameter)
        {
            if (null == parameter)
            {
                return;
            }

            Parameters.Add(parameter);
        }

        /// <summary>
        /// Sends the events.
        /// </summary>
        /// <param name="events">The events that need to be sent.</param>
        /// <remarks>
        /// The subclass must override this method to process the buffered events.
        /// </remarks>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            if (DataWriter == null)
            {
                DataWriter = new DynamoDbDataWriter(ServiceEndpoint, TableName);
            }

            List<Dictionary<string, AttributeValue>> itemList = new List<Dictionary<string, AttributeValue>>();
            foreach (LoggingEvent loggingEvent in events)
            {
                itemList.Add(Parameters.Select(param => param.GetItemAttribute(loggingEvent)).ToDictionary(t => t.Name, t => t.Value));
            }

            DataWriter.WriteItems(itemList);
        }

        /// <summary>
        /// Close this appender instance.
        /// </summary>
        /// <remarks>
        /// Close this appender instance. If this appender is marked
        /// as not <see cref="P:log4net.Appender.BufferingAppenderSkeleton.Lossy"/> then the remaining events in
        /// the buffer must be sent when the appender is closed.
        /// </remarks>
        protected override void OnClose()
        {
            DisposeDataWriter();
            base.OnClose();
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
            if (disposing)
            {
                DisposeDataWriter();
            }
        }

        /// <summary>
        /// Disposes the AWS data writer.
        /// </summary>
        protected virtual void DisposeDataWriter()
        {
            if (DataWriter == null)
                return;
            DataWriter.Dispose();
            DataWriter = null;
        }
    }
}