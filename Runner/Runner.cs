using RabbitMQ.Client;
using System.Configuration;
using RabbitMQReprocessDeadLetter;

namespace Runner
{
    class Runner
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString;
            var factory = new ConnectionFactory
            {
                Uri = connectionString,
                RequestedHeartbeat = 15
                //every N seconds the server will send a heartbeat.  If the connection does not receive a heardbeat within
                //N*2 then the connection is considered dead.
                //suggested from http://public.hudl.com/bits/archives/2013/11/11/c-rabbitmq-happy-servers/
            };
            var connection = factory.CreateConnection();
            var queueName = ConfigurationManager.AppSettings.Get("DeadLetterQueueName");
            var reprocessor = new RabbitReprocessor(connection, queueName);
            reprocessor.StartConsuming();
        }
    }
}
