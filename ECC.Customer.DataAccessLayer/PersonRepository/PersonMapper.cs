using ECC.Customer.DataAccessLayer.SQLCommands;
using ECC.Customer.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.DataAccessLayer.PersonRepository
{
    class PersonMapper : MapperReaderBase<PersonDto>
    {
      
        protected override PersonDto Map(IDataRecord row)
        {
            try
            {
                PersonDto p = new();
                p.Id = (DBNull.Value == row["CustomerID"]) ? 0 : (int)row["CustomerID"];
                p.FirstName = (DBNull.Value == row["FirstName"]) ? string.Empty : (string)row["FirstName"];
                p.LastName = (DBNull.Value == row["LastName"]) ? string.Empty : (string)row["LastName"];
                p.Email = (DBNull.Value == row["Email"]) ? string.Empty : (string)row["Email"];
                p.TimeStamp = (long)row["Lockstamp"];
                p.HomeAddress = new();
                p.HomeAddress.AddressId = (DBNull.Value == row["AddressId"]) ? 0 : (int)row["AddressId"];
                p.HomeAddress.Address1 = (DBNull.Value == row["Address1"]) ? string.Empty : (string)row["Address1"];
                p.HomeAddress.Address2 = (DBNull.Value == row["Address2"]) ? string.Empty : (string)row["Address2"];
                p.HomeAddress.Address3 = (DBNull.Value == row["Address3"]) ? string.Empty : (string)row["Address3"];
                p.HomeAddress.City = (DBNull.Value == row["Town"]) ? string.Empty : (string)row["Town"];
                return p;
            }
            catch
            {               
                throw;              
            }
        }
    }
}
