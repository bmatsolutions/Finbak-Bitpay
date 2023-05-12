using BITPay.DBL.Entities;
using BITPay.DBL.Enums;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface IGeneralRepository
    {
        IEnumerable<ListModel> GetListModel(ListModelType listType);
        Task<GenericModel> GetSystemSettingAsync(SettingType sType);
        DBoardModel GetDBoardData(int userCode);
        IEnumerable<ListModel> GetPayMode(int mode);
        DataTable ValidateCBSRef(string connection, string cbsRef, decimal expectedAmount, string dclntName, string accredit, string schema, string date, string data8);
        Task<BaseEntity> CheckDuplicate(int filecode);
    }
}
