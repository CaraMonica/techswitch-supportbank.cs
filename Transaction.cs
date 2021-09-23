using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

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
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }

        public Transaction(DateTime date, string fromUser, string toUser, string narrative, decimal amount)
        => (Id, Date, FromUser, ToUser, Narrative, Amount) = (++counter, date, fromUser, toUser, narrative, amount);

        public override string ToString()
        {
            string dateString = Date.ToString("dd MMMM, yyyy");
            return $"On the {dateString} {FromUser} lent {ToUser} £{Amount} for {Narrative}.";
        }

        public void Deconstruct(out string fromUser, out string toUser, out decimal amount)
        {
            fromUser = FromUser;
            toUser = ToUser;
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
    }
}