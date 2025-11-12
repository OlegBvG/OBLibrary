
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using FirebirdSql.Data.FirebirdClient;
using Npgsql;

namespace OBLibrary;

public class ConfigOptions
{
    // [MaxLength(5)]
    //
    // public int GFakePeriod
    // {
    //     get => TcpConnectClient.FakePeriod;
    // }

    // private TcpConnectClient _tcpConnectClient;
    // private TcpConnectServer _tcpConnectServer;

    // public TcpConnectClient TcpConnectClient { get => _tcpConnectClient; set => _tcpConnectClient = value; }
    // public TcpConnectServer TcpConnectServer { get => _tcpConnectServer; set=> _tcpConnectServer = value; }

    public TcpConnectClient TcpConnectClient { get; set; }

    public TcpConnectServer TcpConnectServer { get; set; }
    public TcpTmrServer TcpTmrServer { get; set; }
    public Directories Directories { get; set; }
    public DatabaseConnection DatabaseConnection { get; set; }
    public Dbver Dbver { get; set; }
    public TcpArcRecordReading TcpArcRecordReading { get; set; }
    public string Lkg => lkg();

    private string lkg()
    {
        var curentD = AppContext.BaseDirectory;
        var curentD2 = Directory.GetParent ( curentD )?.ToString ( ) ?? "";
        var parentD = Directory.GetParent ( curentD2 )?.ToString ( ) ?? "";
        var itsDir = Path.Combine(parentD, "SGS_ITS");
        var pathLicFile = Path.Combine(itsDir, "sgs.lic"); //todo - вернуть!!!!!!!!!!!!!!!!
        // var pathLicFile = Path.Combine(@"C:\CWork\ConnectServiceN\TmrTcpServicePg\bin\Debug\SGS_ITS\", "sgs.lic");

        return CryptUtils.GetLKG(pathLicFile);
    }
}

public class TcpConnectClient
{
    private const int TimeWorkProxyLocDefault = 120;
    private const int TimeWaitAnswearDefault = 90;
    private const int RefreshPeriodDefault = 2;

    [MaxLength(5)] private string _logLevel = "error";
    private string _serverIpAddr = "127.0.0.1";
    private int _clientPort = 22222;
    private int _servicePort = 22223;
    private string _grpcOn = "on";
    private string _portLog = "off";
    private int _timeWorkProxyLoc = TimeWorkProxyLocDefault;
    private int _timeWaitAnswear = TimeWaitAnswearDefault;
    private int _refreshPeriod = RefreshPeriodDefault;


    public bool IsGrpcOn => GrpcOn.Equals("on", StringComparison.InvariantCultureIgnoreCase);

    // [MinLength(15)]
    public string LogLevel
    {
        get => _logLevel;
        set => _logLevel = value;
    }

    public string ServerIpAddr
    {
        get => _serverIpAddr;
        set => _serverIpAddr = value;
    }
    

    public int ClientPort
    {
        get => _clientPort;
        set => _clientPort = value;
    }

    public int ServicePort
    {
        get => _servicePort;
        set => _servicePort = value;
    }

    public string GrpcOn
    {
        get => _grpcOn;
        set => _grpcOn = value;
    }

    public string PortLog
    {
        get => _portLog;
        set => _portLog = value;
    }

    public int TimeWorkProxyLoc
    {
        get => _timeWorkProxyLoc * 1000;
        set { _timeWorkProxyLoc = value is >= 10 and <= 240 ? value : TimeWorkProxyLocDefault; }
    }

    public int TimeWaitAnswear
    {
        get => _timeWaitAnswear * 1000;
        set { _timeWaitAnswear = value is >= 10 and <= 120 ? value : TimeWaitAnswearDefault; }
    }

    public int RefreshPeriod
    {
        get => _refreshPeriod * 1000;
        set { _refreshPeriod = value is >= 1 and <= 60 ? value : RefreshPeriodDefault; }
    }
}

public class TcpConnectServer
{
    private const int TimeWorkProxyServDefault = 120;
    private const int TimeAnswTmrDefault = 90;
    private const int HolePeriodDefault = 1;
    private const int HoleBpekPeriodDefault = 30;

