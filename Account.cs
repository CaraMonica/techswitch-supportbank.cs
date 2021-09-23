using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Account
    {
        private static int counter = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Balance { get; set; }

        public Account(string name)
        {
            Id = ++counter;
            Name = name;
            Balance = 0;
        }

        public override string ToString()
        {   
            string balanceString = Balance > 0 ? "is owed" : "owes";
            return $"{Name} {balanceString} {Balance}";
        }
    }
}