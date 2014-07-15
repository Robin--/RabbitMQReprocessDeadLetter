using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQReprocessDeadLetter
{
    public class RabbitReprocessor
    {
        private readonly IConnection _rabbitConnection;
        private readonly IModel _model;
        private readonly string _deadLetterQueueName;
        private const ushort FetchSize = 1;
        private const string ConsumerName = "DeadLettterReprocessor";

        public RabbitReprocessor(IConnection rabbitConnection, string deadLetterQueueName)
        {
            _rabbitConnection = rabbitConnection;
            _deadLetterQueueName = deadLetterQueueName;
            _model = rabbitConnection.CreateModel();
        }

        public void StartConsuming(CancellationTokenSource cancellationTokenSource = null)
        {
            // Configure the Quality of service for the model. Below is how what each setting means.
            // BasicQos(0="Dont send me a new message untill I’ve finshed",  _fetchSize = "Send me N messages at a time", false ="Apply to this Model only")
            _model.BasicQos(0, FetchSize, false);

            var queueingBasicConsumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_deadLetterQueueName, false, ConsumerName, queueingBasicConsumer);

            while (true)
            {
                if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var e = queueingBasicConsumer.Queue.Dequeue(); // blocking call
            }
        }
    }
}
