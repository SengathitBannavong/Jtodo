using System;
using System.Collections.Generic;
using System.Text;

namespace Jtodo.Repositories
{
    public class SQLiteUnitOfWork : IAppUnit
    {
        private readonly SQLite _context;
        private TodoListRespository? _todoListRepository;
        private bool _disposed = false;

        public SQLiteUnitOfWork()
        {
            _context = new SQLite();
        }

        public SQLiteUnitOfWork(SQLite context)
        {
            _context = context;
        }

        public TodoListRespository TodoListRepository
        {
            get
            {
                _todoListRepository ??= new TodoListRespository(_context);
                return _todoListRepository;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
