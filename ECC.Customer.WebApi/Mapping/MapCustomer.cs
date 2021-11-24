using ECC.Customer.Dto;
using ECC.Customer.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECC.Customer.WebApi.Mapping
{
    internal static class MapCustomer
    {
        internal static CustomerResponse DtoToCustomerResponse(PersonDto dto)
        {
            return new CustomerResponse
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                Id = dto.Id,
                LastName = dto.LastName,
                Lockstamp = dto.TimeStamp,
                PostalAddress = new Address
                {
                    Address1 = dto.HomeAddress?.Address1,
                    Address2 = dto.HomeAddress?.Address2,
                    City = dto.HomeAddress?.City
                }
            };
        }

        internal static CustomersResponse DtoToCustomersResponse(List<PersonDto> dtoList)
        {
            var resp = new CustomersResponse();
            resp.Customers = new();
            foreach( PersonDto dto in dtoList )
            {
                resp.Customers.Add(DtoToCustomerResponse(dto));
            }

            return resp;
        }

        internal static PersonDto CustomerToDto(Models.Customer cust)
        {
            return new PersonDto
            {
                Email = cust.Email,
                FirstName = cust.FirstName,
                LastName = cust.LastName,
                HomeAddress = new AddressDto
                {
                    Address1 = cust.PostalAddress?.Address1,
                    Address2 = cust.PostalAddress?.Address2,
                    City = cust.PostalAddress?.City
                }
            };
        }
    }
}
