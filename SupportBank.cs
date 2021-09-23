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

        public List<Transaction> Transactions { get; set; }
        public List<Account> Accounts { get; set; }

        public SupportBank()
        {
            Transactions = SupportBank.GetTransactions("./data/Transactions2014.csv");
            Accounts = GetAccounts();
        }

        public static List<Transaction> GetTransactions(string fileName) => File.ReadAllLines(fileName)
                                                                                    .Skip(1)
                                                                                    .Select(row => row.Split(","))
                                                                                    .Select(row => new Transaction(Convert.ToDateTime(row[0]), row[1], row[2], row[3], Convert.ToDecimal(row[4])))
                                                                                    .ToList(); // We could have an Enumerable by deleting this line

        public List<Account> GetAccounts()
        {
            Dictionary<string, Account> nameToAccount = new Dictionary<string, Account>();

            foreach ((string fromUser, string toUser, decimal amount) in Transactions)
            {
                // Check if account exists:
                 if (!nameToAccount.ContainsKey(fromUser)) 
                 {
                     nameToAccount.Add(fromUser, new Account(fromUser));
                 }

                 if (!nameToAccount.ContainsKey(toUser)) 
                 {
                     nameToAccount.Add(toUser, new Account(toUser));
                 }

                // Update balances
                nameToAccount[fromUser].Balance = nameToAccount[fromUser].Balance + amount;
                nameToAccount[toUser].Balance = nameToAccount[toUser].Balance - amount;

            }

            return nameToAccount.Values.ToList();
        }

        void ListAllAccounts()
        {
            Accounts.ForEach(Console.WriteLine);
        }

        void ListAccountTransactions(string name)
        {
            List<Transaction> filteredTransactions = Transactions.Where(transaction => transaction.FromUser.Equals(name) || transaction.ToUser.Equals(name)).ToList();
            filteredTransactions.ForEach(Console.WriteLine);
        }

        void FetchUserAccountName()
        {
            Console.WriteLine("\nPlease enter the account holder's name: ");
                    string name = Console.ReadLine();
                    while(!Accounts.Any(account => account.Name == name))
                    { 
                        Console.WriteLine("Invalid name. Please enter a valid name.");
                        name = Console.ReadLine();
                    }

                    ListAccountTransactions(name);
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


        void RunSoftBank()
        {
            Console.WriteLine("Welcome to Support Bank.");

            while (true)
            {     
                int userOption = GetUserOptions();

                if(userOption == 1)
                {
                    ListAllAccounts(); // No class specified because method is not static
                }
                else 
                {
                    FetchUserAccountName();
                }

                Console.WriteLine("\nWould you like to do something else today? y/n");
                var answer = Console.ReadLine();
                var yesAnswers = new List<string>() {"yes", "Yes", "y", "Y", "yeah", "Yeah", "YES"};
                if (!yesAnswers.Contains(answer))
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
            bank.RunSoftBank();
        }
    }
}