// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.ServiceModel.Channels;
    using System.Runtime.InteropServices;
    /// <summary>
    /// ETW session control
    /// </summary>
    internal class NativeMethods
    {
        public static ulong INVALID_HANDLE_VALUE = unchecked((ulong)(-1));
        public static IntPtr InvalidIntPtr = (IntPtr)(-1);
        public static IntPtr NullPtr = (IntPtr)(0);
        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public const uint
            EVENT_TRACE_CONTROL_QUERY = 0,
            EVENT_TRACE_CONTROL_STOP = 1,
            EVENT_TRACE_CONTROL_UPDATE = 2,
            EVENT_TRACE_CONTROL_FLUSH = 3,

            //ProcessTrace error messages
            ERROR_SUCCESS = 0,
            ERROR_INVALID_HANDLE = 6,
            ERROR_BAD_LENGTH = 24,
            ERROR_SHARING_VIOLATION = 32,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_ALREADY_STARTED = 183,
            ERROR_INSUFFICIENT_DISK = 112,
            ERROR_NOACCESS = 998,
            ERROR_CANCELLED = 1223,
            ERROR_INVALID_TIME = 1901;

        // predefined generic event types (0x00 to 0x09 reserved).
        public const byte EVENT_TRACE_TYPE_INFO = 0x00,  // Info or point event
            EVENT_TRACE_TYPE_START = 0x01,  // Start event
            EVENT_TRACE_TYPE_END = 0x02,  // End event
            EVENT_TRACE_TYPE_DC_START = 0x03,  // Collection start marker
            EVENT_TRACE_TYPE_DC_END = 0x04,  // Collection end marker
            EVENT_TRACE_TYPE_EXTENSION = 0x05,  // Extension/continuation
            EVENT_TRACE_TYPE_REPLY = 0x06,  // Reply event
            EVENT_TRACE_TYPE_DEQUEUE = 0x07,  // De-queue event
            EVENT_TRACE_TYPE_CHECKPOINT = 0x08,  // Generic checkpoint event
            EVENT_TRACE_TYPE_RESERVED9 = 0x09,

            // Event types for Process & Threads
            EVENT_TRACE_TYPE_LOAD = 0x0A,      // Load image

            // Event types for IO subsystem
            EVENT_TRACE_TYPE_IO_READ = 0x0A,
            EVENT_TRACE_TYPE_IO_WRITE = 0x0B,

            // Event types for Memory subsystem
            EVENT_TRACE_TYPE_MM_TF = 0x0A,      // Transition fault
            EVENT_TRACE_TYPE_MM_DZF = 0x0B,      // Demand Zero fault
            EVENT_TRACE_TYPE_MM_COW = 0x0C,      // Copy on Write
            EVENT_TRACE_TYPE_MM_GPF = 0x0D,      // Guard Page fault
            EVENT_TRACE_TYPE_MM_HPF = 0x0E,      // Hard page fault

            // Event types for Network subsystem, all protocols
            EVENT_TRACE_TYPE_SEND = 0x0A,     // Send
            EVENT_TRACE_TYPE_RECEIVE = 0x0B,     // Receive
            EVENT_TRACE_TYPE_CONNECT = 0x0C,	  // Connect
            EVENT_TRACE_TYPE_DISCONNECT = 0x0D,	  // Disconnect
            EVENT_TRACE_TYPE_RETRANSMIT = 0x0E,	  // ReTransmit
            EVENT_TRACE_TYPE_ACCEPT = 0x0F,     // Accept
            EVENT_TRACE_TYPE_RECONNECT = 0x10,     // ReConnect

            // Event Types for the Header (to handle internal event headers)
            EVENT_TRACE_TYPE_GUIDMAP = 0x0A,
            EVENT_TRACE_TYPE_CONFIG = 0x0B,
            EVENT_TRACE_TYPE_SIDINFO = 0x0C,
            EVENT_TRACE_TYPE_SECURITY = 0x0D,

            // Event types for Registry subsystem
            EVENT_TRACE_TYPE_REGCREATE = 0x0A,     // NtCreateKey
            EVENT_TRACE_TYPE_REGOPEN = 0x0B,     // NtOpenKey
            EVENT_TRACE_TYPE_REGDELETE = 0x0C,     // NtDeleteKey
            EVENT_TRACE_TYPE_REGQUERY = 0x0D,     // NtQueryKey
            EVENT_TRACE_TYPE_REGSETVALUE = 0x0E,     // NtSetValueKey
            EVENT_TRACE_TYPE_REGDELETEVALUE = 0x0F,     // NtDeleteValueKey
            EVENT_TRACE_TYPE_REGQUERYVALUE = 0x10,     // NtQueryValueKey
            EVENT_TRACE_TYPE_REGENUMERATEKEY = 0x11,     // NtEnumerateKey
            EVENT_TRACE_TYPE_REGENUMERATEVALUEKEY = 0x12,     // NtEnumerateValueKey
            EVENT_TRACE_TYPE_REGQUERYMULTIPLEVALUE = 0x13,     // NtQueryMultipleValueKey
            EVENT_TRACE_TYPE_REGSETINFORMATION = 0x14,     // NtSetInformationKey
            EVENT_TRACE_TYPE_REGFLUSH = 0x15,     // NtFlushKey
            EVENT_TRACE_TYPE_REGKCBDMP = 0x16,     // KcbDump/create

            // Event types for system configuration records
            EVENT_TRACE_TYPE_CONFIG_CPU = 0x0A,     // CPU Configuration
            EVENT_TRACE_TYPE_CONFIG_PHYSICALDISK = 0x0B,     // Physical Disk Configuration
            EVENT_TRACE_TYPE_CONFIG_LOGICALDISK = 0x0C,     // Logical Disk Configuration
            EVENT_TRACE_TYPE_CONFIG_NIC = 0x0D,     // NIC Configuration
            EVENT_TRACE_TYPE_CONFIG_VIDEO = 0x0E,     // Video Adapter Configuration
            EVENT_TRACE_TYPE_CONFIG_SERVICES = 0x0F,     // Active Services
            EVENT_TRACE_TYPE_CONFIG_POWER = 0x10;     // ACPI Configuration

        //
        // Enable flags for SystemControlGuid only
        //
        public const uint 
            EVENT_TRACE_FLAG_PROCESS = 0x00000001,  // process start & end
            EVENT_TRACE_FLAG_THREAD = 0x00000002,  // thread start & end
            EVENT_TRACE_FLAG_IMAGE_LOAD = 0x00000004,  // image load

            EVENT_TRACE_FLAG_DISK_IO = 0x00000100,  // physical disk IO
            EVENT_TRACE_FLAG_DISK_FILE_IO = 0x00000200,  // requires disk IO

            EVENT_TRACE_FLAG_MEMORY_PAGE_FAULTS = 0x00001000,  // all page faults
            EVENT_TRACE_FLAG_MEMORY_HARD_FAULTS = 0x00002000,  // hard faults only
            EVENT_TRACE_FLAG_NETWORK_TCPIP = 0x00010000,  // tcpip send & receive

            EVENT_TRACE_FLAG_REGISTRY = 0x00020000,  // registry calls
            EVENT_TRACE_FLAG_DBGPRINT = 0x00040000,  // DbgPrint(ex) Calls

            // Pre-defined Enable flags for everybody else
            EVENT_TRACE_FLAG_EXTENSION = 0x80000000,  // indicates more flags
            EVENT_TRACE_FLAG_FORWARD_WMI = 0x40000000,  // Can forward to WMI
            EVENT_TRACE_FLAG_ENABLE_RESERVE = 0x20000000,  // Reserved

            // Logger Mode flags
            EVENT_TRACE_FILE_MODE_NONE = 0x00000000,  // logfile is off
            EVENT_TRACE_FILE_MODE_SEQUENTIAL = 0x00000001,  // log sequentially
            EVENT_TRACE_FILE_MODE_CIRCULAR = 0x00000002,  // log in circular manner
            EVENT_TRACE_FILE_MODE_APPEND = 0x00000004,  // append sequential log
            EVENT_TRACE_FILE_MODE_NEWFILE = 0x00000008,  // auto-switch log file

            EVENT_TRACE_FILE_MODE_PREALLOCATE = 0x00000020,  // pre-allocate mode

            EVENT_TRACE_REAL_TIME_MODE = 0x00000100,  // real time mode on
            EVENT_TRACE_DELAY_OPEN_FILE_MODE = 0x00000200,  // delay opening file
            EVENT_TRACE_BUFFERING_MODE = 0x00000400,  // buffering mode only
            EVENT_TRACE_PRIVATE_LOGGER_MODE = 0x00000800,  // Process Private Logger
            EVENT_TRACE_ADD_HEADER_MODE = 0x00001000,  // Add a logfile header
            EVENT_TRACE_USE_GLOBAL_SEQUENCE = 0x00004000,  // Use global sequence no.
            EVENT_TRACE_USE_LOCAL_SEQUENCE = 0x00008000,  // Use local sequence no.

            EVENT_TRACE_RELOG_MODE = 0x00010000,  // Relogger;

            EVENT_TRACE_USE_PAGED_MEMORY = 0x01000000;  // Use pageable buffers  

        internal const int 
            WNODE_FLAG_TRACED_GUID = 0x00020000;

        [StructLayout(LayoutKind.Explicit)]
        internal struct FILETIME
        {
            [FieldOffset(0)]
            internal uint low;
            [FieldOffset(4)]
            internal uint high;
            [FieldOffset(0)]
            internal long ticks;

            public FILETIME(uint l, uint h)
            {
                ticks = 0;	// don't care; overwritten
                low = l;
                high = h;
            }
            public FILETIME(long ft)
            {
                low = (uint)ft;
                high = (uint)(ft >> 32);
                ticks = ft;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            internal ushort Year;
            internal ushort Month;
            internal ushort DayOfWeek;
            internal ushort Day;
            internal ushort Hour;
            internal ushort Minute;
            internal ushort Second;
            internal ushort Milliseconds;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_TRACE_HEADER
        {
            internal ushort Size;
            internal byte HeaderType;
            internal byte MarkerFlags;
            internal byte Type;
            internal byte Level;
            internal ushort Version;
            internal uint ThreadId;
            internal uint ProcessId;
            internal long TimeStamp;
            internal Guid Guid;
            internal uint ClientContext;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_TRACE
        {
            internal EVENT_TRACE_HEADER Header;
            internal uint InstanceId;
            internal uint ParentInstanceId;
            internal Guid ParentGuid;
            internal IntPtr MofData;
            internal uint MofLength;
            internal uint ClientContext;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TIME_ZONE_INFORMATION
        {
            internal int Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal string StandardName;
            internal SYSTEMTIME StandardDate;
            internal int StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal string DaylightName;
            internal SYSTEMTIME DaylightDate;
            internal int DaylightBias;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WCHAR8
        {
            internal char c01;
            internal char c02;
            internal char c03;
            internal char c04;
            internal char c05;
            internal char c06;
            internal char c07;
            internal char c08;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WCHAR32
        {
            internal WCHAR8 c01;
            internal WCHAR8 c02;
            internal WCHAR8 c03;
            internal WCHAR8 c04;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct TRACE_LOGFILE_HEADER
        {
            internal uint BufferSize;
            internal byte MajorVersion;
            internal byte MinorVersion;
            internal byte SubVersion;
            internal byte SubMinorVersion;
            internal uint ProviderVersion;
            internal uint NumberOfProcessors;
            internal long EndTime;
            internal uint TimerResolution;
            internal uint MaximumFileSize;
            internal uint LogFileMode;
            internal uint BuffersWritten;
            internal Guid LogInstanceGuid;
            [MarshalAs(UnmanagedType.SysInt)]
            internal IntPtr LoggerName;
            internal IntPtr LogFileName;
            internal TIME_ZONE_INFORMATION TimeZone;
            internal long BootTime;
            internal long PerfFreq;
            internal long StartTime;
            internal uint ReservedFlags;
            internal uint BuffersLost;
        }

        public delegate void EVENT_CALLBACK([In] ref EVENT_TRACE et);
        public delegate uint BUFFER_CALLBACK([In] ref EVENT_TRACE_LOGFILE etl);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct EVENT_TRACE_LOGFILE
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string LogFileName;
            [MarshalAs(UnmanagedType.LPTStr)]
            internal string LoggerName;
            internal long CurrentTime;
            internal uint BuffersRead;
            internal uint LogFileMode;
            internal EVENT_TRACE CurrentEvent;
            internal TRACE_LOGFILE_HEADER LogfileHeader;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            internal Delegate BufferCallback;
            internal uint BufferSize;
            internal uint Filled;
            internal uint EventsLost;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            internal Delegate EventCallback;
            internal ulong IsKernelTrace;
            internal IntPtr Context;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WNODE_HEADER
        {
            internal uint BufferSize;
            internal uint ProviderId;
            internal ulong HistoricalContext;
            internal ulong TimeStamp;
            internal Guid Guid;
            internal uint ClientContext;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class EVENT_TRACE_PROPERTIES
        {
            internal WNODE_HEADER Wnode;
            internal uint BufferSize = 0;
            internal uint MinimumBuffers = 0;
            internal uint MaximumBuffers =0 ;
            internal uint MaximumFileSize = 0;
            internal uint LogFileMode = 0;
            internal uint FlushTimer = 0;
            internal uint EnableFlags = 0;
            internal int AgeLimit = 0;
            internal uint NumberOfBuffers = 0;
            internal uint FreeBuffers = 0;
            internal uint EventsLost = 0;
            internal uint BuffersWritten = 0;
            internal uint LogBuffersLost = 0;
            internal uint RealTimeBuffersLost = 0;
            internal IntPtr LoggerThreadId = IntPtr.Zero;
            internal uint LogFileNameOffset;
            internal uint LoggerNameOffset;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class EventTraceProperties : EVENT_TRACE_PROPERTIES
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string LogFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x200)]
            public string LoggerName;

            public EventTraceProperties()
            {
                LoggerName = null;
                LogFileName = null;
                LogFileNameOffset = unchecked((uint)Marshal.SizeOf(typeof(EVENT_TRACE_PROPERTIES)));       // SizeOf(WNodeHeader) + 17 4-byte members
                LoggerNameOffset = unchecked((uint)Marshal.OffsetOf(typeof(EventTraceProperties), "LoggerName")); //returned offset is in bytes, so not conversion is necessary, only cast
                Wnode.BufferSize = unchecked((uint)Marshal.SizeOf(this)); //already in bytes
                Wnode.Flags = NativeMethods.WNODE_FLAG_TRACED_GUID;
            }
        }


        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        internal static extern uint FormatMessage(uint flags,
                                                    IntPtr Source,
                                                    uint MessageId,
                                                    uint LanguageId,
                                                    string Buffer,
                                                    uint Size);

        [DllImport("kernel32.dll")]
        internal static extern bool FileTimeToSystemTime(
            [In]	FILETIME ft,
            [Out]	SYSTEMTIME st);

        [DllImport("user32.dll")]
        internal static extern int MessageBox(int hwnd,
                                                string text,
                                                string caption,
                                                uint flags);

        [DllImport("advapi32.dll", EntryPoint = "OpenTraceW", SetLastError = true)]
        internal static extern ulong OpenTrace([In]	ref EVENT_TRACE_LOGFILE etl);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint CloseTrace([In]		ulong handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint ProcessTrace([In]		ulong[] handles,
                                                 [In]		uint count,
                                                 [In]	ref	FILETIME start,
                                                 [In]	ref	FILETIME end);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint ProcessTrace([In]		ulong[] handles,
         [In]		uint count,
         [In]		IntPtr start,
         [In]		IntPtr end);

        [DllImport("AdvApi32.dll", EntryPoint = "ControlTraceW")]
        internal static extern uint ControlTrace([In]	    ulong SessionHandle,
                                                [MarshalAs(UnmanagedType.LPWStr)] 
												[In]		string SessionName,
                                                [In, Out]	EventTraceProperties etp,
                                                [In]		uint ControlCode);

        [DllImport("AdvApi32.dll", EntryPoint = "QueryTraceW")]
        internal static extern uint QueryTrace([In]	        ulong SessionHandle,
                                                [MarshalAs(UnmanagedType.LPWStr)] 
												[In]		string SessionName,
                                                [In, Out]	EventTraceProperties etp);

        [DllImport("AdvApi32.dll", EntryPoint = "QueryAllTracesW")]
        internal static extern uint QueryAllTraces(
                                                [In, Out]	IntPtr[] etps,
                                                [In]		uint BufferCount,
                                                [Out]   out uint SessionCount);

        [DllImport("AdvApi32.dll", EntryPoint = "StartTraceW")]
        internal static extern uint StartTrace([In, Out]	ref ulong SessionHandle,
                                               [MarshalAs(UnmanagedType.LPWStr)] 
												[In]		string SessionName,
                                                [In, Out]	EventTraceProperties etp);

        [DllImport("AdvApi32.dll", EntryPoint = "EnableTrace")]
        internal static extern uint EnableTrace([In]		uint Enable,
                                                [In]		uint EnableFlag,
                                                [In]		uint EnableLevel,
                                                [In]	ref	Guid ControlGuid,
                                                [In]		ulong SessionHandle);

        [DllImport("AdvApi32.dll", EntryPoint = "StopTrace")]
        internal static extern uint StopTrace([In]		ulong SessionHandle,
                                              [In]		string SessionName,
                                              [In, Out]	EventTraceProperties etp);
    }
}
