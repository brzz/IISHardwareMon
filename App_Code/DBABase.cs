using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

/// <summary>
/// DBABase 的摘要说明
/// </summary>
public class DBABase
{
    public static readonly string path = System.AppDomain.CurrentDomain.BaseDirectory + @"DBA.sqlite";
    public static SQLiteConnection DBA = null;

    public DBABase()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static void Start()
    {
        DBA = new SQLiteConnection("data source=" + path);
        DBA.Open();
    }

    public static bool VerfiyDBA(string tablename)
    {
        SQLiteCommand cmd = DBA.CreateCommand();

        cmd.CommandText = "PRAGMA table_info('" + tablename + "')";

        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
        DataTable table = new DataTable();
        adapter.Fill(table);

        if (table.Rows.Count == 0) return false;
        return true;
    }

    public static void ExeNoQuery(string SQL)
    {
        SQLiteCommand cmd = new SQLiteCommand();
        cmd.Connection = DBA;
        cmd.CommandText = SQL;
        cmd.ExecuteNonQuery();
    }

    public static void InitDBATable1()
    {
        //建立饭店表
        //        string sql =
        //@"CREATE TABLE FanDian(ID INTEGER PRIMARY KEY  AUTOINCREMENT,
        //MC TEXT,fen INTEGER,price REAL, others TEXT, 
        //count INTEGER default(0), denycount INTEGER default(0), gocount INTEGER default(0))";
        //        ExeNoQuery(sql);

        string sql = @"CREATE TABLE SesonsLogs(ID INTEGER PRIMARY KEY AUTOINCREMENT,
HardwareInfoID INTEGER, Value REAL, InsertTime INTEGER
)";

        ExeNoQuery(sql);

    }
	
	public static void InitDBATable2()
	{
		string sql = @"CREATE TABLE HardwareInfo (
ID INTEGER PRIMARY KEY AUTOINCREMENT, Identifier TEXT, SesonName TEXT, type TEXT, HardwareName TEXT, 
UNITSHOW TEXT, MAXSHOW REAL, MINSHOW REAL, ONOFF INTEGER
);";
		ExeNoQuery(sql);
	}

	
	public static void InitDBA()
	{
		try{
			InitDBATable1();
		}
		catch{}
		
		try{
			InitDBATable2();
		}
		catch{}
	}


    public static int QueryHardwareID(string SesonName, string HardwareName, string type, string Identifier)
    {
        string checkhardtype = @"select ID from HardwareInfo where
		SesonName = @SesonName and HardwareName = @HardwareName and type = @type
and Identifier = @Identifier
		";
        SQLiteCommand cmd1 = new SQLiteCommand(checkhardtype, DBA);

        cmd1.Parameters.Add(new SQLiteParameter("@SesonName", DbType.String));
        cmd1.Parameters.Add(new SQLiteParameter("@HardwareName", DbType.String));
        cmd1.Parameters.Add(new SQLiteParameter("@type", DbType.String));
        cmd1.Parameters.Add(new SQLiteParameter("@Identifier", DbType.String));

        cmd1.Parameters[0].Value = SesonName;
        cmd1.Parameters[1].Value = HardwareName;
        cmd1.Parameters[2].Value = type;
        cmd1.Parameters[3].Value = Identifier;

        var res = cmd1.ExecuteReader();
        if (!res.HasRows) return -1;

        res.Read();
        return res.GetInt32(0);
    }
    public static void InsertSensons(OpenHardwareMonitor.Hardware.ISensor s, DateTime dt)
    {
        if (s.Value == null) return;
        int id = QueryHardwareID(s.Name, s.Hardware.Name, s.SensorType.ToString(), s.Identifier.ToString());

        if (id < 0)
		{
			string inshardwaretype = @"insert into HardwareInfo 
(ID,SesonName,HardwareName,type,Identifier) 
values 
(null,@SesonName,@HardwareName,@type,@Identifier)";
            
			SQLiteCommand cmd2 = new SQLiteCommand(inshardwaretype, DBA);
            cmd2.Parameters.Add(new SQLiteParameter("@SesonName", DbType.String));
            cmd2.Parameters.Add(new SQLiteParameter("@HardwareName", DbType.String));
            cmd2.Parameters.Add(new SQLiteParameter("@type", DbType.String));
            cmd2.Parameters.Add(new SQLiteParameter("@Identifier", DbType.String));

            cmd2.Parameters[0].Value = s.Name;
            cmd2.Parameters[1].Value = s.Hardware.Name;
            cmd2.Parameters[2].Value = s.SensorType;
            cmd2.Parameters[3].Value = s.Identifier.ToString();

            cmd2.ExecuteNonQuery();

            id = QueryHardwareID(s.Name, s.Hardware.Name, s.SensorType.ToString(), s.Identifier.ToString());
        }
		
		

        string strInsertSql = @"insert into SesonsLogs 
(ID,HardwareInfoID,Value,InsertTime) 
values 
(null,@HardwareInfoID,@Value,@InsertTime)";
        SQLiteCommand command = new SQLiteCommand(strInsertSql, DBA);
        command.Parameters.Add(new SQLiteParameter("@HardwareInfoID", DbType.Int32));
        
        command.Parameters.Add(new SQLiteParameter("@Value", DbType.Single));
        command.Parameters.Add(new SQLiteParameter("@InsertTime", DbType.Int64));
		//command.Parameters.Add(new SQLiteParameter("@SesonName", DbType.String));
		//command.Parameters.Add(new SQLiteParameter("@HardwareName", DbType.String));
        //command.Parameters.Add(new SQLiteParameter("@type", DbType.String));
		
        command.Parameters[0].Value = id;
        command.Parameters[1].Value = s.Value == null ? float.NaN: Math.Round((float)s.Value, 2) ;
        command.Parameters[2].Value = ConvertDef.ConvertDateTimeInt(dt);
		//command.Parameters[1].Value = s.Name;
		//command.Parameters[3].Value = s.Hardware.Name;
        //command.Parameters[4].Value = s.SensorType;


        command.ExecuteNonQuery();
    }

    public static DataTable QueryDBA(string sql)
    {
        DataTable TSelectReSoult = new DataTable();

        SQLiteCommand cmd = new SQLiteCommand();
        cmd.Connection = DBA;
        cmd.CommandText = sql;
        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            TSelectReSoult.Load(reader);
            /*就这一句用DataTable.Load(DataReader)来把查询到的数据插入到DataTable中*/
        }

        return TSelectReSoult;
    }

    public static DataTable QueryDBA(string sql, List<SQLiteParameter> parameter)
    {
        DataTable TSelectReSoult = new DataTable();

        SQLiteCommand cmd = new SQLiteCommand();
        cmd.Connection = DBA;
        cmd.CommandText = sql;

        cmd.Parameters.AddRange(parameter.ToArray());

        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            TSelectReSoult.Load(reader);
            /*就这一句用DataTable.Load(DataReader)来把查询到的数据插入到DataTable中*/
        }

        return TSelectReSoult;
    }
}