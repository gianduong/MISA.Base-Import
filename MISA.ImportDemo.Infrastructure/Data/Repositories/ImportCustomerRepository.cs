using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Entities.Directory;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Properties;
using MISA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository phục vụ nhập khẩu Khách hàng
    /// </summary>
    /// CreatedBy: NGDUONG (06/06/2021)
    public class ImportCustomerRepository : BaseImportRepository, IImportCustomerRepository
    {
        #region CONSTRUCTOR
        public ImportCustomerRepository(IEntityRepository entityRepository, IMemoryCache importMemoryCache) : base(entityRepository, importMemoryCache)
        {
        }
        #endregion

        #region Method
        /// <summary>
        /// Thực hiện nhập khẩu khách hàng
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy: NGDUONG (06/06/2021)
        public override async Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken)
        {
            var customers = ((List<Customer>)CacheGet(importKey)).Where(e => e.ImportValidState == ImportValidState.Valid || (overriderData && e.ImportValidState == ImportValidState.DuplicateInDb)).ToList(); ;

            using var dbContext = new EfDbContext();

            // Danh sách Khách hànd thêm mới:
            var newCustomers = customers.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.Valid).ToList();
            await dbContext.Customer.AddRangeAsync(newCustomers);

            // Danh sách khách hàng thực hiện ghi đè:
            var modifiedCustomers = customers.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.DuplicateInDb).ToList();
            foreach (var customer in modifiedCustomers)
            {
                dbContext.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                var pbd = dbContext.CustomerRecommend.Where(e => e.CustomerCode == customer.CustomerCode);
                dbContext.CustomerRecommend.AddRange(customer.CustomerRecommend);
                dbContext.CustomerRecommend.RemoveRange(pbd);
            }
            await dbContext.SaveChangesAsync();
            return new ActionServiceResult(true, Resources.Msg_ImportSuccess, MISACode.Success, customers);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng theo công ty
        /// </summary>
        /// <returns>Danh sách khách hàng đang có trong công ty</returns>
        /// CreatedBy: NGDUONG (06/06/2021)
        public async Task<List<Customer>> GetCustomers()
        {
            //var currentOrganizationId = CommonUtility.GetCurrentOrganizationId();
            using var dbContext = new EfDbContext();
            var customers = await dbContext.Customer.ToListAsync();
            return customers;
        }
        #endregion
    }
}
