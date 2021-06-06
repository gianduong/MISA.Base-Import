using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.Directory
{
    public class CustomerGroup
    {
        #region DECLARE
        public virtual ICollection<Customer> Customer { get; set; }
        #endregion

        #region Constructure
        /// <summary>
        /// Hàm khởi tạo của CustomerGroup
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public CustomerGroup()
        {
            Customer = new HashSet<Customer>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Mã nhóm khách hàng
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public Guid CustomerGroupId { get; set; }
        /// <summary>
        /// Tên nhóm khách hàng
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string CustomerGroupName { get; set; }
        /// <summary>
        /// Mô tả nếu có
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string Description { get; set; }
        /// <summary>
        /// Ngày khởi tạo
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Người khởi tạo
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string CreatedBy { get; set; }
        /// <summary>
        /// Ngày Sửa đổi
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// Người sửa đổi
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string ModifiedBy { get; set; }
        #endregion
    }
}
