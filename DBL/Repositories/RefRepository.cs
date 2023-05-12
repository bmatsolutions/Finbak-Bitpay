using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BITPay.DBL.Entities;
using Dapper;

namespace BITPay.DBL.Repositories
{
    public class RefRepository : BaseRepository, IRefRepository
    {
        public RefRepository(string connectionString) : base(connectionString)
        {
        }

        public BaseEntity CreateBranch(Branch branch, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@BranchName", branch.BranchName.ToUpper());
                parameters.Add("@CBS_Code", branch.CBS_Code);
                parameters.Add("@Cash_GL_Account", branch.Cash_GL_Account);
                parameters.Add("@UserCode", userCode);

                return connection.Query<BaseEntity>("sp_CreateBranch", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public IEnumerable<Branch> GetBranches()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<Branch>(GetAllStatement(Branch.TableName)).ToList();
            }
        }

        public BaseEntity CreateBank(Bank bk, int userCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@mode", bk.mode);
                parameters.Add("@BankID", bk.BankID);
                parameters.Add("@BankCode", bk.BankCode);
                parameters.Add("@BankName", bk.BankName.ToUpper());             
                parameters.Add("@UserCode", userCode);
                return connection.Query<BaseEntity>("sp_CreateBank", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public Bank GetBank(int BankCode)
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<Bank>(
                    FindStatement("vw_Banks", "BankCode"),
                    param: new { Id = BankCode }
                    ).FirstOrDefault();
            }
        }

        public IEnumerable<Bank> GetBanks()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<Bank>(GetAllStatement(Bank.TableName)).ToList();
            }
        }

        public IEnumerable<PaymentMode> GetPaymentModes()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

                return connection.Query<PaymentMode>(GetAllStatement(PaymentMode.TableName)).ToList();
            }
        }
    }
}
