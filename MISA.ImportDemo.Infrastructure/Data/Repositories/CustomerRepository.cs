using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Entities.Directory;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public Task<IEnumerable<Customer>> GetCustomerByFilter(object[] filter)
        {
            return Task.FromResult(GetEntities("Proc_GetCustomerByFilter", filter));
        }
    }
}
