using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SupportBank
{
    class XmlTransactionsReader : ITransactionsReader
    {
        public bool CanProcessFile(string fileName) => Path.GetExtension(fileName.ToLower()) == ".xml";

        public IEnumerable<Transaction> ProcessFile(string fileName)
        {
            XDocument XDoc = XDocument.Load(fileName);
            return XDoc.Descendants("SupportTransaction").Select((e, i) => Transaction.FromArray(CreateStringArrayFromElement(e), i, isDateTimeStamp: true))
                                                        .Where(t => t != null);
        }

        private string[] CreateStringArrayFromElement(XElement transactionElement) => new string[]{transactionElement.Attribute("Date").Value,
                                                                                                    transactionElement.Element("Parties").Element("From").Value,
                                                                                                    transactionElement.Element("Parties").Element("To").Value,
                                                                                                    transactionElement.Element("Description").Value,
                                                                                                    transactionElement.Element("Value").Value,
                                                                                                    };
    }
}

