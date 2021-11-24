using ECC.Customer.DataAccessLayer.SQLCommands;
using ECC.Customer.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.DataAccessLayer.PersonRepository
{
    public class PersonRepo : SQLExecuterBase<PersonDto>, IGenericRepository<PersonDto>
    {
        private readonly ILogger<PersonRepo> _logger;

        private readonly string _connectionString;

        public PersonRepo(IConfiguration config, ILogger<PersonRepo> logger)
        {
            CommandType = CommandType.StoredProcedure;
            _connectionString = config.GetConnectionString("DefaultConnection");

            _logger = logger;
        }

        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PersonDto Add(PersonDto entity)
        {
            var spParams = new Collection<IDataParameter>();
            spParams.Add(new SqlParameter("@FirstName", entity.FirstName != null ? entity.FirstName: DBNull.Value));
            spParams.Add(new SqlParameter("@LastName", entity.LastName != null ? entity.LastName : DBNull.Value));
            spParams.Add(new SqlParameter("@Age", entity.Age));
            spParams.Add(new SqlParameter("@Email", entity.Email != null ? entity.Email : DBNull.Value ));
            spParams.Add(new SqlParameter("@Address1", entity.HomeAddress.Address1 != null ? entity.HomeAddress.Address1 : DBNull.Value));
            spParams.Add(new SqlParameter("@Address2", entity.HomeAddress.Address2 != null ? entity.HomeAddress.Address2 : DBNull.Value));
            spParams.Add(new SqlParameter("@Address3", entity.HomeAddress.Address3 != null ? entity.HomeAddress.Address3: DBNull.Value));
            spParams.Add(new SqlParameter("@Town", entity.HomeAddress.City != null ? entity.HomeAddress.City : DBNull.Value));
            spParams.Add(new SqlParameter
            {
                ParameterName = "@CustomerId_out",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            });
            CommandText = "dbo.AddCustomer";
            Parameters = spParams;

            var outParams = base.ExecuteNonQueryWithOutputParams();
            if (outParams.ContainsKey("@CustomerId_out"))
                entity.Id = Convert.ToInt32(outParams["@CustomerId_out"]);

            return entity;
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PersonDto> All()
        {
            CommandText = "dbo.GetCustomers";
            Mapper = new PersonMapper();

            return base.ExecuteReader();
        }

        /// <summary>
        /// Delete a customer by Id and if lockstamp matches
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lockStamp"></param>
        /// <returns></returns>
        public int Delete(int id, long lockStamp)
        {
            var spParams = new Collection<IDataParameter>();
            spParams.Add(new SqlParameter("@CustomerId", id));
            spParams.Add(new SqlParameter("@LockStamp", lockStamp));
            spParams.Add(new SqlParameter { ParameterName = "@ReturnValue", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue });
            CommandText = "dbo.DelCustomerById";
            Parameters = spParams;

            var outParams = base.ExecuteNonQueryWithOutputParams();
            if (outParams.ContainsKey("@ReturnValue"))
                return( Convert.ToInt32(outParams["@ReturnValue"]));

            return (int)DALReturnCodes.Undefined;
        }

        public IEnumerable<PersonDto> Find(Expression<Func<PersonDto, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PersonDto GetById(int id)
        {
            var spParams = new Collection<IDataParameter>();
            spParams.Add(new SqlParameter("@CustomerId", id));
            CommandText = "dbo.GetCustomerById";
            Parameters = spParams;
            Mapper = new PersonMapper();

            var resultSet = base.ExecuteReader();
            return resultSet.FirstOrDefault();

        }

        /// <summary>
        /// Update customer and address by customerId and if lockstamp matches
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lockStamp"></param>
        /// <returns></returns>
        public int Update(PersonDto entity, long lockStamp)
        {
            var spParams = new Collection<IDataParameter>();
            spParams.Add(new SqlParameter("@CustomerId", entity.Id));
            spParams.Add(new SqlParameter("@FirstName", entity.FirstName != null ? entity.FirstName : DBNull.Value));
            spParams.Add(new SqlParameter("@LastName", entity.LastName != null ? entity.LastName : DBNull.Value));
            spParams.Add(new SqlParameter("@Age", entity.Age));
            spParams.Add(new SqlParameter("@Email", entity.Email != null ? entity.Email : DBNull.Value));
            spParams.Add(new SqlParameter("@Address1", entity.HomeAddress.Address1 != null ? entity.HomeAddress.Address1 : DBNull.Value));
            spParams.Add(new SqlParameter("@Address2", entity.HomeAddress.Address2 != null ? entity.HomeAddress.Address2 : DBNull.Value));
            spParams.Add(new SqlParameter("@Address3", entity.HomeAddress.Address3 != null ? entity.HomeAddress.Address3 : DBNull.Value));
            spParams.Add(new SqlParameter("@Town", entity.HomeAddress.City != null ? entity.HomeAddress.City : DBNull.Value));
            spParams.Add(new SqlParameter("@LockStamp", lockStamp));
            spParams.Add(new SqlParameter { ParameterName = "@ReturnValue", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue});
            CommandText = "dbo.UpdCustomer";
            Parameters = spParams;

            var outParams = base.ExecuteNonQueryWithOutputParams();
            if (outParams.ContainsKey("@ReturnValue"))
                return (Convert.ToInt32(outParams["@ReturnValue"]));

            return (int)DALReturnCodes.Undefined;

        }

        protected override IDbConnection GetConnection()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            return connection;
        }


        /// <summary>
        /// Create a retry policy to access the database
        /// </summary>
        /// <returns></returns>
        protected override Policy GetPolicy()
        {
            return Policy.Handle<Exception>().WaitAndRetry(
                 retryCount: 3, // Retry 3 times
                 // Exponential backoff based on an initial 200 ms delay.
                 sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)), 
                 onRetry: (exception, attempt) =>
                 {
                     // Capture some information for logging/telemetry.
                     Debug.WriteLine($"ExecuteReaderWithRetryAsync: Retry {attempt} due to {exception.Message}.");

                     _logger.LogWarning($"ExecuteReaderWithRetryAsync: Retry {attempt} due to {exception.Message}.");
                 });
        }
    }
}
