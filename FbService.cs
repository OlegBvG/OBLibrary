using System.Collections.Concurrent;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace OBLibrary;

public class FbService
{
    //для возвращающих запросов в БД FB

    public static BlockingCollection<(string, Guid)> QSqlForExecute = new BlockingCollection<(string, Guid)>();

    //для возвращающих запросов в БД Connect
    public static BlockingCollection<(string, Guid, string)> QSqlForExecuteAndGetConn = new();

    //для возвращающих запросов в БД SGS
    public static BlockingCollection<(string, Guid, string)> QSqlForExecuteAndGetSgs = new();

    //для возвращающих запросов в БД TMR
    public static BlockingCollection<(string, Guid, string)> QSqlForExecuteAndGetTmr = new();

    public static ConcurrentDictionary<Guid, Object> dictGuidSqlExecutedAndGet = new();

    public static List<Guid> listGuidSqlExecuted = new List<Guid>();
    static readonly Lock lockerListGuid = new();


    static TimeSpan durationWaitFromDb = TimeSpan.FromSeconds(60); // 60 seconds
    static int delayCycleWaitDb = 50;

    //***************************** Для работы с FireBird ******************
    public static void AddListGuid(Guid guid)
    {
        lock (lockerListGuid)
        {
            listGuidSqlExecuted.Add(guid);
        }
    }

    public static bool DelListGuid(Guid guid)
    {
        lock (lockerListGuid)
        {
            return listGuidSqlExecuted.Remove(guid);
        }
    }


    public static void AddDictGuidSqlExecutedAndGet(Guid guid, Object ob)
    {
        dictGuidSqlExecutedAndGet.TryAdd(guid, ob);
    }

    public static (bool, Object?) GetResultGuidSqlExecutedAndGet(Guid guid)
    {
        var isGet = dictGuidSqlExecutedAndGet.TryRemove(guid, out var result);
        return (isGet, result);
    }


    //*****************************************************************************

    public static async Task<DataTable>
        ExecuteSqlGetDT(string sql, CancellationToken cToken,
            string dbName = "conn") //переписано для уменьшения нагрузки на процессор, убрать коннекты
    {
        try
        {
            Guid g = Guid.NewGuid();

            switch (dbName)
            {
                case "conn":
                    QSqlForExecuteAndGetConn.Add((sql, g, "dt"));
                    break;
                case "sgs":
                    QSqlForExecuteAndGetSgs.Add((sql, g, "dt"));
                    ;
                    break;
                case "tmr":
                    QSqlForExecuteAndGetTmr.Add((sql, g, "dt"));
                    ;
                    break;
            }

            //*******************************

            (bool, object?) isGet;

            using (var cts = new CancellationTokenSource(durationWaitFromDb))
            {
                do
                {
                    // var delayTask = Task.Delay ( delayCycleWaitDb, InitClass.stoppingToken );
                    var delayTask = Task.Delay(delayCycleWaitDb, cToken);
                    delayTask.Wait();

                    isGet = GetResultGuidSqlExecutedAndGet(g);
                } while (!isGet.Item1 && !cts.IsCancellationRequested && !cToken.IsCancellationRequested);
            }
            //Log.FLog ( $"ExecuteSqlGetDT, GUID - {g}, Вышли" );

            if (isGet.Item2 is null)
            {
                DataTable dt = new();
                return dt;
            }

            return (DataTable)isGet.Item2;
        }
        catch (Exception ex)
        {
            // Log.ILog ( $" Строка запроса - {sql}" );
            // Log.ELog ( ex );


            throw;
        }
    }


    /// <summary>
    /// Возвращает значение левого верхнего столбца в запрошенном типе
    /// </summary>
    /// <typeparam name="T">Тип</typeparam>
    /// <param name="sql">Запрос</param>
    /// <param name="dbName">Наименование базы: sgs/tmr/conn </param>
    /// <returns></returns>
    public static async Task<T>
        ExecuteSqlGetOne<T>(string sql, CancellationToken cToken,
            string dbName = "conn") //переписано для уменьшения нагрузки на процессор, убрать коннекты
    {
        try
        {
            Guid g = Guid.NewGuid();

            switch (dbName)
            {
                case "conn":
                    QSqlForExecuteAndGetConn.Add((sql, g, "one"));
                    break;
                case "sgs":
                    QSqlForExecuteAndGetSgs.Add((sql, g, "one"));
                    break;
                case "tmr":
                    //strConnectDB = FirstSettings.TmrString;
                    QSqlForExecuteAndGetTmr.Add((sql, g, "one"));
                    break;
            }


            //*******************************


            (bool, object?) isGet;

            using var cts = new CancellationTokenSource(durationWaitFromDb);
            do
            {
                var delayTask = Task.Delay(delayCycleWaitDb, cToken);
                delayTask.Wait(cToken);

                isGet = GetResultGuidSqlExecutedAndGet(g);
            } while (!isGet.Item1 && !cts.IsCancellationRequested && !cToken.IsCancellationRequested);


            var noll = default(T);

            if (isGet.Item1)
            {
                // Log.DLog ( $"Результат запроса {g} получен - {isGet.Item2}" );
            }
            else
            {
                // Log.WLog ( $"Результат запроса {g} НЕ получен." );
                return noll;
            }

            if (isGet.Item2 is null && (typeof(System.Int32) is T) || (typeof(System.Int64) is T))
            {
                return (T)(isGet.Item2 ?? noll);
            }

            if (isGet.Item2 is null && typeof(System.String) is T)
            {
                return (T)(isGet.Item2 ?? "");
            }

            // Log.DLog ( $"var t - type - {isGet.Item2?.GetType ( )}; value - {isGet.Item2}" );

            if (isGet.Item2?.GetType() == typeof(System.Int32))
            {
                var intValue = (T)Convert.ChangeType(isGet.Item2, typeof(Int64));
                return intValue;
            }


            return (T)(isGet.Item2 ?? noll);
        }
        catch (Exception ex)
        {
            // Log.ILog ( $" Строка запроса - {sql}" );
            // Log.ELog ( ex );


            throw;
        }
    }


    public static async Task<DataSet>
        ExecuteSqlGetDs(string sql, CancellationToken cToken,
            string dbName = "conn") //переписано для уменьшения нагрузки на процессор, убрать коннекты
    {
        try
        {
            BlockingCollection<(string, Guid, string)> QSqlForExecuteAndGet = QSqlForExecuteAndGetConn;

            Guid g = Guid.NewGuid();

            switch (dbName)
            {
                case "conn":
                    QSqlForExecuteAndGetConn.Add((sql, g, "ds"));
                    
                    break;
                case "sgs":
                    QSqlForExecuteAndGetSgs.Add((sql, g, "ds"));
                    
                    break;
                case "tmr":
                    QSqlForExecuteAndGetTmr.Add((sql, g, "ds"));
                    
                    break;
            }


            //*******************************


            (bool, object?) isGet;

            using var cts = new CancellationTokenSource(durationWaitFromDb);

            do
            {
                var delayTask = Task.Delay(delayCycleWaitDb, cToken);
                delayTask.Wait();

                isGet = GetResultGuidSqlExecutedAndGet(g);
            } while (!isGet.Item1 && !cts.IsCancellationRequested && !cToken.IsCancellationRequested);

            if (isGet.Item2 is null)
            {
                DataSet ds = new();
                return ds;
            }

            return (DataSet)isGet.Item2;
        }
        catch (Exception ex)
        {
            // Log.ILog ( $" Строка запроса - {sql}" );
            // Log.ELog ( ex );


            throw;
        }
    }
}