
using System;

namespace Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Info { get; set; }
        public int CategoryId { get; set; }
        public int Id { get; set; }
    }
}
