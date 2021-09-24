using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SupportBank
{
    class JsonTransactionReader : TransactionsReader
    {
        public bool CanProcessFile(string fileName) => Path.GetExtension(fileName.ToLower()) == ".json";

        public IEnumerable<Transaction> ProcessFile(string fileName) => JsonConvert.DeserializeObject<IEnumerable<Transaction>>(File.ReadAllText(fileName));
    }

}