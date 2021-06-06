using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Entities.Directory;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Interfaces.Service;
using MISA.ImportDemo.Core.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    public class ImportCustomerService : BaseImportService, IImportCustomerService
    {
        #region DECLARE
        #endregion
        #region CONSTRUCTOR
        public ImportCustomerService(IImportCustomerRepository importRepository, IMemoryCache importMemoryCache) : base(importRepository, importMemoryCache, "Customer")
        {
            //EntitiesFromDatabase = GetListProfileBookDetailsByProfileBookId().Cast<object>().ToList();
        }
        #endregion

        #region METHOD
        /// <summary>
        /// Thực hiện nhập khẩu dữ liệu
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy: NVMANH (10/10/2020)
        public async Task<ActionServiceResult> Import(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            return await _importRepository.Import(keyImport, overriderData, cancellationToken);
        }

        /// <summary>
        /// Thực hiện đọc dữ liệu từ tệp nhập khẩu
        /// </summary>
        /// <param name="importFile">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">Tham số tùy chọn sử dụng xử lý Task đa luồng</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết quả đọc tệp)</returns>
        /// CreatedBy: NVMANH (10/10/2020)
        public async Task<ActionServiceResult> ReadCustomerDataFromExcel(IFormFile importFile, CancellationToken cancellationToken)
        {
            // Lấy dữ liệu khách hàng trên Db về để thực hiện check trùng:
            EntitiesFromDatabase = (await GetCustomersFromDatabase()).Cast<object>().ToList();
            var customers = await base.ReadDataFromExcel<Customer>(importFile, cancellationToken);
            var importInfo = new ImportInfo(String.Format(Properties.Resources.CustomerImport, Guid.NewGuid()), customers);
            // Lưu dữ liệu vào cache:
            importMemoryCache.Set(importInfo.ImportKey, customers);
            // Lưu các vị trí mới vào cache:
            importMemoryCache.Set(string.Format(Properties.Resources.Position, importInfo.ImportKey), _newPossitons);
            return new ActionServiceResult(true, Resources.Msg_ImportFileReadSuccess, MISACode.Success, importInfo);
        }

        /// <summary>
        ///  Lấy toàn bộ danh sách Nhân viên đang có trong Database theo từng công ty.
        ///  với bộ hồ sơ (ProfileBook) đang nhập khẩu vào - lưu vào cache để thực hiện check trùng
        /// </summary>
        /// CreatedBy: NGDUONG (06/06/2021)
        private async Task<List<Customer>> GetCustomersFromDatabase()
        {
            var importRepository = _importRepository as IImportCustomerRepository;
            return await importRepository.GetCustomers();

        }

        /// <summary>
        /// Check trùng dữ liệu trong File Excel và trong database, dựa vào mã khách hàng
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="entitiesInFile">Danh sách các đối tượng được build từ tệp nhập khẩu</param>
        /// <param name="entity">thực thể hiện tại</param>
        /// <param name="cellValue">Giá trị nhập trong ô excel đang đọc</param>
        /// <param name="importColumn">Thông tin cột nhập khẩu (tiêu đề cột, kiểu giá trị....)</param>
        /// CreatedBy: NVMANH (19/06/2020)
        protected override void CheckDuplicateData<T>(List<T> entitiesInFile, T entity, object cellValue, ImportColumn importColumn)
        {
            if (entity is Customer)
            {
                var newCustomer = entity as Customer;
                // Validate: kiểm tra trùng dữ liệu trong File Excel và trong Database: check theo mã khách hàng
                if (importColumn.ColumnInsert == "CustomerCode" && cellValue != null)
                {
                    var customerCode = cellValue.ToString().Trim();
                    // Check trong File
                    var itemDuplicate = entitiesInFile.Where(item => (item.GetType().GetProperty("CustomerCode").GetValue(item) ?? string.Empty).ToString() == customerCode).FirstOrDefault();
                    if (itemDuplicate != null)
                    {
                        entity.ImportValidState = ImportValidState.DuplicateInFile;
                        itemDuplicate.ImportValidState = ImportValidState.DuplicateInFile;
                        entity.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInFile, entity.GetType().GetProperty("FullName").GetValue(entity).ToString()));
                        itemDuplicate.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInFile, itemDuplicate.GetType().GetProperty("FullName").GetValue(itemDuplicate).ToString()));
                    }
                    //todo: kiem tra trung (5/6/2021)
                    // Check trong Db:
                    var itemDuplicateInDb = EntitiesFromDatabase.Where(item => (item.GetType().GetProperty("CustomerCode").GetValue(item) ?? string.Empty).ToString() == customerCode).Cast<T>().FirstOrDefault();
                    // Kiểm tra lỗi
                    if (itemDuplicateInDb != null)
                    {
                        // Mã lỗi
                        entity.ImportValidState = ImportValidState.DuplicateInDb;
                        newCustomer.CustomerId = (Guid)itemDuplicateInDb.GetType().GetProperty("CustomerId").GetValue(itemDuplicateInDb);
                        itemDuplicateInDb.ImportValidState = ImportValidState.DuplicateInFile;
                        // gán lỗi vào importValidError
                        entity.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInDatabase, entity.GetType().GetProperty("FullName").GetValue(entity).ToString()));
                        itemDuplicateInDb.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInDatabase, itemDuplicateInDb.GetType().GetProperty("FullName").GetValue(itemDuplicateInDb).ToString()));
                    }
                }
            }
            else
            {
                base.CheckDuplicateData(entitiesInFile, entity, cellValue, importColumn);
            }
        }

        /// <summary>
        /// Khởi tạo đối tượng trước khi build các thông tin
        /// Dựa vào thông tin bảng dữ liệu sẽ import dữ liệu vào mà map các đối tượng tương ứng.
        /// </summary>
        /// <typeparam name="T">Kiểu của object</typeparam>
        /// <returns>Thực thể được khởi tạo với kiểu tương ứng</returns>
        /// OverriderBy: NGDUONG (06/06/2021)
        protected override dynamic InstanceEntityBeforeMappingData<T>()
        {
            // Kiểm tra xem nó là bảng nào
            var ImportToTable = ImportWorksheetTemplate.ImportToTable;
            switch (ImportToTable)
            {
                case "Customer":
                    var newEntity = new Customer();
                    newEntity.CustomerId = Guid.NewGuid();
                    return newEntity;
                case "CustomerRecommend":
                    var eFamily = new CustomerRecommend()
                    {
                        CustomerRecommendId = Guid.NewGuid()
                    }; //Activator.CreateInstance("MISA.ImportDemo.Core.Entities", "ProfileFamilyDetail");
                    return eFamily;
                default:
                    return base.InstanceEntityBeforeMappingData<T>();//Hàm khởi tạo chung các thông tin như khóa ... 
            }
        }

        /// <summary>
        /// Sau khi các thông tin được build hoàn chỉnh thì làm một số việc cần thiết
        /// 1. Mapping dữ liệu thông tin thành viên gia đình tương ứng với khách hàng nào
        /// 2. Validate có lỗi gì cụ thể
        /// </summary>
        /// <typeparam name="T">kiểu của object</typeparam>
        /// <param name="entity">object thành viên trong gia đình</param>
        /// OverriderBy: NGDUONG (05/06/2021)
        protected override void ProcessDataAfterBuild<T>(object entity)
        {
            if (entity is CustomerRecommend)
            {
                var customerRecommend = entity as CustomerRecommend;
                var customerCode = customerRecommend.CustomerCode;
                var customerMaster = _entitiesFromEXCEL.Cast<Customer>().Where(pbd => pbd.CustomerCode == customerCode).FirstOrDefault();
                if (customerMaster != null && customerCode != null)
                {
                    customerRecommend.CustomerCode = customerMaster.CustomerCode;
                    customerMaster.CustomerRecommend.Add(customerRecommend);

                    // Duyệt từng lỗi của detail và add thông tin vào master:
                    foreach (var importValidError in customerRecommend.ImportValidError.ToList())
                    {
                        customerMaster.ImportValidError.Add(String.Format(Properties.Resources.CustomerRecommendInfo, customerRecommend.FullName, importValidError));
                    }

                    // Nếu master không có lỗi valid, detail có thì gán lại cờ cho master là invalid:
                    if (customerRecommend.ImportValidState != ImportValidState.Valid && customerMaster.ImportValidState == ImportValidState.Valid)
                        customerMaster.ImportValidState = ImportValidState.Invalid;
                }
            }
            base.ProcessDataAfterBuild<T>(entity);
        }

        ///// <summary>
        ///// Xử lý đặc thù với các thông tin lấy trên tệp nhập khẩu ở dạng lựa chọn thông tin ở 1 danh mục trong database
        ///// </summary>
        ///// <typeparam name="T">kiểu của thực thể đang build</typeparam>
        ///// <param name="entity">thực thể</param>
        ///// <param name="cellValue">giá trị của cell đọc được trên tệp</param>
        ///// <param name="importColumn">thông tin cột nhập khẩu hiện tại được khai báo trong databse</param>
        ///// OverriderBy: NVMANH (15/12/2020)
        //protected override void ProcessCellValueByDataTypeWhenTableReference<T>(object entity, ref object cellValue, ImportColumn importColumn)
        //{
        //    var value = cellValue;
        //    if (importColumn.ObjectReferenceName == "ParticipationForm" && entity is Customer)
        //    {
        //        //var listData = _importRepository.GetListObjectByTableName("ParticipationForm").Result.Cast<ParticipationForm>().ToList();
        //        //var par = listData.Where(e => e.Rate == decimal.Parse(value.ToString().Replace(",","."))).FirstOrDefault();
        //        //if (par == null)
        //        //    return;
        //        //(entity as Employee).ParticipationFormId = par.ParticipationFormId;
        //        //(entity as Employee).ParticipationFormName = par.ParticipationFormName;
        //    }
        //    else
        //    {
        //        base.ProcessCellValueByDataTypeWhenTableReference<T>(entity, ref cellValue, importColumn);
        //    }
        //}

        ///// <summary>
        ///// Xử lý đặc thù với các thông tin lấy trên tệp nhập khẩu ở dạng lựa chọn thông tin lưu trữ có kiểu là Enum (VD giới tính, tình trạng hôn nhân...)
        ///// </summary>
        ///// <typeparam name="T">kiểu của thực thể đang build</typeparam>
        ///// <param name="entity">thực thể</param>
        ///// <param name="enumType">Kiểu của enum</param>
        ///// <param name="cellValue">giá trị của cell đọc được trên tệp</param>
        ///// <param name="importColumn">thông tin cột nhập khẩu hiện tại được khai báo trong databse</param>
        ///// OverriderBy: NVMANH (15/12/2020)
        //protected override void CustomAfterSetCellValueByColumnInsertWhenEnumReference<Y>(object entity, Y enumType, string columnInsert, ref object cellValue)
        //{
        //    if (columnInsert == "ResidentialAreaType")
        //    {
        //        //var employee = entity as Employee;
        //        //var enumPropertyName = (ResidentialAreaType)cellValue;
        //        //employee.ResidentialAreaName = Resources.ResourceManager.GetString(string.Format("Enum_ResidentialAreaType_{0}", enumPropertyName));
        //    }
        //    else
        //    {
        //        base.CustomAfterSetCellValueByColumnInsertWhenEnumReference<Y>(entity, enumType, columnInsert, ref cellValue);
        //    }

        //}

        /// <summary>
        /// Xử lý dữ liệu liên quan đến ngày/ tháng
        /// </summary>
        /// <param name="entity">Thực thế sẽ import vào Db</param>
        /// <param name="cellValue">Giá trị của cell</param>
        /// <param name="type">Kiểu dữ liệu</param>
        /// <param name="importColumn">Thông tin cột import được khai báo trong Db</param>
        /// <returns>giá trị ngày tháng được chuyển đổi tương ứng</returns>
        /// CreatedBy: NGDUONG (05/06/2021)
        protected override DateTime? GetProcessDateTimeValue<T>(T entity, object cellValue, Type type, ImportColumn importColumn = null)
        {
            // Nếu là khách hàng và nhóm khách hàng
            if ((entity is CustomerRecommend || entity is Customer) && importColumn.ColumnInsert == "DateOfBirth")
            {
                DateTime? dateTime = null;// Khởi tạo datetime trước
                try// Kiểm tra có format được sang Datetime không
                {
                    dateTime = DateTime.ParseExact(cellValue.ToString().Trim(), new string[] { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "dd/MM/yyyy", "M/yyyy", "yyyy", "MM/yyyy", "dd.MM.yyyy", "d.MM.yyyy", "dd.M.yyyy", "d.M.yyyy", "dd.MM.yyyy", "M.yyyy", "yyyy", "MM.yyyy", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "dd-MM-yyyy", "M-yyyy", "yyyy", "MM-yyyy" }, CultureInfo.InvariantCulture);    
                }
                catch (Exception)// Thông báo lỗi
                {
                    entity.ImportValidState = ImportValidState.Invalid;
                    entity.ImportValidError.Add(string.Format(Properties.Resources.InValidError, importColumn.ColumnTitle));
                }
                return dateTime;
            }
            // Nếu không vào hàm get datetime mặc định
            return base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
        }
        #endregion
    }
}
