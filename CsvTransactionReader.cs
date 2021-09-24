using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SupportBank
{
    class CsvTransactionReader : TransactionsReader
    {
        public bool CanProcessFile(string fileName) => Path.GetExtension(fileName.ToLower()) == ".csv";

        public IEnumerable<Transaction> ProcessFile(string fileName) => File.ReadAllLines(fileName)
                                                                                    .Skip(1)
                                                                                    .Select(row => row.Split(","))
                                                                                    .Select((row, i) => Transaction.FromArray(row, i + 2))
                                                                                    .Where(t => t != null);
    }

}