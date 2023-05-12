using BITPay.DBL.Entities;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Repositories
{
    public interface IRefRepository
    {
        IEnumerable<PaymentMode> GetPaymentModes();

        IEnumerable<Branch> GetBranches();
        BaseEntity CreateBranch(Branch branch, int userCode);
        Bank GetBank(int BankCode);
        IEnumerable<Bank> GetBanks();
        BaseEntity CreateBank(Bank bk, int userCode);
    }
}
