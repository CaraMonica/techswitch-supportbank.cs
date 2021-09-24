using System.Collections.Generic;

namespace SupportBank
{
    interface ITransactionsWriter
    {
        bool CanProcessFile(string fileName);
        void WriteFile(string fileName, IEnumerable<Transaction> transactions);
    }
}