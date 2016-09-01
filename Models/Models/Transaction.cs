
using System;

namespace Models
{
    public class Transaction
    {

        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Info { get; set; }
        public int CategoryId { get; set; }
        internal int Id { get; set; }
        internal Account Account { get; set; }
    }
}
