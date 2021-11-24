using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECC.Customer.WebApi.Controllers
{
    internal class CustomerLogEvents
    {        
        public const int ListItems = 10001;
        public const int GetItem = 10002;
        public const int InsertItem = 10003;
        public const int UpdateItem = 10004;
        public const int DeleteItem = 10005;

        public const int TestItem = 10901;

        public const int GetItemNotFound = 10401;
        public const int UpdateItemNotFound = 10402;

    }
}
