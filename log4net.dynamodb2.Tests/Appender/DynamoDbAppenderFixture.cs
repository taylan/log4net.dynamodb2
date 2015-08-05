using System;
using System.Reflection;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

// NOTE: This is an integration test that WILL access your AWS account. Be sure you understand the ramifications of this.

namespace log4net.Tests.Appender
{
    [TestFixture]
    public class DynamoDbAppenderFixture
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void WriteLogMessage()
        {
            Config.XmlConfigurator.Configure();

            // an example of how you can append custom properties and pick the up 
            // in the parameter list see the App.config file for pattern. the 
            // readme.md file also has more information about how this works.
            ThreadContext.Properties["log4net:CorrelationId"] = Guid.NewGuid();

            // using a numeric field
            ThreadContext.Properties["log4net:ImportantNumber"] = 42;

            // using a binary field
            ThreadContext.Properties["log4net:ImportantObject"] = new Tuple<string, int>("Number", 42);

            // using a boolean field
            ThreadContext.Properties["log4net:ImportantBoolean"] = true;

            ThreadContext.Properties["log4net:NullValue"] = null; // This field should not exist in DynamoDB. (includeNullValues is false)

            ThreadContext.Properties["log4net:SomeField"] = null; // This field should be visible for this record in DynamoDB with type: NULL (includeNullValues is true)

            Logger.Error("An error occured1.", new ApplicationException("You did something stupid!"));

            ThreadContext.Properties["log4net:NullValue"] = "test";
            ThreadContext.Properties["log4net:ImportantNumber"] = 42.3005d;

            Logger.Error("An error occured2.", new ApplicationException("You did something stupid!"));
            // log it; the actual exception will also be serialized and logged in this configuration
            //Assert.DoesNotThrow(() => Logger.Error("An error occured.", new ApplicationException("You did something stupid!")));
        }
    }
}