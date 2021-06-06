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
    /// <summary>
    /// Sử lý nghiệp vụ của khách hàng
    /// </summary>
    /// CreatedBy: NGDUONG (05/06/2021)
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        #region Method
        public Task<IEnumerable<Customer>> GetCustomerByFilter(object[] filter)
        {
            return Task.FromResult(GetEntities("Proc_GetCustomerByFilter", filter));
        }
        #endregion
    }
}
