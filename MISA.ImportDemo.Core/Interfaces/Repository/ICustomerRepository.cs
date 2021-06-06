using MISA.ImportDemo.Core.Entities.Directory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    /// <summary>
    /// Quản lý nghiệp vụ phần đơn vị
    /// </summary>
    /// CreatedBy: NGDUONG (06/06/2021)
    public interface ICustomerRepository
    {
        /// <summary>
        /// Hàm lấy danh sách Customer
        /// </summary>
        /// <param name="filter">Điều kiện</param>
        /// <returns>Danh sách khách hàng</returns>
        /// CreatedBy: NGDUONG (06/06/2021)
        Task<IEnumerable<Customer>> GetCustomerByFilter(object[] filter);
    }
}
