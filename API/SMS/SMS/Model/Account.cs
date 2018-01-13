using System;
using System.Collections.Generic;

namespace SMS.Model
{
    public partial class Account
    {
        public Account()
        {
            PhoneNumber = new HashSet<PhoneNumber>();
        }

        public long Id { get; set; }
        public string AuthId { get; set; }
        public string Username { get; set; }

        public virtual ICollection<PhoneNumber> PhoneNumber { get; set; }
    }
}
