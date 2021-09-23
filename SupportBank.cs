using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    class SupportBank
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<Transaction> Transactions { get; set; }
        Dictionary<string, Account> NameToAccount { get; set; }

        public SupportBank()
        {
            Transactions = SupportBank.GetTransactions("./data/DodgyTransactions2015.csv");
            NameToAccount = GetAccountsDictionary();
        }

        private static IEnumerable<Transaction> GetTransactions(string fileName) => File.ReadAllLines(fileName)
                                                                                    .Skip(1)
                                                                                    .Select(row => row.Split(","))
                                                                                    .Select(row => Transaction.FromArray(row))
                                                                                    .Where(t => t != null);
                                    

        private Dictionary<string, Account> GetAccountsDictionary()
        {
            Dictionary<string, Account> nameToAccount = new Dictionary<string, Account>();

            foreach ((string fromUser, string toUser, decimal amount) in Transactions)
            {
                 if (!nameToAccount.ContainsKey(fromUser)) 
                 {
                     nameToAccount.Add(fromUser, new Account(fromUser));
                 }
                 if (!nameToAccount.ContainsKey(toUser)) 
                 {
                     nameToAccount.Add(toUser, new Account(toUser));
                 }
                nameToAccount[fromUser].Balance = nameToAccount[fromUser].Balance + amount;
                nameToAccount[toUser].Balance = nameToAccount[toUser].Balance - amount;
            }
            return nameToAccount;
        }

        void WriteEnumerableToConsole<T>(IEnumerable<T> enumerable)
        {
            foreach (var element in enumerable)   
            {
                Console.WriteLine(element);
            }
        }

        void ListAllAccounts() => WriteEnumerableToConsole(NameToAccount.Values);

        void ListAccountTransactions(string name) => WriteEnumerableToConsole(Transactions.Where(transaction => transaction.FromUser.Equals(name) || transaction.ToUser.Equals(name)));

        string GetValidAccountName()
        {
            Console.WriteLine("\nPlease enter the account holder's name: ");
                    string name = Console.ReadLine();
                    while(!NameToAccount.ContainsKey(name))
                    { 
                        Console.WriteLine("Invalid name. Please enter a valid name.");
                        name = Console.ReadLine();
                    }

                    return name;
        }

        int GetUserOptions()
        {
                Console.WriteLine("\nWhat would you like to do today?");
                Console.WriteLine(" 1) See all accounts.\n 2) See account transactions.");

                int userOption;
                string firstInput = Console.ReadLine(); // String from user
                while (!(int.TryParse(firstInput, out userOption) && userOption == 1 || userOption == 2))
                {
                    Console.WriteLine("Please enter a valid option 1 or 2.");
                    firstInput = Console.ReadLine();
                }

                return userOption;
        }

        bool HasUserSaidYes()
        {
            var answer = Console.ReadLine();
            var yesAnswers = new List<string>() {"yes", "Yes", "y", "Y", "yeah", "Yeah", "YES"};
            return yesAnswers.Contains(answer);
        }

        void RunSupportBank()
        {
            Console.WriteLine("Welcome to Support Bank.");
            while (true)
            {     
                if(GetUserOptions() == 1)
                {
                    ListAllAccounts(); // No class specified because method is not static
                }
                else 
                {
                    ListAccountTransactions(GetValidAccountName());
                }

                Console.WriteLine("\nWould you like to do something else today? y/n");
                if (!HasUserSaidYes())
                {
                    break;
                }
            }

            Console.WriteLine("Have a nice day!");
        }


        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Work\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            SupportBank bank = new SupportBank();
            bank.RunSupportBank();
        }
    }
}