using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using ECommon.Components;
using ECommon.Configurations;
using ECommon.Dapper;
using ECommon.Logging;
using ECommon.Utilities;
using ENode.Commanding;
using ENode.Configurations;
using ENode.Sample.Commands;
using ENode.Sample.Common;
using ENode.SqlServer;
using EQueue.Configurations;

namespace ENode.Sample.Console.App
{
    class Program
    {
        static ILogger _logger;
        static ENodeConfiguration _configuration;
        static void Main(string[] args)
        {
            ConfigSettings.Initialize();
            InitializeENodeFramework();

            var commandService = ObjectContainer.Resolve<ICommandService>();

            var noteId = ObjectId.GenerateNewStringId();
            var command1 = new CreateNoteCommand { AggregateRootId = noteId, Title = "Sample Title1" };
            var command2 = new ChangeNoteTitleCommand { AggregateRootId = noteId, Title = "Sample Title2" };

            System.Console.WriteLine(string.Empty);

            _logger.Info("Creating Note...");
            commandService.ExecuteAsync(command1, CommandReturnType.EventHandled).Wait();
            _logger.Info("Note create success.");

            System.Console.WriteLine(string.Empty);

            _logger.Info("Updating Note");
            commandService.ExecuteAsync(command2, CommandReturnType.EventHandled).Wait();
            _logger.Info("Note update success.");

            System.Console.WriteLine(string.Empty);

            using (var connection = new SqlConnection(ConfigSettings.ConnectionString))
            {
                var note = connection.QueryList(new { Id = noteId }, ConfigSettings.NoteTable).Single();
                _logger.InfoFormat("Note from ReadDB, id: {0}, title: {1}, version: {2}", note.Id, note.Title, note.Version);
            }

            System.Console.WriteLine(string.Empty);

            _logger.Info("Press Enter to exit...");

            System.Console.ReadLine();
            //_configuration.ShutdownEQueue();

            System.Console.ReadKey();
        }

        private static void InitializeENodeFramework()
        {
            var assemblies = new[]
            {
                Assembly.Load("ENode.Sample.Domain"),
                Assembly.Load("ENode.Sample.Commands"),
                Assembly.Load("ENode.Sample.CommandHandlers"),
                Assembly.Load("ENode.Sample.Denormalizers"),
                Assembly.GetExecutingAssembly()
            };

            var setting = new ConfigurationSetting(ConfigSettings.ConnectionString);

            _configuration = Configuration
                .Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterUnhandledExceptionHandler()               
                .RegisterEQueueComponents()
                .CreateENode(setting)
                .RegisterENodeComponents()

                .UseSqlServerEventStore()
                .UseSqlServerLockService()
                .UseSqlServerPublishedVersionStore()

                .RegisterBusinessComponents(assemblies)             
                
                .UseEQueue()
                .BuildContainer()
                             
                .InitializeBusinessAssemblies(assemblies)

                .InitializeSqlServerPublishedVersionStore()
                .InitializeSqlServerEventStore()
                .InitializeSqlServerLockService()
                .StartEQueue()
                .Start();


            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(typeof(Program).Name);
            _logger.Info("ENode started...");
        }
    }
}