    // #порт для подключения приборов
// #DevicePort=40000
    private int _devicePort = 40000;

// #порт для подключения пользователей
// #ClientPort=22222
    private int _clientPort = 22222;

// #ServicePort=54334
    private int _servicePort = 22223;
    private string _logLevel = "full";

    private string _portLog = "on";

// #portLog=on
    private int _holePeriod = HolePeriodDefault;
    private int _holeBpekPeriod = HoleBpekPeriodDefault;
    private int _timeWorkProxyServ = TimeWorkProxyServDefault;
    private int _timeAnswTmr = TimeAnswTmrDefault;
    private string _grpcOn = "on";

    private bool _isTestMode = false;
    private bool _isSgsManage = false;
    [Url] private string _grpcAddress = "https://localhost:7062";


    public int DevicePort
    {
        get => _devicePort;
        set => _devicePort = value;
    }

    public int ClientPort
    {
        get => _clientPort;
        set => _clientPort = value;
    }

    public int ServicePort
    {
        get => _servicePort;
        set => _servicePort = value;
    }

    public string LogLevel
    {
        get => _logLevel;
        set => _logLevel = value;
    }

    public string PortLog
    {
        get => _portLog;
        set => _portLog = value;
    }

    public int HolePeriod
    {
        get => _holePeriod;
        set { _holePeriod = value is >= 1 and <= 5 ? value : HolePeriodDefault; }
    }

    public int HoleBpekPeriod
    {
        get => _holeBpekPeriod;
        set { _holeBpekPeriod = value is >= 10 and <= 90 ? value : HoleBpekPeriodDefault; }
    }

    public int TimeWorkProxyServ
    {
        get => _timeWorkProxyServ * 1000;
        set { _timeWorkProxyServ = value is >= 10 and <= 240 ? value : TimeWorkProxyServDefault; }
    }

    public int TimeAnswTmr
    {
        get => _timeAnswTmr * 1000;
        set { _timeAnswTmr = value is >= 10 and <= 120 ? value : TimeAnswTmrDefault; }
    }

    public string GrpcOn
    {
        get => _grpcOn;
        set => _grpcOn = value;
    }

    public bool IsGrpcOn => GrpcOn.Equals("on", StringComparison.InvariantCultureIgnoreCase);

    public bool IsTestMode
    {
        get => _isTestMode;
        set => _isTestMode = value;
    }

    public bool IsSgsManage
    {
        get => _isSgsManage;
        set => _isSgsManage = value;
    }

    public string GrpcAddress
    {
        get => _grpcAddress;
        set => _grpcAddress = value;
    }
}

public class TcpTmrServer
{
    private const int TimeAnswTmrDefault = 90;

    // #порт для подключения приборов
    private int _timeAnswTmr = TimeAnswTmrDefault;
    private string _logLevel = "full";
    private int _port = 54319;

    private int _closeLock = 1;
    private int _capacity = 1;
    private int _timeSynchro = 0;
    private int _changeArc = 0;

    public int TimeAnswTmr
    {
        get => _timeAnswTmr * 1000;
        set { _timeAnswTmr = value is >= 10 and <= 120 ? value : TimeAnswTmrDefault; }
    }

    public string LogLevel
    {
        get => _logLevel;
        set => _logLevel = value;
    }

    public int Port
    {
        get => _port;
        set => _port = value;
    }


    public int CloseLock    // 1 - закрывать замок; 0 - не закрывать замок
    {
        get => _closeLock is 0 or 1 ? _closeLock : 1;
        set => _closeLock = value;
    }

    public int Capacity // 1 - закрывать замок; 0 - не закрывать замок
    {
        get => _capacity is 0 or 1 ? _capacity : 1;
        set => _capacity = value;
    }

    public int TimeSynchro      // 1 - синхронизировать время; 0 - не синхронизировать время
    {
        get => _timeSynchro is 0 or 1 ? _timeSynchro : 1;
        set => _timeSynchro = value;
    }

