using System;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using System.Collections.Specialized;

namespace sim_influxdb
{
    class Program
    {
 
        public static async Task Main(string[] args)
        {

            System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcessesByName("sim_influxdb");//获取指定的进程名   
            if (myProcesses.Length > 1) //如果可以获取到知道的进程名则说明已经启动
            {
                Console.WriteLine("sim_influxdb has been running. ");
            }else
            {
                // You can generate an API token from the "API Tokens Tab" in the UI
                var token = ReadConfigString("token","ffitfWuvcUsKA1TxgjcUA-T6VZu9GZ4Ev6dklrKYpQcTzZ5DeH2hrHCGY__26V0QAwNHhZiVBoR63Vz6byKhPg==");
                String bucket = ReadConfigString("bucket","my-bucket");
                String org = ReadConfigString("org","Lu Jie");
                String dbURL = ReadConfigString("URL","http://localhost:8086");
                int interval = ReadConfig("interval", 1000);
                int count = ReadConfig("count", 2000);

                Console.WriteLine("URL = {0}\ncount = {1}\ninterval = {2}", dbURL, count, interval);

                using var client = new InfluxDBClient(dbURL, token);

                if (client == null)
                {
                    Console.WriteLine("Fail to connect with influxdb. ");
                }
                else
                {
                    var writeApi = client.GetWriteApi();
                    if (writeApi == null)
                    {
                        Console.WriteLine("Fail to get API.");
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int data_0 = i % 50;
                            double data_1 = 50 * Math.Sin(i * Math.PI / 100.0);
                            var point = PointData
                                        .Measurement("measurement1")
                                        .Tag("host", "host1")
                                        .Field("field1", data_0)
                                        .Field("field2", data_1)
                                        .Field("field3", data_1)
                                        .Field("field4", data_1)
                                        .Field("field5", data_1)
                                        .Field("field6", data_1)
                                        .Field("field7", data_1)
                                        .Timestamp(DateTime.UtcNow, WritePrecision.Ms);


                            writeApi.WritePoint(point, bucket, org);

                            Console.WriteLine("...{0}, {1}", i, data_1);

                            //Thread.Sleep(1000);
                            await Task.Delay(interval);
                        }
                    }

                }

            }

            static int ReadConfig(String key, int default_data)
            {
                int data = default_data;
                
                if (ConfigurationManager.AppSettings[key] != null)
                {
                    try
                    {
                        data = Convert.ToInt32(ConfigurationManager.AppSettings[key]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Write configuration error:{0}", ex.Message);
                    }
                }

                return data;
            }

            static string ReadConfigString(string key, string default_data)
            {
                string data = default_data;

                if (ConfigurationManager.AppSettings[key] != null)
                {
                    data = ConfigurationManager.AppSettings[key];
                }

                return data;
            }
        }
    }
}
