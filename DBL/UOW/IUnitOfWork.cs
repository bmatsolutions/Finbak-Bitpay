using BITPay.DBL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.UOW
{
    public interface IUnitOfWork
    {
        ISecurityRepository SecurityRepository { get; }
        ITaxRepository TaxRepository { get; }
        IGeneralRepository GeneralRepository { get; }
        IRefRepository RefRepository { get; }
        IReportRepository ReportRepository { get; }
        IDomesticRepository DomesticRepository { get; }
        IMiarieRepository MiarieRepository { get; }
        IRegidesoRepository RegidesoRepository { get; }
        void Reset();
    }
}