    public int ChangeArc
    {
        get => _changeArc is 0 or 1 ? _changeArc : 1;
        set => _changeArc = value;
    }
}

public class Directories
{
    private static string baseDirectory = AppContext.BaseDirectory;

    private string _arcDir = Path.Combine(baseDirectory, "ArcDir");
    // private string _arcDir2 = "";   //Path.Combine(baseDirectory, "ArcDir2");
    private string _updateDir = Path.Combine(baseDirectory, "UpdateDir");
    private string _tmrArcDir = "";
    private string _logDir = Path.Combine(baseDirectory, "LogDir");
    private string _templateDir = Path.Combine(baseDirectory, "TemplateDir");
    private string _helpDir = "";
    private string _aupDir = "";
    private string _sgs_ModemIniFile = "";
    private string _tcpTmrArcDir = Path.Combine(baseDirectory, "TCPTmrArc");
    private string _tcpArcDir = Path.Combine(baseDirectory, "TCPArcDir");
    private string _fastDir = "";
    private string _updateSMTDir = "";

    public  string BaseDirectory { get => baseDirectory; set => baseDirectory =  value ; }
    public string ArcDir
    {
        get => _arcDir;
        set => _arcDir = value;
    }
    // public string ArcDir2
    // {
    //     get => _arcDir2;
    //     set => _arcDir2 = value;
    // }
    
    // public bool IsArcDir2
    // {
    //     get => !String.IsNullOrWhiteSpace(_arcDir2);
    //    
    // }
    public string UpdateDir
    {
        get => _updateDir;
        set => _updateDir = value;
    }
    
    public string UpdateSMTDir
    {
        get => _updateSMTDir;
        set => _updateSMTDir = value;
    }

    public string TmrArcDir
    {
        get => _tmrArcDir;
        set => _tmrArcDir = value;
    }

    public string LogDir
    {
        get => _logDir;
        set => _logDir = value;
    }

    public string TemplateDir
    {
        get => _templateDir;
        set => _templateDir = value;
    }

    public string HelpDir
    {
        get => _helpDir;
        set => _helpDir = value;
    }

    public string AupDir
    {
        get => _aupDir;
        set => _aupDir = value;
    }

    public string Sgs_ModemIniFile
    {
        get => _sgs_ModemIniFile;
        set => _sgs_ModemIniFile = value;
    }

    public string TcpTmrArcDir
    {
        get => _tcpTmrArcDir;
        set => _tcpTmrArcDir = value;
    }

    public string TcpArcDir
    {
        get => _tcpArcDir;
        set => _tcpArcDir = value;
    }

    public string FastDir
    {
        get => _fastDir;
        set => _fastDir = value;
    }

