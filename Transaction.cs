using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
#nullable enable

namespace SupportBank
{
    class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        // Counter to create individual ids for each transaction.
        private static int counter = 0;

        // Features
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }

        public Transaction(DateTime date, string fromAccount, string toAccount, string narrative, decimal amount)
        => (Id, Date, FromAccount, ToAccount, Narrative, Amount) = (++counter, date, fromAccount, toAccount, narrative, amount);

        public override string ToString()
        {
            string dateString = Date.ToString("dd MMMM, yyyy");
            return $"On the {dateString} {FromAccount} lent {ToAccount} £{Amount} for {Narrative}.";
        }

        public void Deconstruct(out string fromAccount, out string toAccount, out decimal amount)
        {
            fromAccount = FromAccount;
            toAccount = ToAccount;
            amount = Amount;
        }

        public static Transaction? FromArray(string[] transaction)
        {
            DateTime date;
            if (!DateTime.TryParse(transaction[0], out date))
            {
                Logger.Warn($"Could not parse transaction. {transaction[0]} is an invalid date.");
                return null;
            }

            decimal amount;
            if (!decimal.TryParse(transaction[4], out amount))
            {
                Logger.Warn($"Could not parse transaction. {transaction[4]} is an invalid decimal amount.");
                return null;
            }

            return new Transaction(date, transaction[1], transaction[2], transaction[3], amount);
        }

        public static Transaction? FromJSON(string[] transaction)
        {
            DateTime date;
            if (!DateTime.TryParse(transaction[0], out date))
            {
                Logger.Warn($"Could not parse transaction. {transaction[0]} is an invalid date.");
                return null;
            }

            decimal amount;
            if (!decimal.TryParse(transaction[4], out amount))
            {
                Logger.Warn($"Could not parse transaction. {transaction[4]} is an invalid decimal amount.");
                return null;
            }

            return new Transaction(date, transaction[1], transaction[2], transaction[3], amount);
        }
    }
}