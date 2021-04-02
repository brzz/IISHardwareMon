using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class ShowData : System.Web.UI.Page
{
    public string dropliststr;
    protected void Page_Load(object sender, EventArgs e)
    {
        string sql0 = @"SELECT type ,SesonName, HardwareName
FROM HardwareInfo 
GROUP BY SesonName, type
ORDER BY TYPE ASC";
        DataTable dt0 = DBABase.QueryDBA(sql0);
        dropliststr = JsonConvert.SerializeObject(dt0);


        string sql = @"SELECT DISTINCT type FROM HardwareInfo";

        DataTable dt = DBABase.QueryDBA(sql);

        this.DropDownList1.DataSource = dt;
        this.DropDownList1.DataTextField = "type";
        this.DropDownList1.DataValueField = "type";
        this.DropDownList1.DataBind();


        string sql2 = @"SELECT DISTINCT SesonName FROM HardwareInfo where type='" + dt.Rows[0][0] + "'";

        DataTable dt2 = DBABase.QueryDBA(sql2);

        this.DropDownList2.DataSource = dt2;
        this.DropDownList2.DataTextField = "SesonName";
        this.DropDownList2.DataValueField = "SesonName";
        this.DropDownList2.DataBind();

    }

    #region 数据获取及处理
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, XmlSerializeString = false)]
    public static preJson<TableRes> GetData(int getCount, string Type, string Name)
    {
        try
        {
            //Temperature
            //CPU Package
            string sql = @"select * from 
(SELECT t1.ID, t2.Identifier, t2.SesonName, t2.HardwareName, t2.type, t1.Value, t1.InsertTime FROM 'SesonsLogs' t1, 'HardwareInfo' t2 
where t1.HardwareInfoID = t2.ID
and t2.type = @TYPE and t2.SesonName = @NAME
order by t1.InsertTime desc
LIMIT 0, @getCount ) 
order by InsertTime asc";

            List<SQLiteParameter> ps = new List<SQLiteParameter>();
            ps.Add(new SQLiteParameter("getCount", getCount));
            ps.Add(new SQLiteParameter("TYPE", Type));
            ps.Add(new SQLiteParameter("NAME", Name));


            DataTable dt = DBABase.QueryDBA(sql, ps);

            preJson<TableRes> r = new preJson<TableRes>(dt);
            r.DataLenght = (UInt64)dt.Rows.Count;
            r.StartTime = ConvertDef.GetTime(dt.Rows[0]["InsertTime"].ToString());



            string minandmax = ConfigurationSettings.AppSettings[Type + "+" + "*"];
			if(!String.IsNullOrWhiteSpace(minandmax))
			{
				string[] sArray=minandmax.Split(',') ;
				r.ShowMIN = Convert.ToSingle(sArray[0]);
				r.ShowMAX = Convert.ToSingle(sArray[1]);
				r.ShowUnit = sArray[2];
			}
			
			minandmax = ConfigurationSettings.AppSettings[Type + "+" + Name];
			if(!String.IsNullOrWhiteSpace(minandmax))
			{
				string[] sArray=minandmax.Split(',') ;
				r.ShowMIN = Convert.ToSingle(sArray[0]);
				r.ShowMAX = Convert.ToSingle(sArray[1]);
				r.ShowUnit = sArray[2];
			}
			 
            return r;
        }
        catch { return null; }
    }

    public class TableRes
    {
        public float Value { get; set; }
        public Int64 InsertTime { get; set; }
        public string HardwareName { get; set; }
		public string Identifier {get; set;}
    }

    #endregion
}