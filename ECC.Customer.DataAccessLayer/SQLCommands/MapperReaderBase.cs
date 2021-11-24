using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.Customer.DataAccessLayer.SQLCommands
{
    public abstract class MapperReaderBase<T>
    {
        protected abstract T Map(IDataRecord row);

        internal Collection<T> MapAll(IDataReader row)
        {
            Collection<T> collection = new();
            while (row.Read())
            {
                try
                {
                    collection.Add(Map(row));
                }
                catch
                {
                    throw;                  
                }
            }
            return collection;
        }
    }
}