    public void CheckAndCreateDir()
    {
        Type directoriesType = this.GetType();
        PropertyInfo[] properties = directoriesType.GetProperties();
        // Loop through each property
        foreach (PropertyInfo property in properties)
        {
            // Get the property name
            string propertyName = property.Name;

            // Get the property value
            object propertyValue = property.GetValue(this);

            // Display the property name and value
            // if (propertyName is "ArcDir" or "ArcDir2" or "UpdateDir" or "LogDir" or "TemplateDir" or "HelpDir" or "AupDir" or "TCPTmrArcDir" or "TCPArcDir")
            if (propertyName is "ArcDir" or "ArcDir2" or "LogDir" or "AupDir" or "TcpTmrArcDir" or "TcpArcDir")
            {
                if (propertyValue is string dir && !string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            // Console.WriteLine($"---------- {propertyName}: {propertyValue}");
            // Console.WriteLine($"{propertyName}: {propertyValue}");
        }
    }
}

public class DatabaseConnection
{
// #DatabaseType=Firebird
    private string _databaseType = "PostgreSQL";

    private string _serverSgsPgDatabaseName = "sgs";

// #ServerSgsPgDatabaseIP=localhost
    private string _serverSgsPgDatabaseIP = "127.0.0.1";
    private int _serverSgsPgDatabasePort = 5432;
    private string _serverSgsPgDatabaseLogin = "GazSetLogin";
    private string _serverSgsPgDatabasePassword = "masterGazSetLogin";
    private NpgsqlConnection _npgsqlConnection;

// #имя пользователя БД
// #ConnectLogin=SYSDBA
// #пароль пользователя БД
// #ConnectPassword=Pg4XEQ4qJDcS
// #путь к БД
    private string _connectDatabase = @"127.0.0.1/3050:C:\Connect\DB\Connect.fdb";


// #Имя пользователя БД
    private string _connectLogin = "SYSDBA";

// #Пароль пользователя БД
    private string _connectPassword = "Pg4XEQ4qJDcS";

// #Путь к БД
// #ConnectDatabase=C:\SGS-Server\DB\CONNECT.FDB
// #ConnectDatabase=C:\\Connect\\DB\\Connect.fdb
// #ConnectDataSource=127.0.0.1
// #ConnectPort=3050
    private string _connectCharset = "UTF8";

    private int _connectDialect = 3;
    private int _connectConnectionLifeTime = 15;
    private bool _connectPooling = true;
    private int _connectMinPoolSize = 0;

    private int _connectMaxPoolSize = 50;
    private int _connectPacketSize = 8192;

// #Имя пользователя БД SGS для программы Connect
// #ConnectLoginSGS=SYSDBA
// #Пароль пользователя БД SGS
// #ConnectPasswordSGS=Pg4XEQ4qJDcS
// #Путь к БД
// #ConnectDatabase=C:\SGS-Server\DB\CONNECT.FDB
// #ConnectDatabaseSGS=C:\SGS-Server\DB\sgs.fdb
// #ConnectDataSourceSGS=127.0.0.1
// #ConnectPortSGS=3050
// #######################

    private string _serverDatabase_SRV = @"C:\SGS-Server\DB\sgs_srv.fdb";
    private string _serverDatabase = @"127.0.0.1/3050:C:\SGS-Server\DB\sgs.fdb";
    private string _serverLogin = "SYSDBA";

    private string _serverPassword = "Pg4XEQ4qJDcS";

// #ServerPassword=IFkjA188DiJSUzQ0W1A=
    private string _tmrDatabase = @"127.0.0.1/3050:C:\SGS-Server\DB\tmr.fdb";
    private string _tmrLogin = "SYSDBA";

    private string _tmrPassword = "Pg4XEQ4qJDcS";
    private int _connectPort = 3050;
    private int _connectPortSgs = 3050;
    private int _connectPortTmr = 3050;
    //******************


    public string DatabaseType
    {
        get => _databaseType;
        set => _databaseType = value;
    }

    public string ServerSgsPgDatabaseName
    {
        get => _serverSgsPgDatabaseName;
        set => _serverSgsPgDatabaseName = value;
    }

    public string ServerSgsPgDatabaseIP
    {
        get => _serverSgsPgDatabaseIP;
        set => _serverSgsPgDatabaseIP = value;
    }

    public int ServerSgsPgDatabasePort
    {
        get => _serverSgsPgDatabasePort;
        set => _serverSgsPgDatabasePort = value;
    }

    public string ServerSgsPgDatabaseLogin
    {
        get => _serverSgsPgDatabaseLogin;
        set => _serverSgsPgDatabaseLogin = value;
    }

    public string ServerSgsPgDatabasePassword
    {
        get => _serverSgsPgDatabasePassword;
        set => _serverSgsPgDatabasePassword = value;
    }

    public string ConnectDatabase
    {
        get => _connectDatabase;
        set => _connectDatabase = value;
    }

    public string ConnectLogin
    {
        get => _connectLogin;
        set => _connectLogin = value;
    }

    public string ConnectPassword
    {
        // get => _connectPassword;
        get => CryptUtils.DecryptSODEK_Energy(_connectPassword, "SodekXORkey");

        set => _connectPassword = value;
    }

    public string ConnectCharset
    {
        get => _connectCharset;
        set => _connectCharset = value;
    }

    public int ConnectDialect
    {
        get => _connectDialect;
        set => _connectDialect = value;
    }

    public int ConnectConnectionLifeTime
    {
        get => _connectConnectionLifeTime;
        set => _connectConnectionLifeTime = value;
    }

    public bool ConnectPooling
    {
        get => _connectPooling;
        set => _connectPooling = value;
    }

    public int ConnectMinPoolSize
    {
        get => _connectMinPoolSize;
        set => _connectMinPoolSize = value;
    }

    public int ConnectMaxPoolSize
    {
        get => _connectMaxPoolSize;
        set => _connectMaxPoolSize = value;
    }

    public int ConnectPacketSize
    {
        get => _connectPacketSize;
        set => _connectPacketSize = value;
    }

    public string ServerDatabase_SRV
    {
        get => _serverDatabase_SRV;
        set => _serverDatabase_SRV = value;
    }

    public string ServerDatabase
    {
        get => _serverDatabase;
        set => _serverDatabase = value;
    }

    public string ServerLogin
    {
        get => _serverLogin;
        set => _serverLogin = value;
    }

    public string ServerPassword
    {
        // get => _serverPassword;
        get => CryptUtils.DecryptSODEK_Energy(_serverPassword, "SodekXORkey");

        set => _serverPassword = value;
    }

    public string TmrDatabase
    {
        get => _tmrDatabase;
        set => _tmrDatabase = value;
    }

    public string TmrLogin
    {
        get => _tmrLogin;
        set => _tmrLogin = value;
    }

    public string TmrPassword
    {
        // get => _tmrPassword;
        get => CryptUtils.DecryptSODEK_Energy(_tmrPassword, "SodekXORkey");
        set => _tmrPassword = value;
    }

    public string ConnectionStringPg =>
        @$"Host={ServerSgsPgDatabaseIP};Port={_serverSgsPgDatabasePort};Username={ServerSgsPgDatabaseLogin};Password={ServerSgsPgDatabasePassword};Database={ServerSgsPgDatabaseName};Maximum Pool Size=100";
    // dataSourcePg = NpgsqlDataSource.Create ( connectionStringPg );

    public NpgsqlDataSource NpgsqlDataSource => NpgsqlDataSource.Create(ConnectionStringPg);
    
    public NpgsqlConnection NpgsqlConnection
    {
        get
        {
            if (_npgsqlConnection != null)
                if (_npgsqlConnection.State is not (ConnectionState.Broken or ConnectionState.Closed))
                    return _npgsqlConnection;

            // if (_npgsqlConnection is { State: not (ConnectionState.Broken or ConnectionState.Closed) }) return _npgsqlConnection;

            
            _npgsqlConnection = new NpgsqlConnection(ConnectionStringPg);
            _npgsqlConnection.Open();

            return _npgsqlConnection;
        }
    }


    public bool IsPgDb
    {
        get => DatabaseType is "PostgreSQL"; //является ли используемая БД - PostgreSQL (для сокращения ввода в тексте программы)
    }

    public string? ConnectDataSource { get; set; }

    public int ConnectPort
    {
        get => _connectPort;
        set => _connectPort = value;
    }
    
    public string? ConnectDataSourceSgs { get; set; }
    public int ConnectPortSgs
    {
        get => _connectPortSgs;
        set => _connectPortSgs = value;
    }
    
    public string? ConnectDataSourceTmr { get; set; }
    public int ConnectPortTmr
    {
        get => _connectPortTmr;
        set => _connectPortTmr = value;
    }
    public string ConnectString
    {
        get => _connectString();
    }

    private string _connectString()
    {
        FbConnectionStringBuilder cs = new();
        // ConnectDatabase=127.0.0.1/3050:C:\Connect\DB\Connect.fdb
        cs.UserID = ConnectLogin;
        cs.Password = ConnectPassword;
        cs.Database = ConnectDatabase;

        if (ConnectDataSource != null)
            cs.DataSource = ConnectDataSource;

        
        cs.Port = ConnectPort;
        //cs.Database = "127.0.0.1/3050:C:\\Connect\\DB\\Connect.fdb";

        cs.Charset = ConnectCharset;
        cs.Dialect = ConnectDialect;
        cs.ConnectionLifeTime = ConnectConnectionLifeTime;
        cs.MinPoolSize = ConnectMinPoolSize;
        cs.MaxPoolSize = ConnectMaxPoolSize;
        cs.PacketSize = ConnectPacketSize;


        cs.Role = "";
        cs.Pooling = ConnectPooling;
        cs.ServerType = FbServerType.Default;

        var connString = cs.ToString();
        return connString;
    }
    
    public string SgsString
    {
        get => _sgsString();
    }

    private string _sgsString()
    {
        FbConnectionStringBuilder cs = new();
        // ConnectDatabase=127.0.0.1/3050:C:\Connect\DB\Connect.fdb
        cs.UserID = ServerLogin;
        cs.Password = ServerPassword;
        cs.Database = ServerDatabase;

        if (ConnectDataSourceSgs != null)
            cs.DataSource = ConnectDataSourceSgs;

        
        cs.Port = ConnectPortSgs;
        //cs.Database = "127.0.0.1/3050:C:\\Connect\\DB\\Connect.fdb";

        cs.Charset = ConnectCharset;
        cs.Dialect = ConnectDialect;
        cs.ConnectionLifeTime = ConnectConnectionLifeTime;
        cs.MinPoolSize = ConnectMinPoolSize;
        cs.MaxPoolSize = ConnectMaxPoolSize;
        cs.PacketSize = ConnectPacketSize;


        cs.Role = "";
        cs.Pooling = ConnectPooling;
        cs.ServerType = FbServerType.Default;

        var sgsString = cs.ToString();
        return sgsString;
    }

    public string TmrString
    {
        get => _tmrString();
    }

    private string _tmrString()
    {
        FbConnectionStringBuilder cs = new();
        // ConnectDatabase=127.0.0.1/3050:C:\Connect\DB\Connect.fdb
        cs.UserID = TmrLogin;
        cs.Password = TmrPassword;
        cs.Database = TmrDatabase;

        if (ConnectDataSourceTmr != null)
            cs.DataSource = ConnectDataSourceTmr;

        
        cs.Port = ConnectPortTmr;
        //cs.Database = "127.0.0.1/3050:C:\\Connect\\DB\\Connect.fdb";

        cs.Charset = ConnectCharset;
        cs.Dialect = ConnectDialect;
        cs.ConnectionLifeTime = ConnectConnectionLifeTime;
        cs.MinPoolSize = ConnectMinPoolSize;
        cs.MaxPoolSize = ConnectMaxPoolSize;
        cs.PacketSize = ConnectPacketSize;


        cs.Role = "";
        cs.Pooling = ConnectPooling;
        cs.ServerType = FbServerType.Default;

        var tmrString = cs.ToString();
        return tmrString;
    }

    
}

public class Dbver
{
    public string NEW_SGS_DB_VERSION { get; set; }
    public string NEW_TMR_DB_VERSION { get; set; }
    public string NEW_SRV_DB_VERSION { get; set; }
    public string NEW_ALP_DB_VERSION { get; set; }
    public string NEW_CONNECT_DB_VERSION { get; set; }
   
    //для TCP Connect
    public string VersDbConnectPresent{ get; set; }
   
}

public class TcpArcRecordReading
{
    public int ComplexHourArc { get; set; } = 20;
    public int ComplexDayArc { get; set; } = 20;
    public int ComplexTelemetryArc { get; set; } = 20;
    public int ComplexChangeArc { get; set; } = 20;
    public int ComplexSystemArc { get; set; } = 20;
    public int SmartHourArc { get; set; } = 20;
    public int SmartDayArc { get; set; } = 20;
    public int SmartTelemetryArc { get; set; } = 20;
    public int SmartChangeArc { get; set; } = 20;
    public int SmartSystemArc { get; set; } = 20;
    public int NewSmartHourArc { get; set; } = 20;
    public int NewSmartDayArc { get; set; } = 20;
    public int NewSmartTelemetryArc { get; set; } = 20;
    public int NewSmartChangeArc { get; set; } = 20;
    public int NewSmartSystemArc { get; set; } = 20;
}