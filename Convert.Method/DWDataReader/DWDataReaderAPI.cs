using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Convert.Method.DWDataReader.API
{
    public class DWDataReaderAPI
    {
        //#if _WIN64
        //        public const string DLL_Path = @".\DWDataReaderLib64.dll";
        //#else
        //        public const string DLL_Path = @".\DWDataReaderLib.dll";
        //#endif
        public const string DLL_Path = @".\DWDataReaderLib64.dll";

        public enum DWStatus                    // status returned from library function calls
        {
            DWSTAT_OK = 0,                                  // status OK
            DWSTAT_ERROR = 1,                               // error occurred in the dll
            DWSTAT_ERROR_FILE_CANNOT_OPEN = 2,              // unable to open file
            DWSTAT_ERROR_FILE_ALREADY_IN_USE = 3,           // file already in use
            DWSTAT_ERROR_FILE_CORRUPT = 4,                  // file corrupted
            DWSTAT_ERROR_NO_MEMORY_ALLOC = 5,               // memory not allocated
            DWSTAT_ERROR_CREATE_DEST_FILE = 6,              // error creating destination file (only for d7z files)
            DWSTAT_ERROR_EXTRACTING_FILE = 7,               // error extracting data (only for d7z files)
            DWSTAT_ERROR_CANNOT_OPEN_EXTRACTED_FILE = 8     // error opening extracted file (only for d7z files)
        }

        // channel properties
        public enum DWChannelProps              // used for DWGetChannelProps() calls
        {
            DW_DATA_TYPE = 0,                               // get data type
            DW_DATA_TYPE_LEN_BYTES = 1,                     // get length of data type in bytes
            DW_CH_INDEX = 2,                                // get channel index
            DW_CH_INDEX_LEN = 3,                            // get length of channel index
            DW_CH_TYPE = 4,                                 // get channel type
            DW_CH_SCALE = 5,                                // get channel scale
            DW_CH_OFFSET = 6,                               // get channel offset
            DW_CH_XML = 7,                                  // get channel XML
            DW_CH_XML_LEN = 8,                              // get length of channel XML
            DW_CH_XMLPROPS = 9,                             // get channel XML properties
            DW_CH_XMLPROPS_LEN = 10,                        // get length of channel XML properties
            DW_CH_CUSTOMPROPS = 11,                         // get channel XML custom properties
            DW_CH_CUSTOMPROPS_COUNT = 12,	                // get length of channel XML custom properties
            DW_CH_LONGNAME = 13,                            // get channel long name
            DW_CH_LONGNAME_LEN = 14                         // get length of channel long name
        }

        public enum DWChannelType                       // channel type
        {
            DW_CH_TYPE_SYNC = 0,                            // sync channel
            DW_CH_TYPE_ASYNC = 1,                           // async channel
            DW_CH_TYPE_SV = 2                               // single value channel
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DWCANPortData
        {
            public uint arb_id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public string data;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DWFileinfo
        {
            public double sample_rate;                             // the sampling rate
            public double start_store_time;                        // absolute time of the start of storing (days)
            public double duration;                                // duration of data file (seconds)
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct DWChannel                                     // structure used for DeweSoft channel
        {
            public int index;                                              // unique channel identifier  
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string name;                                              // the name of a channel
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string unit;                                              // the unit of a channel
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
            public string description;                                       // the description of a channel
            public uint color;                                             // specifies the color of a channel
            public int array_size;                                         // length of the array channel (normal array_size = 1)
            public int data_type;                                          // channel data type - see data types in buffer 
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct DWEvent
        {
            public int event_type;                                         // 1..start event; 2..stop event
            public double time_stamp;                                      // relative position in seconds
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
            public string event_text;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DWReducedValue
        {
            public double time_stamp;                                      // relative time in seconds
            public double ave;
            public double min;
            public double max;
            public double rms;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DWArrayInfo
        {
            int index;                                              // unique axis identifier  
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            string name;                                              // axis name
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            string unit;                                              // axis unit
            int size;                                               // length of the axis size
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DWComplex
        {
            double re;
            double im;
        }

        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWDeInit();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWInit();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWAddReader();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetNumReaders(ref int num_readers);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWSetActiveReader(int index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetVersion();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWOpenoriFile(string file_name, ref DWFileinfo file_info);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWCloseoriFile();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetChannelListCount();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetChannelList([In, Out] DWChannel[] channel_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetChannelFactors(int ch_index, ref double scale, ref double offset);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetChannelProps(int ch_index, DWChannelProps ch_prop, IntPtr buffer, ref int max_len);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern long DWGetBinarySamplesCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetBinarySamples(int ch_index, long sampleIndex, ref string data, [In, Out] double[] time_stamp, ref int datalen);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern long DWGetScaledSamplesCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetScaledSamples(int ch_index, long position, int count, [In, Out] double[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetRawSamples(int ch_index, long position, int count, [In, Out] IntPtr[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexChannelListCount();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexChannelList([In, Out] DWChannel[] channel_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexScaledSamplesCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexScaledSamples(int ch_index, long position, int count, [In, Out] DWComplex[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexRawSamplesCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetComplexRawSamples(int ch_index, long position, int count, [In, Out] DWComplex[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetEventListCount();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetEventList([In, Out] DWEvent[] event_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetStream(ref string stream_name, ref string buffer, ref int max_len);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWExportHeader(string file_name);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTextChannelListCount();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTextChannelList([In, Out] DWChannel[] channel_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTextValuesCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTextValues(int ch_index, int position, int count, [In, Out] StringBuilder[] text_values, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetReducedValuesCount(int ch_index, ref int count, ref double block_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetReducedValues(int ch_index, int position, int count, [In, Out] DWReducedValue[] data);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetHeaderEntryCount();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryList([In, Out] DWChannel[] channel_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryText(int ch_index, string text_value, int text_value_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetStoringType();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetArrayInfoCount(int ch_index);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetArrayInfoList(int ch_index, [In, Out] DWArrayInfo[] array_inf_list);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetArrayIndexValue(int ch_index, int array_info_index, int array_value_index, string value, int value_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetArrayIndexValueF(int ch_index, int array_info_index, int array_value_index, ref double value);

        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetChannelListItem(int array_index, ref int index, ref string name, ref string unit, ref string description, ref string color, ref int array_size, int max_char_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryListItem(int array_index, ref int index, ref string name, ref string unit, ref string description, ref int color, ref int array_size, int max_char_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetEventListItem(int event_Index, ref int event_type, ref double time_stamp, ref string event_text, int max_char_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetReducedAveValues(int ch_index, int position, int count, [In, Out] double[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetReducedMinValues(int ch_index, int position, int count, [In, Out] double[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetReducedMaxValues(int ch_index, int position, int count, [In, Out] double[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetReducedRMSValues(int ch_index, int position, int count, [In, Out] double[] data, [In, Out] double[] time_stamp);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryTextF(int entry_number, ref string text_value, int text_value_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryNameF(int entry_number, ref string name, int name_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetHeaderEntryIDF(int entry_number, ref string ID, int name_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern double DWGetEventTimeF(int event_number);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetEventTextF(int event_number, ref string text, int text_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetEventTypeF(int event_number);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetReducedDataChannelCountF();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetReducedDataChannelNameF(int Channel_Number, ref string name, int name_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetReducedDataChannelIndexF(ref string name);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetRecudedDataChannelInfoF(int Channel_Number, ref string X_Axis_Units, int X_Axis_Units_size, ref string Y_Axis_Units, int Y_Axis_Units_size, ref double Chn_Offset, ref int Channel_Length, ref double ch_rate);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetRecudedDataF(int Channel_Number, ref double X_Axis, ref double Y_Axis, int position, int count);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetRecudedYDataF(int Channel_Number, ref double Y_Axis, int position, int count);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetRecudedDataAllF(int Channel_Number, ref double Y_MIN_Axis, ref double Y_AVE_Axis, ref double Y_MAX_Axis, ref double Y_RMS_Axis, int position, int count);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTriggerDataTriggerCountF();
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern double DWGetTriggerDataTriggerTimeF(int Trigger_Number);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetTriggerDataChannelNameF(int Channel_Number, ref string name, int name_size);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DWGetTriggerDataChannelIndexF(ref string name);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetTriggerDataChannelInfoF(int Trigger_Number, int Channel_Number, ref string X_Axis_Units, int X_Axis_Units_size, ref string Y_Axis_Units, int Y_Axis_Units_size, ref double Chn_Offset, ref double Channel_Length, ref double ch_rate, ref int ch_type);
        [DllImport(DLL_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern DWStatus DWGetTriggerDataF(int Trigger_Number, int Channel_Number, ref double Y_Axis, ref double X_Axis, double position, int count);


    }
}
