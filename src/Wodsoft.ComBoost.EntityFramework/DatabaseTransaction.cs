using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Wodsoft.ComBoost.Data.Entity
{
    public class DatabaseTransaction : IDatabaseTransaction
    {
        private DbContextTransaction _transaction;

        public DatabaseTransaction(DbContextTransaction transaction)
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
