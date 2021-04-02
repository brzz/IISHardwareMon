
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowData.aspx.cs" Inherits="ShowData" %>
    <!DOCTYPE html>
    <html style="height: 100%">
        
        <head>
            <meta charset="utf-8">
        </head>
        <script src="js/jquery-3.1.0.min.js"></script>
        <script src="js/ProExtend.js"></script>
        <script src="js/hashtable.js"></script>
        <script type="text/javascript" src="js/echarts.js"></script>
        <script>
		var colorlist = ["red", "green", "blue", "black", "lightpink", "lightblue", "lightgreen"];
		var colorindex = 0;
        function GetLineDefalutSet()
        {
            var s = {type: 'line',// 根据名字对应到相应的系列
                    name: '温度',
                    showSymbol: false,
                    connectNulls: false,
                    hoverAnimation: false,
                    symbol: "none", //空心点
                    data: new Array(),
                    markLine: {data: [{type: 'average',name: '平均值'}] }
            };
            return s;
        }
        
        function GetSetFromHashTable(HashTbaleSet)
        {
            //[
                //{type: 'line', // 根据名字对应到相应的系列
                // name: '温度',
                //data: daty//}
            //]
            
            return HashTbaleSet.getValues();
        }
        
        function pushJsontoHashtableSet(jsonarry, HashTbaleSet)
        {
		     colorindex = 0;
            jsonarry.forEach(function (value, index, array) {
                var t = new Array();
                t.push(parseInt(value.InsertTime + '000'));
                t.push(value.Value);
                if(HashTbaleSet.getKeys().lastIndexOf(value.Identifier) < 0)
                {
                    var dset = GetLineDefalutSet();
                    dset.name = value.HardwareName;
					dset.color = colorlist[colorindex];colorindex = colorindex+1;
                    HashTbaleSet.add(value.Identifier, dset);
                }
                
                HashTbaleSet.getValue(value.Identifier).data.push(t);
                
                //arry.push(t);
                //datx.push(value.InsertTime);
            });
        }
        </script>
        
        <body style="height: 100%; margin: 0">
            <form id="form1" style="height: 100%; margin: 0" runat="server">

                <script>
                //有几个线这里应该有几个配置，主要是存line配置用的
                var allsetanddata = new HashTable();

                var raw;
                var lasttime;
                var maxdata;
                var mindata;
				var ShowUint;
                var queryType = '<%="Temperature"%>';
                var queryName = '<%="CPU Package"%>';
                var RequestNumb = 240;



                
                 //var datx = new Array(); //x-y图需要 time图不用
                 /*//原版单折线
                var daty = new Array();

                function ConvertJsontoArrayWithDate(jsonarry, arry) {
                    jsonarry.forEach(function (value, index, array) {
                        var t = new Array();
                        t.push(parseInt(value.InsertTime + '000'));
                        t.push(value.Value)
                        arry.push(t);
                        //datx.push(value.InsertTime);
                    });
                }
                */

                function ConvertJsontoArray(jsonarry, arry) {
                    jsonarry.forEach(function (value, index, array) {
                        arry.push(value.Value);
                        //datx.push(value.InsertTime);
                    });
                }


                //一次性获得大量数据并统一显示
                function Getdata(counter) {
                    myChart.showLoading(); //loading动画
                    allsetanddata.clear();
                    //$.ajaxSettings.async = false;//同步加载ajax，防止在加载AJAX时候EASYUI已经开始加载导致数据不对
                    ////Temperature
                    //CPU Package
                    $.ajax({
                        type: "POST",
                        url: "ShowData.aspx/GetData",
                        data: "{ getCount: '" + counter + "',Type:\"" + queryType + "\",Name:\"" + queryName + "\" }", //可选参数
                        dataType: "json",
                        contentType: 'application/json;charset=utf-8', // 设置请求头信息
                        success: function (data) {
                            myChart.hideLoading();

                            raw = new Array();
                            lasttime = data.d.rows[data.d.rows.length - 1].InsertTime;
                            pushJsontoHashtableSet(data.d.rows, allsetanddata);//新版多折线
                            //ConvertJsontoArrayWithDate(data.d.rows, daty);//原版单折线
                            //ConvertJsontoArray(data.d.rows, raw); //原版手动确定坐标大小

                           // var max = raw.max();
                           // var min = raw.min();
                           // maxdata = GetNearbyNumbMax(max, min, 0.2);
                           // mindata = GetNearbyNumbMin(max, min, 0.2)
						   ShowUint = data.d.ShowUnit;
						   
						   if(data.d.ShowUnit == null || data.d.ShowUnit == '')
						   {
						       option.yAxis.axisLabel.formatter = "{value}";
						   }
						   else
						   {
						       option.yAxis.axisLabel.formatter = "{value} " + ShowUint;
						   }

                            //根据配置文件调节自动缩放
							if(data.d.ShowMIN == 0 && data.d.ShowMAX == 0)
							{
							    option.yAxis.min=undefined
								option.yAxis.max=undefined
							    option.yAxis.scale = true;
								option.tooltip={trigger: 'axis'};
							    myChart.setOption(option);
							}
							else
							{
							    option.yAxis.scale = false;
								option.yAxis.min = data.d.ShowMIN;
								option.yAxis.max = data.d.ShowMAX;
								option.tooltip={trigger: 'axis', formatter: function (params) {
var res = "";
var simpletime = new Date(params[0].data[0]);
res +=  simpletime.toLocaleString() + "<br/>" ;
params.forEach(function (item) {
　　　　//console.log(item)
　　　　res +=  item.marker + item.seriesName + " : " + item.data[1] + " " + ShowUint + "</br>"
})
return res;
}};
							    myChart.setOption(option);
							}
							
							//缩放调节后 填入数据
                            myChart.setOption({
                                //xAxis: {
                                //    data: datx
                                //},
                                
                                // 填入数据
                                series: GetSetFromHashTable(allsetanddata)

                            });
                        },

                        error: function (errorMsg) {
                            //请求失败时执行该函数
                            alert("请求数据失败!");
                            myChart.hideLoading();
                        }
                    });

                    //$.ajaxSettings.async = true;//异步加载ajax
                }


                
                //实时刷新用的，这里没有更改
                function GetOneDataandPush() {
                    $.ajax({
                        type: "POST",
                        url: "ShowData.aspx/GetData",
                        data: "{ getCount: '1',Type:\"" + queryType + "\",Name:\"" + queryName + "\" }", //可选参数
                        dataType: "json",
                        contentType: 'application/json;charset=utf-8', // 设置请求头信息
                        success: function (data) {
                            var rowtime = data.d.rows[data.d.rows.length - 1].InsertTime;
                            if (rowtime > lasttime) {
                                ConvertJsontoArrayWithDate(data.d.rows, daty);
                                lasttime = rowtime;
                                // 填入数据
                                myChart.setOption({
                                    series: [{
                                            type: 'line',
                                            // 根据名字对应到相应的系列
                                            name: '温度',
                                            data: daty
                            }]
                                });
                            }
                        },



                        error: function (errorMsg) {
                            //请求失败时执行该函数
                            alert("请求数据失败!");
                            myChart.hideLoading();
                        }
                    });
                }


                function flushtable() {
                    daty = [];
                    myChart.setOption(option, true)
                    RequestNumb = $('#Select1').val();
                    queryType = $('#DropDownList1').val();
                    queryName = $('#DropDownList2').val();
                    Getdata(RequestNumb);
                }
                </script>
                显示设备：
                <select id="DropDownList1" runat="server"></select>
                <select id="DropDownList2" runat="server"></select>
                <select id="Select1" runat="server">
                    <option value="240">240</option>
                    <option value="480">480</option>
                    <option value="1440">1440</option>
                    <option value="2880">2880</option>
                    <option value="5760">5760</option>
                    <option value="40320">40320</option>
                </select>
                <input id="Button2" type="button" value="确定" onclick="flushtable()" />
                <div id="container" style="height: 90%"></div>
				<script type="text/javascript">
				    //下拉框用
				    var droplistdat = <%=dropliststr%>;

				    $("#DropDownList1").change(function(){  
				        var sel = $("#DropDownList1").val();
				        $("#DropDownList2").empty();

				        for(i=0; i<droplistdat.length; i++)
				        {
				            if(droplistdat[i].type == sel)
				            {
				                $("#DropDownList2").append("<option value='" + droplistdat[i].SesonName + "'>" + droplistdat[i].SesonName + "</option>");
				            }
				        }
				    })
				</script>
                <script type="text/javascript">
                var dom = document.getElementById("container");
                var myChart = echarts.init(dom);
                var app = {};
                option = null;
                option = {
                    title: {
                        text: '硬件状态变化',
                        subtext: '物理机器'
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: ['最高温'] //, '最低温']
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            dataZoom: {
                                yAxisIndex: 'none'
                            },
                            dataView: {
                                readOnly: true
                            },
                            magicType: {
                                type: ['line', 'bar']
                            },
                            restore: {},
                            saveAsImage: {}
                        }
                    },
                    xAxis: {
                        //X轴设置
                        type: 'time', //'category',
                        axisTick: {
                            alignWithLabel: true
                        },
                        //axisLine: {
                        //     onZero: false,
                        // },
                        //data: []
                    },
                    //dataZoom: [{
                    //        //X轴滚动条
                    //        type: 'slider',
                    //        show: true,
                   // //        xAxisIndex: [0],
                   //         left: '9%',
                   //         bottom: -5,
                    //        start: 0,
                   //         end: 100 //初始化滚动条位置
           // }],

                    yAxis: {
                        //Y轴设置
                        type: 'value',
                        scale: true, //自动调节
                        boundaryGap: [0.05, 0.05], //下方保留0.05  上方保留0.05
                        //min: 10,
                        //max:90,
                        axisLabel: {
                            formatter: '{value} °C'
                        }
                    },
                    series: [
                        {
                            name: '最高温',
                            type: 'line',
                            //smooth: true,//平滑
                            data: [],
                            markPoint: {
                                data: [
                                    {
                                        type: 'max',
                                        name: '最大值'
                                    },
                                    {
                                        type: 'min',
                                        name: '最小值'
                                    }
                                ]
                            },
                            markLine: {
                                data: [
                                    {
                                        type: 'average',
                                        name: '平均值'
                                    }
                                ]
                            },
                },
                    ]
                };

                myChart.showLoading(); //loading动画

                Getdata(RequestNumb);
                 // setInterval("GetOneDataandPush()", 15000);

                if (option && typeof option === "object") {
                    myChart.setOption(option, true);
                }
                </script>
            </form>
        </body>
    
    </html>