using Actors;
using Akka.Actor;
using Akka.Routing;
using Messages;
using Sample.Actors;
using Sample.Messages;
using Sample.Projections;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        private static ActorSystem Sys = null;
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                //.WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"D:\GitHub\Akka.Cassandra.Cqrs.Core\Sample\bin\Debug\Log-{Date}.log", fileSizeLimitBytes: 102400000, retainedFileCountLimit: 5, shared: true, buffered: false)
                .MinimumLevel.Information()
                .CreateLogger();

            Serilog.Log.Logger = logger;
            Sys = ActorSystem.Create("sampleweb");

            SystemActors.CommandProcessor = Sys.ActorOf(Props.Create(() => new AggregateRootCoordinatorActor()), "command-proc");
            //SystemActors.CommandProcessor = Sys.ActorOf(Props.Create(() => new AggregateRootCoordinatorActor()).WithRouter(new RoundRobinPool(10)), "command-proc");
            //Guid newAccount1 = Guid.NewGuid();
            //SystemActors.CommandProcessor.Tell(new CreateAccount(newAccount1, "123456", "TestEntityName", Domain.AccountType.Business));
            //Guid newAccount2 = Guid.NewGuid();
            //SystemActors.CommandProcessor.Tell(new CreateAccount(newAccount2, "654321", "Joe Shmoe", Domain.AccountType.Individual));
            //SystemActors.CommandProcessor.Tell(new UpdateAccount(newAccount2, "654321", "Joe Shmoe2", Domain.AccountType.Individual));
            //Domain.Address addr = new Domain.Address();
            //addr.StreetAddress1 = "1234th St";
            //SystemActors.CommandProcessor.Tell(new UpdateAccountMailingAddress(newAccount2, addr));

            //warm up the dictionary of iactorrefs
            for (int i=0;i<100000;i++)
            {
                Guid newAccount = Guid.NewGuid();
                SystemActors.CommandProcessor.Tell(new CreateAccount(newAccount, "taxid-"+i, "TestEntityName"+i, Domain.AccountType.Business));
                if(i%500 == 0)
                    Thread.Sleep(TimeSpan.FromMilliseconds(300));
                //SystemActors.CommandProcessor.Tell(new UpdateAccount(newAccount, "taxid-" + i, "John Doe"+i, Domain.AccountType.Individual));
            }

            //for (int i = 0; i < 1000; i++)
            //{
            //    Guid newAccount = Guid.NewGuid();
            //    SystemActors.CommandProcessor.Tell(new CreateAccount(newAccount, "taxid-" + i, "TestEntityName" + i, Domain.AccountType.Business));
            //    SystemActors.CommandProcessor.Tell(new UpdateAccount(newAccount, "taxid-" + i, "John Doe" + i, Domain.AccountType.Individual));
            //}
            //eventually consistent...wait just a bit!
            Thread.Sleep(20000);
            //CommandHandled response = _currentCommandActor.Ask<CommandHandled>(new HandleCommand(command, inputString,Self, _currentKnowledgeFile,originator), TimeSpan.FromSeconds(10)).Result;
            //ProjectionResult<AccountProjectionState> result = SystemActors.CommandProcessor.Ask<ProjectionResult<AccountProjectionState>>(new GetProjection(newAccount1), TimeSpan.FromSeconds(60)).Result;
            //Console.WriteLine("Name1:" + result.Projection.Account.EntityName);
            //Console.WriteLine("TaxNumber1:" + result.Projection.Account.TaxNumber);
            ////Console.WriteLine("MailingAddress1:" + result.Projection.Account.MailingAddress.StreetAddress1);
            //Console.WriteLine("Version1: " +result.Projection.Version);

            //ProjectionResult<AccountProjectionState> result2 = SystemActors.CommandProcessor.Ask<ProjectionResult<AccountProjectionState>>(new GetProjection(newAccount2), TimeSpan.FromSeconds(60)).Result;
            //Console.WriteLine("Name2:" + result2.Projection.Account.EntityName);
            //Console.WriteLine("TaxNumber2:" + result2.Projection.Account.TaxNumber);
            //Console.WriteLine("MailingAddress2:" + result2.Projection.Account.MailingAddress.StreetAddress1);
            //Console.WriteLine("Version2: " + result2.Projection.Version);

            for (int x = 0; x < 40; x++)
            {
                IndexProjectionResult<AccountIndexProjectionState> iResult = SystemActors.CommandProcessor.Ask<IndexProjectionResult<AccountIndexProjectionState>>(new GetIndexProjection(NameRegistry.AccountIndexProjection), TimeSpan.FromSeconds(60)).Result;
                DumpIndexProjectionToConsole(iResult.Projection);
                Thread.Sleep(5000);
            }

            Console.ReadKey();
            Sys.WhenTerminated.Wait(3000);
        }

        private static void DumpIndexProjectionToConsole(AccountIndexProjectionState state)
        {
            //foreach (AccountIndexItem o in state.Accounts.Values.ToArray())
            //{
            //    Console.WriteLine("AccountID:{0} Name:{1} Type:{2} TaxId:{3}", o.AccountId, o.EntityName, o.AccountType, o.TaxNumber);
            //}
            Console.WriteLine(DateTime.UtcNow.ToString() + " Total Accounts = " +state.Accounts.Values.Count);
        }
    }
}
