using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.Dto
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string MiddleName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public long TimeStamp { get; set; }

        public AddressDto HomeAddress { get; set; }

    }
}
