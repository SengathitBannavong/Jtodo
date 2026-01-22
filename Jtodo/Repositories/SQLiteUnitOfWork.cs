using Jtodo.Domains;
using Jtodo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtodo.Repositories
{
    public class SQLiteUnitOfWork : IAppUnit
    {
        private readonly SQLite _context;
        private TodoListRepository? _todoListRepository;
        private TypeRepository? _typeRepository;
        private TodoItemRepository? _todoItemRepository;
        private bool _disposed = false;

        public SQLiteUnitOfWork()
        {
            _context = new SQLite();
        }

        public SQLiteUnitOfWork(SQLite context)
        {
            _context = context;
        }

        public TodoListRepository TodoListRepository
        {
            get
            {
                _todoListRepository ??= new TodoListRepository(_context);
                return _todoListRepository;
            }
        }

        public TypeRepository TypeRepository
        {
            get
            {
                _typeRepository ??= new TypeRepository(_context);
                return _typeRepository;
            }
        }

        public TodoItemRepository TodoItemRepository
        {
            get
            {
                _todoItemRepository ??= new TodoItemRepository(_context);
                return _todoItemRepository;
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
