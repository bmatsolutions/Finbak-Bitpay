using System;
using System.Collections.Generic;
using System.Text;
using BITPay.DBL.Repositories;

namespace BITPay.DBL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private string connString;
        private bool _disposed;

        private ISecurityRepository securityRepository;
        private ITaxRepository taxRepository;
        private IGeneralRepository generalRepository;
        private IRefRepository refRepository;
        private IReportRepository reportRepository;
        private IPaywayGatewayRepository paywayGatewayRepository;
        private IDomesticRepository domesticRepository;
        private IMiarieRepository miarieRepository;
        private IRegidesoRepository regidesoRepository;

        public UnitOfWork(string connectionString)
        {
            connString = connectionString;
        }

        public ISecurityRepository SecurityRepository
        {
            get { return securityRepository ?? (securityRepository = new SecurityRepository(connString)); }
        }

        public ITaxRepository TaxRepository
        {
            get { return taxRepository ?? (taxRepository = new TaxRepository(connString)); }
        }

        public IGeneralRepository GeneralRepository
        {
            get { return generalRepository ?? (generalRepository = new GeneralRepository(connString)); }
        }

        public IRefRepository RefRepository
        {
            get { return refRepository ?? (refRepository = new RefRepository(connString)); }
        }

        public IReportRepository ReportRepository
        {
            get { return reportRepository ?? (reportRepository = new ReportRepository(connString)); }
        }

        public IPaywayGatewayRepository PaywayGatewayRepository
        {
            get { return paywayGatewayRepository ?? (paywayGatewayRepository = new PaywayGatewayRepository(connString)); }
        }

        public IDomesticRepository DomesticRepository
        {
            get { return domesticRepository ?? (domesticRepository = new DomesticRepository(connString)); }
        }

        public IMiarieRepository MiarieRepository
        {
            get { return miarieRepository ?? (miarieRepository = new MiarieRepository(connString)); }
        }

        public IRegidesoRepository RegidesoRepository
        {
            get { return regidesoRepository ?? (regidesoRepository = new RegidesoRepository(connString)); }
        }

        public void Reset()
        {
            securityRepository = null;
            taxRepository = null;
            generalRepository = null;
            refRepository = null;
            reportRepository = null;
            paywayGatewayRepository = null;
            domesticRepository = null;
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
