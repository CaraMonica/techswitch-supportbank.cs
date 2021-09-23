using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;

namespace SupportBank
{
    class SupportBank
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public IEnumerable<Transaction> Transactions { get; set; }
        public Dictionary<string, Account> NameToAccount { get; set; }
        private SupportBank()
        {
            Transactions = SupportBank.GetTransactionsFromFile("./data/Transactions2014.csv");
            NameToAccount = GetAccountsDictionary();
        }
        private SupportBank(string transactionFile)
        {
            Transactions = SupportBank.GetTransactionsFromFile(transactionFile);
            NameToAccount = GetAccountsDictionary();
        }
        public static SupportBank FromFile()
        {
            while (true)
            {
                Console.WriteLine("\nTo load your own file type the location below. To use the default press enter.");
                string fileName = Console.ReadLine();
                if (String.IsNullOrEmpty(fileName))
                {
                    return new SupportBank();
                }
                if (File.Exists(fileName) && (fileName.EndsWith(".csv") || fileName.EndsWith(".json")))
                {
                    return new SupportBank(fileName);
                }
                Console.WriteLine($"Could not find file {fileName}");
            }
        }

        private static IEnumerable<Transaction> GetTransactionsFromFile(string fileName)
        {
            if (fileName.EndsWith("csv"))
            {
                return GetTransactionsFromCSV(fileName);
            }
            else if (fileName.EndsWith("json"))
            {
                return GetTransactionsFromJSON(fileName);
            }

            throw new ArgumentOutOfRangeException("File name is not of the correct type.", fileName);
        }

        private static IEnumerable<Transaction> GetTransactionsFromJSON(string fileName) =>
        JsonConvert.DeserializeObject<IEnumerable<Transaction>>(File.ReadAllText(fileName));

        private static IEnumerable<Transaction> GetTransactionsFromCSV(string fileName) => File.ReadAllLines(fileName)
                                                                                    .Skip(1)
                                                                                    .Select(row => row.Split(","))
                                                                                    .Select(row => Transaction.FromArray(row))
                                                                                    .Where(t => t != null);

        private void AddTransactions(string fileName)
        {
            Transactions = Transactions.Concat(GetTransactionsFromCSV(fileName));
        }
        private Dictionary<string, Account> GetAccountsDictionary()
        {
            Dictionary<string, Account> nameToAccount = new Dictionary<string, Account>();

            foreach ((string fromAccount, string toAccount, decimal amount) in Transactions)
            {
                if (!nameToAccount.ContainsKey(fromAccount))
                {
                    nameToAccount.Add(fromAccount, new Account(fromAccount));
                }
                if (!nameToAccount.ContainsKey(toAccount))
                {
                    nameToAccount.Add(toAccount, new Account(toAccount));
                }
                nameToAccount[fromAccount].Balance = nameToAccount[fromAccount].Balance + amount;
                nameToAccount[toAccount].Balance = nameToAccount[toAccount].Balance - amount;
            }
            return nameToAccount;
        }

        private void WriteEnumerableToConsole<T>(IEnumerable<T> enumerable)
        {
            foreach (var element in enumerable)
            {
                Console.WriteLine(element);
            }
        }

        private void ListAllAccounts() => WriteEnumerableToConsole(NameToAccount.Values);

        private void ListAccountTransactions(string name) => WriteEnumerableToConsole(Transactions.Where(transaction => transaction.FromAccount.Equals(name) || transaction.ToAccount.Equals(name)));

        private string GetValidAccountName()
        {
            Console.WriteLine("\nPlease enter the account holder's name: ");
            string name = Console.ReadLine();
            while (!NameToAccount.ContainsKey(name))
            {
                Console.WriteLine("Invalid name. Please enter a valid name.");
                name = Console.ReadLine();
            }

            return name;
        }

        private int GetUserOptions()
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

        private bool HasUserSaidYes()
        {
            var answer = Console.ReadLine();
            var yesAnswers = new List<string>() { "yes", "Yes", "y", "Y", "yeah", "Yeah", "YES" };
            return yesAnswers.Contains(answer);
        }

        public void RunSupportBank()
        {
            Console.WriteLine("Welcome to Support Bank.");
            while (true)
            {
                if (GetUserOptions() == 1)
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

            SupportBank bank = SupportBank.FromFile();
            bank.RunSupportBank();
        }
    }
}