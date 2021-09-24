using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, Account> NameToAccount { get; set; }
        private IEnumerable<TransactionsReader> transactionsReaders = new List<TransactionsReader>() { new CsvTransactionReader(), new JsonTransactionReader(), new XmlTransactionReader() };
        private SupportBank(string transactionFile = "./data/Transactions2014.csv")
        {
            Transactions = GetTransactionsFromFile(transactionFile);
            NameToAccount = GetAccountsDictionary();
        }
        public static SupportBank FromFile()
        {
            SupportBank bank;
            while (true)
            {
                Console.WriteLine("\nTo load your own file type the location below. To use the default press enter.");
                string fileName = Console.ReadLine();
                if (String.IsNullOrEmpty(fileName))
                {
                    bank = new SupportBank();
                    break;
                }

                if (File.Exists(fileName))
                {
                    try
                    {
                        bank = new SupportBank(fileName);
                        break;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Error($" File type not supported, {e}");
                    }
                }
                Logger.Info($"User provided missing or incorrect file name {fileName}");
                Console.WriteLine($"Could not find file {fileName}.");
            }
            return bank;
        }

        private IEnumerable<Transaction> GetTransactionsFromFile(string fileName)
        {
            var reader = transactionsReaders.First(r => r.CanProcessFile(fileName));
            if (reader is null)
            {
                throw new ArgumentOutOfRangeException($"File {fileName} not supported.");
            }

            return reader.ProcessFile(fileName);
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