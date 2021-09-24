using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SupportBank
{
    class JsonTransactionsWriter : ITransactionsWriter
    {
        public bool CanProcessFile(string fileName) => Path.GetExtension(fileName.ToLower()) == ".json";

        public void WriteFile(string fileName, IEnumerable<Transaction> transactions) {
            string json = JsonSerializer.Serialize(transactions);
            File.WriteAllText(fileName, json);
        }
    }

}