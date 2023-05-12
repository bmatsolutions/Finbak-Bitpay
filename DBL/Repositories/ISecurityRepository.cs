using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public interface ISecurityRepository
    {
        GenericModel VerifyUser(string userName);
        GenericModel UserLogin(int userCode, int status);
        GenericModel GetUserPassword(int userCode);
        BaseEntity ChangeUserPassword(int userCode, string password, string salt);
        UserProfileModel GetUserProfile(int userCode);

        IEnumerable<SysUserModel> GetUsers();
        SysUserModel GetUser(int code);
        GenericModel CreateUser(User user, int creator);
        BaseEntity UpdateUserCashAccount(int userCode, string account);
        GenericModel ResetUserPassword(int userCode, string salt, string password);

        IEnumerable<ApprovalItemModel> GetApprovalItems(DateTime? from = null, DateTime? to = null, int typ = 0);
        ApprovalItemModel GetApprovalItem( int id);
        BaseEntity ItemApprovalAction(int logId, int action, string reason, int userCode);
        vwUser Get_User(int code);
        GenericModel UpdateUser(vwUser user, int creator);
        GenericModel BlockUser(int userCode, int Maker, int mode);
        
        IEnumerable<AuditModel> GetAudit(int usercode, string assesNo, string dateFrom, string dateTo);
        IEnumerable<Syssetting> GetSysSettings();
        Syssetting GetSyssetting(int code);
        vwSystemSett Get_Systemsett(int code);
        GenericModel UpdateParams(vwSystemSett user, int makerUser);
        IEnumerable<ApprovalParamsModel> GetParamItems(DateTime? from, DateTime? to, int typ);
        ApprovalParamsModel GetApprovalParams(int id);
        BaseEntity ParamsApprovalAction(int id, int action, string reason, int userCode);
        IEnumerable<FailedTransactions> GetFailedPayments(int usercode, string dateFrom, string dateTo);
        GenericModel UpdateFailedTransactions(int usercode, int code);
    }
}
