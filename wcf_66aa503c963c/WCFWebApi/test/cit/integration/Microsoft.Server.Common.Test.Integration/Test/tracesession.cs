// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Server.Common.Diagnostics
{
    using System;
    using System.ServiceModel.Channels;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// managed ETW session controller
    /// </summary>
    public class TraceSession
    {
        ulong sessionId;	//TRACEHANDLE session handle from StartTrace
        bool running = false;
        string sessionname = null;
        uint buffersWritten;
        uint eventsLost = 0;
        uint eventsLostPrevious = 0;

        public TraceSession(string fileName)
        {
            this.sessionname = fileName;
        }

        public TraceSession(string fileName, bool start) : this(fileName)
        {
            if (start)
            {
                Start();
            }
            else
            {
                Lookup();
            }
        }

        public string DisableProvider(Guid guid)
        {
            uint res = NativeMethods.EnableTrace(0, 0, 255, ref guid, this.sessionId);
            return "disable trace returned: " + res;
        }

        public string EnableProvider(Guid guid, uint level)
        {
            if (!this.running)
            {
                //somebody has already stopped the session
                return "Cannot enable the provider in a stopped session";
            }

            uint res = NativeMethods.EnableTrace(1, 0, level, ref guid, this.sessionId);
            return "enable trace returned: " + res;
        }

        public static string Start(string sessionname, out ulong sessionId)
        {
            NativeMethods.EventTraceProperties etp = new NativeMethods.EventTraceProperties();

            etp.LogFileName = sessionname;
            sessionId = 0;

            //make the session name the same as filename to deal with changing file and session names
            uint res = NativeMethods.StartTrace(ref sessionId, sessionname, etp);

            //183 means file/session exists, which is ok, it will still be opened
            if (res != NativeMethods.ERROR_SUCCESS && res != NativeMethods.ERROR_ALREADY_STARTED)
            {
                switch (res)
                {
                    case NativeMethods.ERROR_SHARING_VIOLATION:
                    {	// likely the session is already running
                        res = NativeMethods.QueryTrace(sessionId, sessionname, etp);
                        if (res == NativeMethods.ERROR_SUCCESS)
                        {
                            sessionId = etp.Wnode.HistoricalContext;
                            return "QueryTrace: session " + sessionname + " is running with handle " + sessionId;
                        }
                        else
                            return "QueryTrace returned " + res + " (session has already been running)";
                    }
                    case NativeMethods.ERROR_INSUFFICIENT_DISK:
                    {
                        throw new ApplicationException("Out of disk space");
                    }
                    default:
                    {
                        throw new ApplicationException("Start error " + res);
                    }
                }
            }

            return "returned logger name: " + etp.LoggerName +
                   "\r\n" + "start trace returned: " + res +
                   "\r\n" + " session handle: " + sessionId + " session name: " + sessionname;
        }

        public string Start()
        {
            if (this.sessionname == null)
                return "Session name has not been not initialized";

            string result = TraceSession.Start(this.sessionname, out this.sessionId);
            this.running = true;

            return result;
        }

        public static string Stop(string sessionName)
        {
            NativeMethods.EventTraceProperties etp = new NativeMethods.EventTraceProperties();
            uint res = NativeMethods.StopTrace(0, sessionName, etp);

            return "stop trace returned: " + res;
        }

        public string Stop()
        {
            running = false;

            return TraceSession.Stop(this.sessionname);
        }

        public string Flush()
        {
            NativeMethods.EventTraceProperties etp = new NativeMethods.EventTraceProperties();
            ulong session = 0;
            uint res = NativeMethods.ControlTrace(session, sessionname, etp, NativeMethods.EVENT_TRACE_CONTROL_FLUSH);

            return "ControlTrace:Flush returned " + res;
        }

        public uint BuffersWritten
        {
            get
            {
                Query();
                return this.buffersWritten;
            }
        }

        public uint EventsLost
        {
            get
            {
                Query();
                return this.eventsLost;
            }
        }

        public uint EventsLostDelta
        {
            get
            {
                Query();
                uint result = this.eventsLost - this.eventsLostPrevious;
                this.eventsLostPrevious = this.eventsLost;
                return result;
            }
        }

        public string Query()
        {
            NativeMethods.EventTraceProperties etp = new NativeMethods.EventTraceProperties();
            uint res = NativeMethods.ControlTrace(sessionId, null, etp, NativeMethods.EVENT_TRACE_CONTROL_QUERY);

            if (res == NativeMethods.ERROR_SUCCESS)
            {
                this.eventsLost = etp.EventsLost;
                this.buffersWritten = etp.BuffersWritten;

                return "log file name:        " + etp.LogFileName + "\r\n"
                     + "lost events:          " + etp.EventsLost.ToString() + "\r\n"
                     + "buffer size:          " + etp.BufferSize.ToString() + "\r\n"
                     + "buffers:              " + etp.NumberOfBuffers.ToString() + "\r\n"
                     + "buffers written:      " + etp.BuffersWritten.ToString() + "\r\n"
                     + "buffers free:         " + etp.FreeBuffers.ToString() + "\r\n"
                     + "flush interval (sec): " + etp.FlushTimer.ToString() + "\r\n"
                     + "guid:                 " + etp.Wnode.Guid.ToString() + "\r\n"
                     + "provider id:          " + etp.Wnode.ProviderId.ToString() + "\r\n"
                     ;
            }
            else
                return "ControlTrace:Query returned " + res;
        }

        public static ulong Lookup(string sessionname)
        {
            NativeMethods.EventTraceProperties etp = new NativeMethods.EventTraceProperties();
            ulong session = 0;
            uint res = NativeMethods.ControlTrace(session, sessionname, etp, NativeMethods.EVENT_TRACE_CONTROL_UPDATE);

            if (res == NativeMethods.ERROR_SUCCESS)
            {
                session = etp.Wnode.HistoricalContext;
                return session;
            }
            else
                throw new Exception(res.ToString());
        }

        public string Lookup()
        {
            try
            {
                this.sessionId = Lookup(this.sessionname);
                this.running = true;
                return "session #" + this.sessionId + "\r\n";
            }
            catch (Exception e)
            {
                return "ControlTrace:Update returned " + e.Message;
            }
        }
    }
}

