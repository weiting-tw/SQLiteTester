using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;
using Dapper;
using McMaster.Extensions.CommandLineUtils;

namespace SQLiteTest
{
    public class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-c|--connection-string <CONNECTION_STRING>", Description = "Connection String. Defaults to 'Data Source=test.db'")]
        public string ConnectionString { get; } = "Data Source=test.db";

        [Option("-t|--threads <COUNT>", Description = "Thread Count. Defaults to 8")]
        public int ThreadCount { get; } = 8;

        [Option("-o|--operations <COUNT>", Description = "Operation Count. Defaults to 1,000")]
        public int OperationCount { get; } = 1000;

        private void OnExecute()
        {
            new SQLiteTester(ConnectionString, ThreadCount, OperationCount).Run();
        }
    }

    public class SQLiteTester
    {
        private readonly string _connectionString;
        private readonly int _operationsCount;
        private readonly int _threadCount;
        private int _errorCount = 0;

        public SQLiteTester(string connectionString, int threadCount, int operationCount)
        {
            Console.WriteLine($"Initialize Database with ConnectionString: '{connectionString}' ...");
            var builder = new SQLiteConnectionStringBuilder(connectionString);
            _connectionString = builder.ConnectionString;
            _threadCount = threadCount;
            _operationsCount = operationCount;
            using(var conn = CreateConnection())
            using(var tx = conn.BeginTransaction())
            {
                conn.Execute("DROP TABLE IF EXISTS Tests");
                conn.Execute("CREATE TABLE IF NOT EXISTS Tests (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)");
                for (var i = 1; i <= _threadCount; i++)
                {
                    conn.Execute("INSERT INTO Tests (Id,Name) VALUES (@Id, @Name)", new { Id = i, Name = "test" });
                }
                tx.Commit();
            }
        }

        public void Run()
        {
            Console.WriteLine($"Runing tests with {_threadCount} threads and {_operationsCount:n0} operations-per-thread ...");
            var sw = Stopwatch.StartNew();
            var workers = new List<Thread>();
            for (var i = 1; i <= _threadCount; i++)
            {
                var id = i; // HACK: prevent captured variable in loop
                var worker = new Thread(() => UpdateRecord(id));
                workers.Add(worker);
                worker.Start();
            }
            foreach (var worker in workers)
            {
                worker.Join();
            }
            sw.Stop();
            Console.WriteLine($"Done in {sw.ElapsedMilliseconds:n0} ms with errors: {_errorCount}");
        }

        private IDbConnection CreateConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private void UpdateRecord(int id)
        {
            using(var conn = CreateConnection())
            for (var i = 0; i < _operationsCount; i++)
            {
                try
                {
                    using(var tx = conn.BeginTransaction())
                    {
                        conn.Execute(
                            "UPDATE Tests SET Name = @Name WHERE Id = @Id", new { Name = Guid.NewGuid().ToString(), Id = id });
                        tx.Commit();
                    }
                }
                catch (Exception)
                {
                    Interlocked.Increment(ref _errorCount);
                }
            }
        }
    }
}