using SQLiteTest;
using Xunit;

namespace SQLiteTest.Test {
    public class TestSQLite {
        [Theory]
        [InlineData ("Data Source=test.db", 100, 100)]
        [InlineData ("Data Source=test.db;BusyTimeout=100", 100, 100)]
        [InlineData ("Data Source=test.db;Journal Mode=Wal", 100, 100)]
        [InlineData ("Data Source=test.db;Journal Mode=Wal;Page Size=4096", 100, 100)]
        [InlineData ("Data Source=test.db;Journal Mode=Wal;BusyTimeout=100", 100, 100)]
        [InlineData ("Data Source=test.db;Journal Mode=Wal;Synchronous=Normal", 100, 100)]
        [InlineData ("Data Source=test.db;Journal Mode=Wal;Synchronous=Normal;BusyTimeout=100", 100, 100)]
        public void ReturnFalseGivenValueOf1 (string connectionString, int threadCount, int operationCount) {
            new SQLiteTester (connectionString, threadCount, operationCount).Run ();
        }
    }
}