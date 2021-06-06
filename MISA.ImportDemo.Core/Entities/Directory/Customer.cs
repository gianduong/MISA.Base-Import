using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.Directory
{
    public class Customer : BaseEntity
    {
        #region DECLARE
        public virtual CustomerGroup CustomerGroup { get; set; }
        public virtual Ethnic Ethnic { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual ICollection<CustomerRecommend> CustomerRecommend { get; set; }
        #endregion

        #region Constructure
        /// <summary>
        /// Hàm khởi tạo của Customer
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public Customer()
        {
            CustomerRecommend = new HashSet<CustomerRecommend>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string CustomerCode { get; set; }

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
        /// Giới tính viết kiêu chữ
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
        /// <summary>
        /// Mã nhóm khách hàng
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public Guid CustomerGroupId { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public string Email { get; set; }
        /// <summary>
        /// Nhóm khách hàng
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        [NotMapped]
        public string CustomerGroupName { get; set; }
        /// <summary>
        /// Mã dân tộc
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public int? EthnicId { get; set; }
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        public int? NationId { get; set; }
        /// <summary>
        /// Tên dân tộc
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        [NotMapped]
        public string NationalityName { get; set; }
        /// <summary>
        /// Tên quốc gia
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        [NotMapped]
        public string EthnicName { get; set; }
        /// <summary>
        /// Sắp xếp
        /// </summary>
        /// CreatedBy: DuongNG (20/05/2021)
        [NotMapped]
        public int? Sort { get; set; }
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

        #region Method


        #endregion

    }
}
