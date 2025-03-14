using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SignalsDic = System.Collections.Generic.Dictionary<string, double[]>;
using Convert.Method.DWDataReader.API;

namespace Convert.Method.DWDataReader
{
    public class DWDataReader
    {
        public static SignalsDic ReadDxdFile(string filePath)
        {
            Debug.WriteLine($"Reading {filePath} ...");

            Debug.WriteLine(DWDataReaderAPI.DWInit());

            DWDataReaderAPI.DWFileinfo FileInfo = new DWDataReaderAPI.DWFileinfo();
            DWDataReaderAPI.DWOpenoriFile(filePath, ref FileInfo);
            Debug.WriteLine("Sample rate: {0}", FileInfo.sample_rate);
            Debug.WriteLine("Start store time: {0}", FileInfo.start_store_time);
            Debug.WriteLine("Duration: {0}", FileInfo.duration);

            long channelCount = DWDataReaderAPI.DWGetChannelListCount();
            Debug.WriteLine("Number of channels: {0}", channelCount);

            int channelListCount = DWDataReaderAPI.DWGetChannelListCount();
            DWDataReaderAPI.DWChannel[] dwChannelList = new DWDataReaderAPI.DWChannel[channelListCount];
            DWDataReaderAPI.DWGetChannelList(dwChannelList);

            SignalsDic signals = new SignalsDic();
            for (int i = 0; i < channelListCount; i++)
            {
                Debug.WriteLine("FULL SAMPLE RATE:");
                Debug.WriteLine("Index={0}; Name={1}; Unit={2}; Description={3}", dwChannelList[i].index, dwChannelList[i].name, dwChannelList[i].unit, dwChannelList[i].description);
                long numberOfSamples = DWDataReaderAPI.DWGetScaledSamplesCount(dwChannelList[i].index);
                double[] data;
                double[] timeStamp = new double[numberOfSamples];

                if (dwChannelList[i].array_size > 1)
                {
                    data = new double[numberOfSamples * dwChannelList[i].array_size];
                    DWDataReaderAPI.DWGetScaledSamples(dwChannelList[i].index, 0, (int)numberOfSamples * dwChannelList[i].array_size, data, timeStamp);
                }
                else
                {
                    data = new double[numberOfSamples];
                    DWDataReaderAPI.DWGetScaledSamples(dwChannelList[i].index, 0, (int)numberOfSamples, data, timeStamp);
                }
                signals["Second"] = timeStamp;
                signals.Add(dwChannelList[i].name, data);
            }

            DWDataReaderAPI.DWCloseoriFile();
            DWDataReaderAPI.DWDeInit();


            //    Debug.WriteLine("Finish. ");

            //    foreach (string name in signals.Keys)
            //        Console.Write($"{name}, ");
            //    Debug.WriteLine("");

            //    for (int i = 0; i < signals["time"].Length; i++)
            //    {
            //        foreach (double[] signal in signals.Values)
            //            Console.Write($"{signal[i]}, ");
            //        Debug.WriteLine("");
            //    }

            return signals;
        }
        public static void DxdToCsv(string dxdPath, string csvPath)
        {
            SignalsDic signal = ReadDxdFile(dxdPath);
            if (signal.Count == 0)
            {
                Debug.WriteLine("No signal read. ");
                return;
            }

            StreamWriter sw = new StreamWriter(csvPath, false, Encoding.UTF8);

            string str = "";
            foreach (string feature in signal.Keys)
                str += feature + ",";
            str = str.Remove(str.Length - 1);
            sw.WriteLine(str);

            for (int i = 0; i < signal["Second"].Length; i++)
            {
                str = "";
                foreach (double[] feature in signal.Values)
                    str += feature[i].ToString() + ",";
                str = str.Remove(str.Length - 1);
                sw.WriteLine(str);
            }

            sw.Close();
        }
    }
}
