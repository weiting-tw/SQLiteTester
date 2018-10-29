## 執行測試

可在專案目錄下利用 dotnet-core SDK 執行上述程式

```sh
# 顯示使用說明
$ dotnet run -- --help
Usage: SQLiteTest [options]

Options:
  -c|--connection-string <CONNECTION_STRING>  Connection String. Defaults to 'Data Source=test.db'
  -t|--threads <COUNT>                        Thread Count. Defaults to 8
  -o|--operations <COUNT>                     Operation Count. Defaults to 1000
  -?|-h|--help                                Show help information

# 以預設組態執行測試
$ dotnet run
Initialize Database with ConnectionString: 'Data Source=test.db' ...
Runing tests with 8 threads and 1000 operations-per-thread ...
Done in 192,996 ms with errors: 18

# 指定執行緒與操作次數
$ dotnet run --threads 10 --operations 100
Initialize Database with ConnectionString: 'Data Source=test.db' ...
Runing tests with 10 threads and 100 operations-per-thread ...
Done in 24,173 ms with errors: 0

# 指定 Connection String
$ dotnet run --connection-string "Data Source=test.db;Journal Mode=Wal"
Initialize Database with ConnectionString: 'Data Source=test.db;Journal Mode=Wal' ...
Runing tests with 8 threads and 1000 operations-per-thread ...
Done in 17,910 ms with errors: 0
```

## 測試數據

以 8 個執行緒，每個執行緒進行 1,000 次操作，總計 8,000 次來進行每次測試

| 連線字串 | 花費時間(毫秒) | 錯誤次數 |
|:--------|------------------------:|---------:|
| Data Source=test.db | 192,996 | 18 |
| Data Source=test.db;BusyTimeout=100 | 70,629 | 0 |
| Data Source=test.db;Journal Mode=Wal | 17,910 | 0 |
| Data Source=test.db;Journal Mode=Wal;Page Size=4096 | 19,431 | 0 |
| Data Source=test.db;Journal Mode=Wal;BusyTimeout=100 | 14,500 | 0 |
| Data Source=test.db;Journal Mode=Wal;Synchronous=Normal | 1,412 | 0 |
| Data Source=test.db;Journal Mode=Wal;Synchronous=Normal;BusyTimeout=100 | 1,609 | 0 |

## [連線字串(ConnectionString)](https://system.data.sqlite.org/index.html/artifact?ci=trunk&&filename=System.Data.SQLite/SQLiteConnectionStringBuilder.cs)

以;分隔的參數

### Data Source (必填)

設定值:

- 一個檔案名稱
- ":memory:"字串
- 任何支持的URI (從 SQLite 3.7.7版開始)

> 從 1.0.86.0 開始，為了連續使用超過一個反斜線（例如一個 UNC 路徑），
> 每個相鄰的反斜線字元必須雙份
> (例如 "\\Network\Share\test.db" 需變成"\\\\Network\Share\test.db")

### Version

設定值: 3

預設值: 3

### UseUTF16Encoding

設定值:

- True
- False

預設值: False

### DateTimeFormat

設定值:

