using ECC.Customer.DataAccessLayer;
using ECC.Customer.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.BusinessFacade
{
    public class CustomerBusinessFacade : ICustomerBusinessFacade
    {

        private readonly ILogger<CustomerBusinessFacade> _logger;

        //Customer repo
        IGenericRepository<PersonDto> _personRepo;

        //Add person repo using dependency injection
        public CustomerBusinessFacade(IGenericRepository<PersonDto> personRepo, ILogger<CustomerBusinessFacade> logger)
        {
            _personRepo = personRepo;
            _logger = logger;
        }

        public CrudResultDto Add(PersonDto person, string sessionId, string correlationId)
        {
            var result = new CrudResultDto();
            
            //validate entity
            if(string.IsNullOrEmpty(person.FirstName) || string.IsNullOrEmpty(person.LastName))
            {
                result.IsSuccessful = false;
                result.Message = string.Format("{0}:{1}", CrudResultInfo.Messages.LIT_MANDATORY_VALUES_MISSING, "LastName, FirstName");
                result.Code = CrudResultInfo.Codes.CODE_MANDATORY_VALUES_MISSING;

                return result;
            }

            PersonDto personResult;            
            try
            {
                personResult = _personRepo.Add(person);
                if (personResult.Id != 0)
                {
                    result.IsSuccessful = true;
                    result.EntityId = personResult.Id;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{message}\nSessionId:{sessionId}\nCorrelationId:{correlationId}", ex.Message, sessionId, correlationId);
                result.IsSuccessful = false;
                result.Code = CrudResultInfo.Codes.CODE_INTERNAL_ERROR;
            }

            return result;
        }

        public CrudResultDto Delete(int personId, long timeStamp, string sessionId, string correlationId)
        {
            var result = new CrudResultDto();
            int rc;

            try
            {
                rc = _personRepo.Delete(personId, timeStamp);
                if (rc == (int)DALReturnCodes.Successful)
                    result.IsSuccessful = true;


                if (rc == (int)DALReturnCodes.RecordNotFound)
                {
                    result.Code = CrudResultInfo.Codes.CODE_ENTITY_NOTFOUND;
                    result.Message = CrudResultInfo.Messages.LIT_ENTITY_NOTFOUND;
                }

                if (rc == (int)DALReturnCodes.LockstampMisMatch)
                {
                    result.Code = CrudResultInfo.Codes.CODE_LOCKSTAMP_MISMATCH;
                    result.Message = CrudResultInfo.Messages.LIT_LOCKSTAMP_MISMATCH;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{message}\nSessionId:{sessionId}\nCorrelationId:{correlationId}", ex.Message, sessionId, correlationId);
                result.Code = CrudResultInfo.Codes.CODE_INTERNAL_ERROR;

            }           
            return result;
        }

        public Tuple<CrudResultDto, PersonDto> Get(int personId, string sessionId, string correlationId)
        {
            Tuple<CrudResultDto, PersonDto> result;

            try
            {
                result = Tuple.Create(new CrudResultDto {IsSuccessful = true }, _personRepo.GetById(personId));                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{message}\nSessionId:{sessionId}\nCorrelationId:{correlationId}", ex.Message, sessionId, correlationId);
                result = Tuple.Create(
                    new CrudResultDto { IsSuccessful = false, Code = CrudResultInfo.Codes.CODE_INTERNAL_ERROR }, 
                    new PersonDto());               
            }

            return result;
        }

        public Tuple<CrudResultDto, List<PersonDto>> Search(string firstName, string lastName, string sessionId, string correlationId)
        {
            var result = Tuple.Create(new CrudResultDto(), new List<PersonDto>());

            //if no filters supplied get all    
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                try
                {
                    var persons = _personRepo.All();
                    result.Item1.Code = CrudResultInfo.Codes.CODE_SUCCESSFUL;
                    result.Item1.IsSuccessful = true;

                    result.Item2.AddRange(_personRepo.All());
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "{message}\nSessionId:{sessionId}\nCorrelationId:{correlationId}", ex.Message, sessionId, correlationId);
                    result.Item1.Code = CrudResultInfo.Codes.CODE_INTERNAL_ERROR;
                }
            }
            else
            {
                //apply filter

            }

            return result;
        }

        public CrudResultDto Update(PersonDto person, long timeStamp, string sessionId, string correlationId)
        {
            var result = new CrudResultDto();

            //validate entity
            if (string.IsNullOrEmpty(person.FirstName) || string.IsNullOrEmpty(person.LastName))
            {
                result.IsSuccessful = false;
                result.Message = string.Format("{0}:{1}",CrudResultInfo.Messages.LIT_MANDATORY_VALUES_MISSING, "LastName, FirstName"); 
                result.Code = CrudResultInfo.Codes.CODE_MANDATORY_VALUES_MISSING;

                return result;
            }

            try
            {
                var rc = _personRepo.Update(person, timeStamp);
                if (rc == (int)DALReturnCodes.Successful)
                    result.IsSuccessful = true;

                if (rc == (int)DALReturnCodes.RecordNotFound)
                {
                    result.Code = CrudResultInfo.Codes.CODE_ENTITY_NOTFOUND;
                    result.Message = CrudResultInfo.Messages.LIT_ENTITY_NOTFOUND;
                }
                if (rc == (int)DALReturnCodes.LockstampMisMatch)
                {
                    result.Code = CrudResultInfo.Codes.CODE_LOCKSTAMP_MISMATCH;
                    result.Message = CrudResultInfo.Messages.LIT_LOCKSTAMP_MISMATCH;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{message}\nSessionId:{sessionId}\nCorrelationId:{correlationId}", ex.Message, sessionId, correlationId);
                result.Code = CrudResultInfo.Codes.CODE_INTERNAL_ERROR;
            }

            return result;
        }
    }
}
