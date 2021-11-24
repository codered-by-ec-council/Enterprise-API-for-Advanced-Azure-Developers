using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.DataAccessLayer
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> All();
        T GetById(int id);
        T Add(T entity);
        int Delete(int id, long lockStamp);
        int Update(T entity, long lockStamp);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }

    public enum DALReturnCodes{ Undefined = -1, Successful= 0, RecordNotFound = 1, LockstampMisMatch = 2 }

}
