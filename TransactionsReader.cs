using System.Collections.Generic;

namespace SupportBank
{
    interface TransactionsReader
    {
        bool CanProcessFile(string fileName);
        IEnumerable<Transaction> ProcessFile(string fileName);
    }
}