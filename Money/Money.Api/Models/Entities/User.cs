using System.Collections.Generic;

namespace Money.Api.Models.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public List<Transaction> Transactions { get; } = new List<Transaction>();
    }
}