- Ticks: 使用 DateTime.Ticks 的值
- ISO8601: 使用 [ISO8601](http://en.wikipedia.org/wiki/ISO8601) 格式 
- JulianDay: 使用 [儒略日](http://en.wikipedia.org/wiki/Julian_day)
- UnixEpoch: 使用自 [UNIX Epoch](http://en.wikipedia.org/wiki/Unix_epoch) (1970-01-01T00:00:00Z 到目前的總秒數)
- InvariantCulture: 使用 .NET Framework 與文化無關的字串值
- CurrentCulture: 使用 .NET Framework 與目前文化相關的字串值

預設值: ISO8601

### DateTimeKind

設定值:

- Unspecified: 不指定 UTC 或本地時間
- Utc: 時間為 UTC 時間
- Local: 時間為本地時間

預設值: Unspecified

### DateTimeFormatString

設定值: 用來格式化或分析時間值的格式化字串

預設值: null

> 預設 UTC 時間會是 `yyyy-MM-dd HH:mm:ss.FFFFFFFK`
> 而非 UTC 時間會是 `yyyy-MM-dd HH:mm:ss.FFFFFFF`

### BaseSchemaName

設定值: Some base data classes in the framework (e.g. those that build SQL queries dynamically) assume that an ADO.NET provider cannot support an alternate catalog (i.e. database) without supporting alternate schemas as well; however, SQLite does not fit into this model. Therefore, this value is used as a placeholder and removed prior to preparing any SQL statements that may contain it.

預設值: `sqlite_default_schema`

### BinaryGUID

設定值:

- True: 以二進制方式儲存 GUID 欄位
- False: 以字串儲存 GUID 欄位

預設值: True

### Cache Size

設定值: Size in Bytes

預設值: 2000

### Synchronous

設定值:

- Normal: 在大多數關鍵時刻會同步回資料庫,但比 Full 要少一些
- Full: 在所有寫入動作時完整同步回資料庫
- Off: 由底層作業系統處理 IO 同步

預設值: Full

### Page Size

設定值: Size in Bytes

The default cluster size for a Windows NTFS system seems to be 4096 bytes. Setting the SQLite database page size to the same size will speed up your database on systems where the cluster size is the same

預設值: 1024

### Password

設定值: Using this parameter requires that the CryptoAPI based codec be enabled at compile-time for both the native interop assembly and the core managed assemblies; otherwise, using this parameter may result in an exception being thrown when attempting to open the connection.

### HexPassword

設定值: Must contain a sequence of zero or more hexadecimal encoded byte values without a leading "0x" prefix.  Using this parameter requires that the CryptoAPI based codec be enabled at compile-time for both the native interop assembly and the core managed assemblies; otherwise, using this parameter may result in an exception being thrown when attempting to open the connection.

### Enlist

設定值:

- Y: Automatically enlist in distributed transactions
- N: No automatic enlistment

預設值: Y

### Pooling

設定值:

- True: Use connection pooling.
- False: Do not use connection pooling.

**WARNING**: _When using the default connection pool implementation,
setting this property to True should be avoided by applications that make
use of COM (either directly or indirectly) due to possible deadlocks that
can occur during the finalization of some COM objects._

預設值: False

### FailIfMissing

設定值:

- True: 不自動建立資料庫
- False: 自動建立資料庫,如果資料庫檔案不存在

預設值: False

### Max Page Count

設定值: Limits the maximum number of pages (limits the size) of the database

預設值: 0

### Legacy Format

設定值:

- True: 使用相容舊版 3.x 資料庫格式
- False: 使用 3.3x 新版資料庫格式,於壓縮數字時更有效率

預設值: False

### Default Timeout

設定值:

- {time in seconds}: 預設指令逾時秒數

預設值: 30

### [BusyTimeout](https://www.sqlite.org/c3ref/busy_timeout.html)

> System.Data.SQLite 可用版本 >= 1.0.98.0

SQLite 同一時間只支援單一程序對資料庫進行寫入操作，
預設 BusyTimeout 設定值為 0 時，即一旦當發現同一時間有其它程序也正在寫入而造成資料庫鎖定時，將會立即丟出 [SQLITE_BUSY](https://www.sqlite.org/rescode.html#busy)，即 Database is locked 錯誤

設定值:

- {time in milliseconds}: 當發現同一時間有其它程序也正在寫入而造成資料庫鎖定時,要等候多少毫秒後重試

預設值: 0 

### Journal Mode

設定值:

- Delete: 每次交易結束時，回滾日誌檔（rollback journal）就會被刪除，刪除檔案的動作會使交易提交（commit）。※ 注意：此模式是 SQLite 的預設模式。
- Persist: 每次交易結束時，並不刪除回滾日誌檔，而只是在檔頭填充 0，這如此會阻止其他的資料庫連結使用此檔進行 rollback。此模式在某些平臺上是一種最佳解，特別是刪除（DELETE）或清空日誌檔（TRUNCATE）比以「0」覆寫檔案第一區塊代價高的時候。
- Off: 關閉日誌模式，但這樣就不能使用交易了。所以不可以使用 ROLLBACK 指令，而且如果 AP 突然 crash 掉，資料庫便極有可能毀損。
- Truncate: 只會將回滾日誌檔內容清空（檔案大小為 0），而不是刪除它。在大部分的 OS 上，這會比 DELETE 模式速度快（因為不用刪除檔，所以所屬的目錄內容也不需要改變）。
- Memory: 只將回滾日誌檔案儲存到記憶體中，節省了DISK I/O，但帶來的代價是穩定性和完整性上的損失。但如果交易過程 crash 掉了，資料庫就極有可能損壞。
- Wal: 也就是以「Write-Ahead Log」來取代「回滾日誌檔」（rollback journal）。此模式是持久化的，也就是在多個資料庫連結都會生效，且重新打開資料庫以後，仍然有效。該模式只在 3.7.0 以後才有效（過程會生成兩個檔：.shm 和 .wal）。

預設值: Delete

### Read Only

設定值:

- True: 以唯讀方式開啟資料庫
- False: 以正常可讀寫方式開啟資料庫

預設值: False

### Max Pool Size

設定值: The maximum number of connections for the given connection string that can be in the connection pool

預設值: 100

### Default IsolationLevel

設定值: The default transaction isolation level

- Serializable
- ReadCommitted

預設值: Serializable

### Foreign Keys

設定值: Enable foreign key constraints

預設值: False

### Flags

設定值: Extra behavioral flags for the connection.

- None
- LogPrepare: Enable logging of all SQL statements to be prepared.
- LogPreBind: Enable logging of all bound parameter types and raw values.
- LogBind: Enable logging of all bound parameter strongly typed values.
- LogCallbackException: Enable logging of all exceptions caught from user-provided managed code called from native code via delegates.
- LogBackup: Enable logging of backup API errors.
- NoExtensionFunctions: Skip adding the extension functions provided by the native interop assembly.
- BindUInt32AsInt64: When binding parameter values with the UInt32 type, use the interop method that accepts an Int64 value
- BindAllAsText: When binding parameter values, always bind them as though they were plain text (i.e. no numeric, date/time, or other conversions should be attempted).
- GetAllAsText: When returning column values, always return them as though they were plain text (i.e. no numeric, date/time, or other conversions should be attempted).
- NoLoadExtension: Prevent this SQLiteConnection object instance from loading extensions.
- NoCreateModule: Prevent this SQLiteConnection object instance from creating virtual table modules.
- NoBindFunctions: Skip binding any functions provided by other managed assemblies when opening the connection.
- NoLogModule: Skip setting the logging related properties of the SQLiteModule object instance that was passed to the SQLiteConnection.CreateModule method.
- LogModuleError: Enable logging of all virtual table module errors seen by the SQLiteModule.SetTableError(IntPtr,String) method.
- LogModuleException: Enable logging of certain virtual table module exceptions that cannot be easily discovered via other means.
- TraceWarning: Enable tracing of potentially important [non-fatal] error conditions that cannot be easily reported through other means.
- ConvertInvariantText: When binding parameter values, always use the invariant culture when converting their values from strings.
- BindInvariantText: When binding parameter values, always use the invariant culture when converting their values to strings.
- NoConnectionPool: Disable using the connection pool by default.  If the "Pooling" connection string property is specified, its value will override this flag.  The precise outcome of combining this flag with the UseConnectionPool flag is unspecified; however, one of the flags will be in effect.
- UseConnectionPool: Enable using the connection pool by default.  If the "Pooling" connection string property is specified, its value will override this flag.  The precise outcome of combining this flag with the UseConnectionPool flag is unspecified; however, one of the flags will be in effect.
- BindAndGetAllAsText: BindAllAsText | GetAllAsText
- ConvertAndBindInvariantText: ConvertInvariantText | BindInvariantText
- BindAndGetAllAsInvariantText: BindAndGetAllAsText | BindInvariantText
- ConvertAndBindAndGetAllAsInvariantText: BindAndGetAllAsText | ConvertAndBindInvariantText
- LogAll: LogPrepare | LogPreBind | LogBind | LogCallbackException | LogBackup | LogModuleError | LogModuleException
- Default: LogCallbackException | LogModuleException

預設值: Default

### SetDefaults

設定值:

- True: Apply the default connection settings to the opened database.
- False: Skip applying the default connection settings to the opened database.

預設值: True

### ToFullPath

設定值:

- True: Attempt to expand the data source file name to a fully qualified path before opening.
- False: Skip attempting to expand the data source file name to a fully qualified path before opening.

預設值: True

### PrepareRetries

設定值:

The maximum number of retries when preparing SQL to be executed.  This
normally only applies to preparation errors resulting from the database
schema being changed.

預設值: 30

## SQLite 環境變數設定

從 System.Data.SQLite 1.0.91 之後可以設定於 [System.Data.SQLite.dll.config](https://system.data.sqlite.org/index.html/artifact?ci=trunk&filename=System.Data.SQLite/Configurations/System.Data.SQLite.dll.config) 的組態中

### Force_SQLiteLog

If this configuration variable is set [to anything], the SQLite
logging subsystem may be initialized in a non-default application
domain.  By default, this is not allowed due to the potential
for application domain unloading issues

### No_PreLoadSQLite

If this configuration variable is set [to anything], the native
library pre-loading functionality will be disabled.  By default,
the native library pre-loading will attempt to load the native
SQLite library from architecture-specific (e.g. "x86", "amd64",
"x64") or platform-specific (e.g. "Win32") directories that
reside underneath the application base directory.

### No_SQLiteConnectionNewParser

If this configuration variable is set [to anything], the new
connection string parsing algorithm will not be used.  This
environment variable is intended for use with legacy code only.

### No_SQLiteFunctions

If this configuration variable is set [to anything], the initial
search for types in all loaded assemblies that are tagged with
the SQLiteFunction attribute will be skipped.  Normally, this
search is conducted only once per application domain by the
static constructor of the SQLiteFunction class; however, these
implementation details are subject to change.

### PreLoadSQLite_BaseDirectory

If this configuration variable is set [to anything], it will be
used instead of the application base directory by the native
library pre-loader.  This environment variable can be especially
useful in ASP.NET and other hosted environments where direct
control of the location of the managed assemblies is not under
the control of the application.

### PreLoadSQLite_ProcessorArchitecture

If this configuration variable is set [to anything], it will be
used instead of the processor architecture value contained in the
PROCESSOR_ARCHITECTURE environment variable to help build the
path of the native library to pre-load.

### PreLoadSQLite_NoSearchForDirectory

If this environment variable is set [to anything], the native
library pre-loading code will skip conducting a search for the
native library to pre-load.  By default, the search starts in the
location of the currently executing assembly (i.e. the assembly
containing all the managed components for System.Data.SQLite) and
then falls back to the application domain base directory.

### PreLoadSQLite_UseAssemblyDirectory

If this configuration variable is set [to anything], the location
of the currently executing assembly (i.e. the one containing all
the managed components for System.Data.SQLite) will be used as
the basis for locating the the native library to pre-load (i.e.
instead of using the application domain base directory).

### PROCESSOR_ARCHITECTURE

This configuration variable is normally set by the operating
system itself and should reflect the native processor
architecture of the current process (e.g. a 32-bit x86
application running on a 64-bit x64 operating system should have
the value "x86").

### SQLite_ForceLogPrepare

If this environment variable is set [to anything], all calls to
prepare a SQL query will be logged, regardless of the flags for
the associated connection.

### TypeName_SQLiteProviderServices

If this environment variable is set [to anything], it will be
used by the System.Data.SQLite.SQLiteFactory class as the type
name containing the System.Data.Common.DbProviderServices
implementation that should be used.
