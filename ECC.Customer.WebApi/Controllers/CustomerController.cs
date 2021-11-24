using ECC.Customer.BusinessFacade;
using ECC.Customer.WebApi.Mapping;
using ECC.Customer.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECC.Customer.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ILogger<CustomerController> _logger;

        private const string LIT_HEADER_ETAG = "ETag";
        private const string LIT_HEADER_LOCATION = "Location";

        private ICustomerBusinessFacade _customerBusinessFacade;


        /// <summary>
        /// Customer Business Facade Dependency injection 
        /// </summary>
        /// <param name="customerBusinessFacade"></param>
        public CustomerController(ICustomerBusinessFacade customerBusinessFacade, ILogger<CustomerController> logger)
        {
            _customerBusinessFacade = customerBusinessFacade;
            _logger = logger;
        }


        /// <summary>
        /// Delete customer by Id
        /// </summary>
        /// <remarks>Delete the customer by Id and if the etag matches the record on the server.</remarks>
        /// <param name="ifMatch">Set this to the etag value received from the initial GET request.</param>
        /// <param name="xSessionId"></param>
        /// <param name="xCorrelationId"></param>
        /// <param name="customerId"></param>
        /// <response code="204">Succeeded and no cotent to return in the response</response>
        /// <response code="400">Invalid requst was sent by the client.</response>
        /// <response code="401">Unauthorised access</response>
        /// <response code="404">Customer not found</response>
        /// <response code="412">Precondition failed</response>
        [Authorize(Policy = "CrmAccess", Roles = "crm.delete")]
        [HttpDelete]
        [Route("/{customerId}")]
        [SwaggerOperation("DeleteCustomerById")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Invalid requst was sent by the client.")]
        [SwaggerResponse(statusCode: 412, type: typeof(InlineResponse412), description: "Precondition failed")]
        public virtual IActionResult DeleteCustomerById([FromHeader(Name = "If-Match")][Required()] long ifMatch, [FromHeader(Name = "X-Session-Id")][Required()] string xSessionId, [FromHeader(Name = "X-Correlation-Id")][Required()] string xCorrelationId, [FromRoute][Required] int? customerId)
        {
            _logger.LogInformation(CustomerLogEvents.DeleteItem, "Session:{0} CorrelationId:{1}", xSessionId, xCorrelationId);

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);


            var result = _customerBusinessFacade.Delete((int)customerId, ifMatch, xSessionId, xCorrelationId);
            
            if(result.IsSuccessful)
                return StatusCode(StatusCodes.Status204NoContent);

            if(result.Code == CrudResultInfo.Codes.CODE_LOCKSTAMP_MISMATCH)
                return StatusCode(StatusCodes.Status412PreconditionFailed, new InlineResponse412 { ErrorCode = result.Code, Message=result.Message});
                            

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Find a customer by Id
        /// </summary>
        /// <remarks>Return the customer using the primary key.</remarks>
        /// <param name="customerId"></param>
        /// <param name="xSessionId"></param>
        /// <param name="xCorrelationId"></param>
        /// <response code="200">Get customer and provide etag</response>
        /// <response code="400">Invalid requst was sent by the client.</response>
        /// <response code="401">Unauthorised access</response>
        /// <response code="404">Customer not found</response>
        [Authorize(Policy = "CrmAccess", Roles = "crm.readone")]
        [HttpGet]
        [Route("/{customerId}")]
        [SwaggerOperation("GetCustomerById")]
        [SwaggerResponse(statusCode: 200, type: typeof(CustomerResponse), description: "Get customer and provide etag")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Invalid requst was sent by the client.")]
        public virtual IActionResult GetCustomerById([FromRoute][Required] int? customerId, [FromHeader(Name = "X-Session-Id")][Required()] string xSessionId, [FromHeader(Name = "X-Correlation-Id")][Required()] string xCorrelationId)
        {
            _logger.LogInformation(CustomerLogEvents.GetItem, "Session:{0} CorrelationId:{1}", xSessionId, xCorrelationId);

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            var result = _customerBusinessFacade.Get((int)customerId, xSessionId, xCorrelationId);
            
            if(result.Item1.IsSuccessful)
            {
                if(result.Item2 == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    Response.Headers.Add(LIT_HEADER_ETAG, result.Item2.TimeStamp.ToString());
                    return StatusCode(StatusCodes.Status200OK, MapCustomer.DtoToCustomerResponse(result.Item2));
                }
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <remarks>Return all retail customers. There is no filtering provided.</remarks>
        /// <param name="xSessionId"></param>
        /// <param name="xCorrelationId"></param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorised access</response>
        [Authorize(Policy = "CrmAccess", Roles ="crm.readlist,crm.readone")]
        [HttpGet]
        [Route("/")]
        [SwaggerOperation("GetCustomers")]
        [SwaggerResponse(statusCode: 200, type: typeof(CustomersResponse), description: "")]
        public virtual IActionResult GetCustomers([FromHeader(Name="X-Session-Id")][Required()] string xSessionId, [FromHeader(Name = "X-Correlation-Id")][Required()] string xCorrelationId)
        {
            _logger.LogInformation(CustomerLogEvents.ListItems, "Session:{0} CorrelationId:{1}", xSessionId, xCorrelationId);

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            var result = _customerBusinessFacade.Search(null,null, xSessionId, xCorrelationId);

            if(result.Item1.IsSuccessful)
            {                
                return StatusCode(StatusCodes.Status200OK, MapCustomer.DtoToCustomersResponse(result.Item2));
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Save retail customer
        /// </summary>
        /// <remarks>Saves a retail customer and returns the new Id in the response header</remarks>
        /// <param name="body"></param>
        /// <response code="201">The customer was created successfully and returns the uri of the new asset.</response>
        /// <response code="400">Invalid requst was sent by the client.</response>
        /// <response code="401">Unauthorised access</response>
        [Authorize(Policy = "CrmAccess", Roles = "crm.create")]
        [HttpPost]
        [Route("/")]
        [SwaggerOperation("PostCustomer")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Invalid requst was sent by the client.")]
        public virtual IActionResult PostCustomer([FromHeader(Name = "X-Session-Id")][Required()] string xSessionId, [FromHeader(Name = "X-Correlation-Id")][Required()] string xCorrelationId, [FromBody] Models.Customer body)
        {
            _logger.LogInformation(CustomerLogEvents.InsertItem, "Session:{0} CorrelationId:{1}", xSessionId, xCorrelationId);

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            var result = _customerBusinessFacade.Add(MapCustomer.CustomerToDto(body), xSessionId, xCorrelationId);
            if(result.IsSuccessful)
            {
                Response.Headers.Add(LIT_HEADER_LOCATION, string.Format("{0}/{1}", Request.PathBase.Value, result.EntityId));
                return StatusCode(StatusCodes.Status201Created);
            }

            if (result.Code == CrudResultInfo.Codes.CODE_MANDATORY_VALUES_MISSING)
                return StatusCode(StatusCodes.Status400BadRequest, new InlineResponse400 { ErrorCode = result.Code, Message = result.Message });
            
            return StatusCode(StatusCodes.Status500InternalServerError);            
        }

        /// <summary>
        /// Update customer by Id
        /// </summary>
        /// <remarks>Updates the customer</remarks>
        /// <param name="ifMatch">Set this to the etag value received from the initial GET request.</param>
        /// <param name="xSessionId"></param>
        /// <param name="xCorrelationId"></param>
        /// <param name="customerId"></param>
        /// <param name="body"></param>
        /// <response code="200">Customer saved successfully</response>
        /// <response code="400">Invalid requst was sent by the client.</response>
        /// <response code="401">Unauthorised access</response>
        /// <response code="404">Customer not found</response>
        /// <response code="412">Precondition failed</response>
        [Authorize(Policy = "CrmAccess", Roles = "crm.update")]
        [HttpPut]
        [Route("/{customerId}")]
        [SwaggerOperation("PutCustomerById")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Invalid requst was sent by the client.")]
        [SwaggerResponse(statusCode: 412, type: typeof(InlineResponse412), description: "Precondition failed")]
        public virtual IActionResult PutCustomerById([FromHeader(Name = "If-Match")][Required()] long ifMatch, [FromHeader(Name = "X-Session-Id")][Required()] string xSessionId, [FromHeader(Name = "X-Correlation-Id")][Required()] string xCorrelationId, [FromRoute][Required] int? customerId, [FromBody] Models.Customer body)
        {
            _logger.LogInformation(CustomerLogEvents.UpdateItem, "Session:{0} CorrelationId:{1}", xSessionId, xCorrelationId);

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            var dtoCust = MapCustomer.CustomerToDto(body);
            dtoCust.Id = (int)customerId;
            var result = _customerBusinessFacade.Update(dtoCust, ifMatch, xSessionId, xCorrelationId);
            if (result.IsSuccessful)                         
                return StatusCode(StatusCodes.Status200OK);
            
            if(result.Code == CrudResultInfo.Codes.CODE_ENTITY_NOTFOUND)
                return StatusCode(StatusCodes.Status404NotFound);

            if(result.Code == CrudResultInfo.Codes.CODE_MANDATORY_VALUES_MISSING)
                return StatusCode(StatusCodes.Status400BadRequest, new InlineResponse400 { ErrorCode = result.Code, Message =  result.Message });
            
            if (result.Code == CrudResultInfo.Codes.CODE_LOCKSTAMP_MISMATCH)
                return StatusCode(StatusCodes.Status412PreconditionFailed, new InlineResponse400 { ErrorCode = result.Code, Message = result.Message });


            return StatusCode(StatusCodes.Status500InternalServerError);

        }

    }
}
