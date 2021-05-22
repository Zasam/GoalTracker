using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GoalTracker.UnitTest
{
    public class InMemoryConnectionManager : IDisposable
    {
        private readonly DbConnection connection;

        public InMemoryConnectionManager(DbContextOptions contextOptions)
        {
            connection = RelationalOptionsExtension.Extract(contextOptions).Connection;
        }

        public static DbConnection CreateInMemoryConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}