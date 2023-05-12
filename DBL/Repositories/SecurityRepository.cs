using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System.Threading.Tasks;

namespace BITPay.DBL.Repositories
{
    public class SecurityRepository : BaseRepository, ISecurityRepository
    {
        public SecurityRepository(string connectionString) : base(connectionString)
        {
        }

        public BaseEntity ChangeUserPassword(int userCode, string password, string salt)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);
                parameters.Add("@Pwd", password);
                parameters.Add("@Salt", salt);

                return connection.Query<BaseEntity>("sp_ChangeUserPassword", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel CreateUser(User user, int creator)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserName", user.UserName);
                parameters.Add("@UserRole", user.UserRole);
                parameters.Add("@FullNames", user.FullNames);
                parameters.Add("@Email", user.Email);
                parameters.Add("@Salt", user.Salt);
                parameters.Add("@Pwd", user.Pwd);
                parameters.Add("@BranchCode", user.BranchCode);
                parameters.Add("@Maker", creator);

                return connection.Query<GenericModel>("sp_CreateUser", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
        public GenericModel UpdateUser(vwUser user, int creator)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", user.UserCode);
                parameters.Add("@UserName", user.UserName);
                parameters.Add("@FullNames", user.FullNames);
                parameters.Add("@Email", user.Email);
                parameters.Add("@UserRole", user.UserRole);
                parameters.Add("@BranchCode", user.BranchCode);
                parameters.Add("@Maker", creator);
                return connection.Query<GenericModel>("sp_modifyUser", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel UpdateParams(vwSystemSett user, int creator)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                int mode = 1;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", user.Id);
                parameters.Add("@ItemName", user.ItemName);
                parameters.Add("@ItemValue", user.ItemValue);
                parameters.Add("@Descr", user.Descr);
                parameters.Add("@Maker", creator);
                parameters.Add("@mode", mode);
                return connection.Query<GenericModel>("sp_modifySystemParam", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel BlockUser(int userCode, int Maker,int mode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);
                parameters.Add("@Maker", Maker);
                parameters.Add("@mode", mode);
                return connection.Query<GenericModel>("sp_BlockUser", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public ApprovalItemModel GetApprovalItem(int id)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<ApprovalItemModel>(
                    FindStatement("vw_ApprovalItems", "LogId"),
                    param: new { Id = id }
                    ).FirstOrDefault();
            }
        }

        public ApprovalParamsModel GetApprovalParams(int id)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<ApprovalParamsModel>(
                    FindStatement("vw_SysSettingApproval", "idxno"),
                    param: new { Id = id }
                    ).FirstOrDefault();
            }
        }

        public IEnumerable<ApprovalItemModel> GetApprovalItems(DateTime? from = null, DateTime? to = null, int typ = 0)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<ApprovalItemModel>(GetAllStatement("vw_ApprovalItems")).ToList();
            }
        }

        public IEnumerable<ApprovalParamsModel> GetParamItems(DateTime? from = null, DateTime? to = null, int typ = 0)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<ApprovalParamsModel>(GetAllStatement("vw_SysSettingApproval")).ToList();
            }
        }

        public SysUserModel GetUser(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<SysUserModel>(
                      FindStatement("vw_Users", "UserCode"),
                      param: new { Id = code }
                  ).FirstOrDefault();
            }
        }

        public Syssetting GetSyssetting(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<Syssetting>(
                      FindStatement("vw_Systemsett", "Id"),
                      param: new { Id = code }
                  ).FirstOrDefault();
            }
        }
        public vwUser Get_User(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<vwUser>(
                      FindStatement("vw_Users", "UserCode"),
                      param: new { Id = code }
                  ).FirstOrDefault();
            }
        }

        public vwSystemSett Get_Systemsett(int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<vwSystemSett>(
                      FindStatement("vw_SystemSett", "Id"),
                      param: new { Id = code }
                  ).FirstOrDefault();
            }
        }

        public GenericModel GetUserPassword(int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);

                return connection.Query<GenericModel>("sp_GetUserPassword", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public UserProfileModel GetUserProfile(int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Code", userCode);

                string sql = "Select * From vw_UserProfile Where UserCode = @Code";
                return connection.Query<UserProfileModel>(sql, parameters, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public IEnumerable<SysUserModel> GetUsers()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<SysUserModel>(GetAllStatement("vw_Users")).ToList();
            }
        }

        public IEnumerable<Syssetting> GetSysSettings()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<Syssetting>(GetAllStatement("vw_Systemsett")).ToList();
            }
        }

        public BaseEntity ItemApprovalAction(int logId, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@LogId", logId);
                parameters.Add("@Action", action);
                parameters.Add("@Reason", reason);
                parameters.Add("@UserCode", userCode);

                return connection.Query<BaseEntity>("sp_ItemApprovalAction", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public BaseEntity ParamsApprovalAction(int logId, int action, string reason, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TxnNo", logId);
                parameters.Add("@flg", action);

                parameters.Add("@Reason", reason);
                parameters.Add("@CheckerId", userCode);

                return connection.Query<BaseEntity>("Sp_MCAuthorize", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel ResetUserPassword(int userCode, string salt, string password)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);
                parameters.Add("@Salt", salt);
                parameters.Add("@Password", password);

                return connection.Query<GenericModel>("sp_ResetUserPassword", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public BaseEntity UpdateUserCashAccount(int userCode, string account)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);
                parameters.Add("@Account", account);

                return connection.Query<BaseEntity>("sp_UpdateUserCashAccount", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel UserLogin(int userCode, int status)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserCode", userCode);
                parameters.Add("@LoginStatus", status);

                return connection.Query<GenericModel>("sp_UserLogin", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public GenericModel VerifyUser(string userName)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserName", userName);

                return connection.Query<GenericModel>("sp_VerifyUser", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<AuditModel> GetAudit(int usercode, string assesNo, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@assesNo", assesNo);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);
                return connection.Query<AuditModel>("sp_GetAudit", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public IEnumerable<FailedTransactions> GetFailedPayments(int usercode, string dateFrom, string dateTo)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@usercode", usercode);
                parameters.Add("@dateFrom", dateFrom);
                parameters.Add("@dateTo", dateTo);
                return connection.Query<FailedTransactions>("sp_GetFailedPayments", parameters, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public GenericModel UpdateFailedTransactions(int usercode, int code)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@user", usercode);
                parameters.Add("@filecode", code);

                return connection.Query<GenericModel>("sp_UpdateFailedPayments", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
    }
}
