using MISA.ImportDemo.Core.Entities.Directory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface khai báo các hàm cung cấp cho việc nhập khẩu Khách hàng
    /// </summary>
    /// CreatedBy: NGDUONG (06/06/2021)
    public interface IImportCustomerRepository : IBaseImportRepository
    {
        /// <summary>
        /// Lấy toàn bộ danh sách Khách hàng có trong Db
        /// </summary>
        /// <returns>List Khach hang</returns>
        /// CreatedBy: NGDUONG (06/06/2021)
        Task<List<Customer>> GetCustomers();
    }
}
