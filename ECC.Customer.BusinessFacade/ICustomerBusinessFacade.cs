using ECC.Customer.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.BusinessFacade
{
    public interface ICustomerBusinessFacade
    {
        CrudResultDto Add(PersonDto person, string sessionId, string correlationId);

        CrudResultDto Delete(int personId, long timeStamp, string sessionId, string correlationId);

        CrudResultDto Update(PersonDto person, long timeStamp, string sessionId, string correlationId);

        Tuple<CrudResultDto, PersonDto> Get(int personId, string sessionId, string correlationId);

        Tuple<CrudResultDto, List<PersonDto>> Search(string firstName, string lastName, string sessionId, string correlationId);

    }
}
