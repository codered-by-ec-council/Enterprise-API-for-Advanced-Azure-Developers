using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.BusinessFacade
{
    public static class CrudResultInfo
    {
        //define crudresult return codes 
        public static class Codes
        {
            public static string CODE_LOCKSTAMP_MISMATCH = "LOCKSTAMP-MISMATCH";
            public static string CODE_ENTITY_NOTFOUND = "ENTITY_NOTFOUND";
            public static string CODE_MANDATORY_VALUES_MISSING = "MANDATORY-VALUES-MISSING";
            public static string CODE_SUCCESSFUL = "SUCCESSFUL";
            public static string CODE_INTERNAL_ERROR = "INTERNAL_ERROR";
        }

        public static class Messages
        {
            public static string LIT_LOCKSTAMP_MISMATCH = "Lockstamps do not match. Possible due to a previous update by someone else.";
            public static string LIT_ENTITY_NOTFOUND = "Entity was not found.";
            public static string LIT_MANDATORY_VALUES_MISSING = "Mandatory values must be supplied.";
            public static string LIT_SUCCESSFUL = "Successful";
        }
    }
}
