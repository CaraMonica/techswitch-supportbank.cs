using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SupportBank
{
    class CsvTransactionsWriter : ITransactionsWriter
    {
        public bool CanProcessFile(string fileName) => Path.GetExtension(fileName.ToLower()) == ".csv";

        public void WriteFile(string fileName, IEnumerable<Transaction> transactions)
        {
            var headers = "Date,From,To,Narrative,Amount";
            var lines = transactions.Select(t => $"{t.Date},{t.FromAccount},{t.ToAccount},{t.Narrative},{t.Amount}");
            File.WriteAllText(fileName, headers + "\n" + string.Join("\n", lines));
        }
    }

}