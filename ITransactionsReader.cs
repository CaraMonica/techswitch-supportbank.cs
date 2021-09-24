using System.Collections.Generic;

namespace SupportBank
{
    interface ITransactionsReader
    {
        bool CanProcessFile(string fileName);
        IEnumerable<Transaction> ProcessFile(string fileName);
    }
}