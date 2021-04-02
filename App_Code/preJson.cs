using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// preJson 的摘要说明
/// </summary>
public class preJson<T>
{
    public T[] rows;

    public DateTime StartTime;
    public UInt64 DataLenght;
	public float ShowMAX = 0;
	public float ShowMIN = 0;
	public string ShowUnit;

    public preJson(DataTable dt)
    {
        this.rows = this.rows = ConvertDef.TableToList<T>(dt).ToArray();
    }


}