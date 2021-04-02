<%@ Application Language="C#" %>

<%@ Import Namespace="OpenHardwareMonitor.Hardware" %>
<%@ Import Namespace="System.Threading" %>



<script RunAt="server">

    private static OpenHardwareMonitor.Hardware.Computer computer;
    public static bool run = true;
    public static int CheckCycle = 60000;

    static void UpdateSubHardware(IHardware subHardware, DateTime dt)
    {
        if (subHardware == null) return;
        subHardware.Update();

        foreach(var s in subHardware.Sensors)
        {
            DBABase.InsertSensons(s, dt);
        }

        foreach(var h in subHardware.SubHardware)
        {
            UpdateSubHardware(h, dt);
        }
    }
    static void everytime()
    {
        while (run)
        {

            DateTime dt = DateTime.Now;
            foreach (var h in computer.Hardware)
            {
                UpdateSubHardware(h, dt);
            }

            Thread.Sleep(CheckCycle);
        }

        computer.Close();
    }


    void Application_Start(object sender, EventArgs e)
    {
        // 在应用程序启动时运行的代码

        DBABase.Start();

        if (!DBABase.VerfiyDBA("HardwareInfo")) DBABase.InitDBA();


        computer = new Computer();

        computer.CPUEnabled = true;
        computer.FanControllerEnabled = true;
        computer.MainboardEnabled = true;
        computer.RAMEnabled = true;
        computer.HDDEnabled = false;
        computer.GPUEnabled = true;

        computer.Open();

        Action check = everytime;
        check.BeginInvoke(null, null);

        //System.Threading.Thread

    }

    void Application_End(object sender, EventArgs e)
    {
        //  在应用程序关闭时运行的代码

        run = false;

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在出现未处理的错误时运行的代码

        run = false;
    }

    void Session_Start(object sender, EventArgs e)
    {
        // 在新会话启动时运行的代码

    }

    void Session_End(object sender, EventArgs e)
    {
        // 在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer
        // 或 SQLServer，则不引发该事件。

    }

</script>
