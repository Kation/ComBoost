using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class DatabaseTransaction : IDatabaseTransaction
    {
        private IDbContextTransaction _transaction;

        public DatabaseTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }
                
        public void Dispose()
        {
            if (_transaction == null)
                return;
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
