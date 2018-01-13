using System;
using System.Collections.Generic;

namespace SMS.Model
{
    public partial class PhoneNumber
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long? AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
