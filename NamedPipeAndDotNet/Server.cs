using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CsvHelper;
using System.Collections.Generic;

namespace NamedPipeAndDotNet
{
    class Server
    {
        static void Main()
        {

            // get our sensor readings for the 8 sensors from the csv
            Queue<byte[]> records = getRecords();

            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
            {
                Console.WriteLine("NamedPipeServerStream object created.");

                // Wait for a client to connect

                var running = true;
                while (running)
                {
                    Console.Write("Waiting for client connection...");
                    pipeServer.WaitForConnection();

                    Console.WriteLine("Client connected.");

                    var session = true;
                    while (session)
                    {
                        byte[] transferFrame = new byte[4 * 8]; // need 4 bytes per sensor reading and have 8 sensors
                        foreach (byte[] record in records)
                        {

                            try
                            {
                                pipeServer.Write(record, 0, record.Length);
                            }
                            catch (System.IO.IOException e)
                            {
                                session = false;
                                running = false;
                                break;
                            }

                            pipeServer.WaitForPipeDrain();

                        }

                    }
                }
            }
        }

        static Queue<byte[]> getRecords()
        {
            var records = new Queue<byte[]>();
            using (var csvReader = new StreamReader("..//..//..//SampleData_8Sensor.csv"))
            using (var csv = new CsvReader(csvReader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var SensorReadings = new float[9];

                    SensorReadings[0] = csv.GetField<float>("S1");
                    SensorReadings[1] = csv.GetField<float>("S2");
                    SensorReadings[2] = csv.GetField<float>("S3");
                    SensorReadings[3] = csv.GetField<float>("S4");
                    SensorReadings[4] = csv.GetField<float>("S5");
                    SensorReadings[5] = csv.GetField<float>("S6");
                    SensorReadings[6] = csv.GetField<float>("S7");
                    SensorReadings[7] = csv.GetField<float>("S8");
                    SensorReadings[8] = csv.GetField<float>("time");

                    var bytesArray = new byte[SensorReadings.Length * 4];
                    Buffer.BlockCopy(SensorReadings, 0, bytesArray, 0, bytesArray.Length);
  
                    records.Enqueue(bytesArray);
                }
            };
            return records;
        }
    }
}




