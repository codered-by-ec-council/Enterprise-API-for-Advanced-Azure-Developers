using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.Dto
{
    public class CrudResultDto
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public int EntityId { get; set; }
    }
}
