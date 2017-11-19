using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using ECommon.Components;
using ECommon.Logging;
using ECommon.Scheduling;
using ECommon.Socketing;
using ENode.Commanding;
using ENode.Configurations;
using ENode.EQueue;
using ENode.Eventing;
using ENode.Infrastructure;
using ENode.Sample.Common;
using EQueue.Broker;
using EQueue.Configurations;
using EQueue.NameServer;

namespace ENode.Sample.Console.App
{
    public static class ENodeExtensions
    {
        private static NameServerController _nameServer;
        private static BrokerController _broker;

        private static CommandService _commandService;
        private static CommandConsumer _commandConsumer;

        private static DomainEventPublisher _eventPublisher;     
        private static DomainEventConsumer _eventConsumer;


        public static ENodeConfiguration BuildContainer(this ENodeConfiguration enodeConfiguration)
        {
            enodeConfiguration.GetCommonConfiguration().BuildContainer();
            return enodeConfiguration;
        }

        public static ENodeConfiguration UseEQueue(this ENodeConfiguration enodeConfiguration)
        {

            _commandService = new CommandService();
            _eventPublisher = new DomainEventPublisher();

            var assemblies = new[] { Assembly.GetExecutingAssembly() };
            enodeConfiguration.RegisterTopicProviders(assemblies);

            var configuration = enodeConfiguration.GetCommonConfiguration();
            configuration.RegisterEQueueComponents();

            configuration.SetDefault<ICommandService, CommandService>(_commandService);
            configuration.SetDefault<IMessagePublisher<DomainEventStreamMessage>, DomainEventPublisher>(_eventPublisher);

            return enodeConfiguration;
        }

        public static ENodeConfiguration StartEQueue(this ENodeConfiguration enodeConfiguration)
        {
            //var configuration = enodeConfiguration.GetCommonConfiguration();
            _commandService.Initialize(
                new CommandResultProcessor().Initialize(
                    new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.BrokerCommandPort)));
            _eventPublisher.Initialize();

            _commandConsumer = new CommandConsumer().Initialize().Subscribe(EQueueTopics.NoteCommandTopic);
            _eventConsumer = new DomainEventConsumer().Initialize().Subscribe(EQueueTopics.NoteEventTopic);

            var brokerStorePath = @"d:\note-sample-equeue-store";
            if (Directory.Exists(brokerStorePath))
            {
                Directory.Delete(brokerStorePath, true);
            }

            _nameServer = new NameServerController();
            _broker = BrokerController.Create();

            _nameServer.Start();
            _broker.Start();
            _commandService.Start();
            _eventConsumer.Start();
            _commandConsumer.Start();
            _eventPublisher.Start();
            
            WaitAllConsumerLoadBalanceComplete();

            return enodeConfiguration;
        }

        private static void WaitAllConsumerLoadBalanceComplete()
        {
            var logger = ObjectContainer.Resolve<ILoggerFactory>().Create(typeof(ENodeExtensions).Name);
            var scheduleService = ObjectContainer.Resolve<IScheduleService>();
            var waitHandle = new ManualResetEvent(false);
            logger.Info("Waiting for all consumer load balance complete, please wait for a moment...");
            scheduleService.StartTask("WaitAllConsumerLoadBalanceComplete", () =>
            {
                var eventConsumerAllocatedQueues = _eventConsumer.Consumer.GetCurrentQueues();
                var commandConsumerAllocatedQueues = _commandConsumer.Consumer.GetCurrentQueues();
                if (eventConsumerAllocatedQueues.Count() == 4 && commandConsumerAllocatedQueues.Count() == 4)
                {
                    waitHandle.Set();
                }
            }, 1000, 1000);

            waitHandle.WaitOne();
            scheduleService.StopTask("WaitAllConsumerLoadBalanceComplete");
            logger.Info("All consumer load balance completed.");
        }
    }
}