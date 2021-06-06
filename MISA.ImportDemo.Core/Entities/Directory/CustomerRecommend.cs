using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.Directory
{
    public class CustomerRecommend : BaseEntity
    {
        #region DECLARE
        public virtual Customer Customer { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Mã khách hàng được đề xuất
        /// </summary>
        public Guid CustomerRecommendId { get; set; }
        /// <summary>
        /// Mã khách hàng đè xuất
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Khóa của khách hàng đề xuất
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Họ và tên
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string FullName { get; set; }
        /// <summary>
        /// ngày sinh
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// giới tính
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public int? Gender { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Kiểu
        /// </summary>
        public int? SortOrder { get; set; }
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
        /// <summary>
        /// Cách hiển thị
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public int? DobdisplaySetting { get; set; }
        /// <summary>
        /// Kiểu thao tác
        /// </summary>
        public int? Sort { get; set; }


        /// <summary>
        /// Tên giới tính
        /// </summary>
        public string GenderName
        {
            get
            {
                var name = string.Empty;
                if (Gender != null)
                {
                    switch ((Enumeration.Gender)Gender)
                    {
                        case Enumeration.Gender.Female:
                            name = Properties.Resources.Enum_Gender_Female;
                            break;
                        case Enumeration.Gender.Male:
                            name = Properties.Resources.Enum_Gender_Male;
                            break;
                        case Enumeration.Gender.Other:
                            name = Properties.Resources.Enum_Gender_Other;
                            break;
                        default:
                            break;
                    }
                }
                return name;
            }
        }
        #endregion
    }
}
