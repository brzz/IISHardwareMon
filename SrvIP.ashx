<%@ WebHandler Language="C#" Class="SrvIP" %>

using System;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Management;

public class SrvIP : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        IPAddress ipa = null;
		Int32? list = null;
        try
        {
			if(context.Request.QueryString["ethid"] != null)
			{
                list = Int32.Parse(context.Request.QueryString["ethid"].ToString());
                IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
                ipa = ipe.AddressList[(int)list];
			}

            if(context.Request.QueryString["dns"] != null)
             {
                IPHostEntry ipe = Dns.GetHostEntry(context.Request.QueryString["dns"].ToString());
                ipa = list == null ? ipe.AddressList[1]:ipe.AddressList[(int)list] ;
            }

			if(ipa == null) return;
			
            context.Response.ContentType = "text/plain";
            string ip = ipa.ToString();
            context.Response.Write(ip.Split('%')[0]);

            /*

                        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection nics = mc.GetInstances();
                        foreach (ManagementObject nic in nics)
                        {
                            if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                            {
                                string mac = nic["MacAddress"].ToString();//Mac地址
                                string ip6 = (nic["IPAddress"] as String[])[0];//IP地址
                                string ipsubnet = (nic["IPSubnet"] as String[])[0];//子网掩码
                                string ipgateway = (nic["DefaultIPGateway"] as String[])[0];//默认网关
                            }
                        }
             */

        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("");
        }

    }

    public string GetGlobalIPv6()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return IpEntry.AddressList[i].ToString();
                }
            }
            return "";
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}