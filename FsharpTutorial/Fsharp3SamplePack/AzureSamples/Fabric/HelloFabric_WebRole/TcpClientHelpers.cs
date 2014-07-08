//
// <copyright file="TcpClientHelpers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
namespace Microsoft.Samples.ServiceHosting.HelloFabric
{
    static class TcpClientHelpers
    {
        static void LogMessage(string fmt, params object[] args)
        {
            Trace.TraceInformation(String.Format(fmt, args));
        }
        static public bool TryConnect(this TcpClient tcpClient, IPEndPoint ep, int retires, int timeout)
        {
            // TODO: exponential backoff.
            do
            {
                try
                {
                    tcpClient.Connect(ep);
                    LogMessage("Connected to {0}", ep);
                    return true;
                }
                catch (SocketException e)
                {
                    switch (e.SocketErrorCode)
                    {
                        case SocketError.ConnectionRefused:
                        case SocketError.ConnectionReset:
                        case SocketError.HostDown:
                        case SocketError.Shutdown:
                        case SocketError.TimedOut:
                        case SocketError.TryAgain:
                            LogMessage("Connection to {0} failed with {1} wating {2} to retry",
                                ep, e.SocketErrorCode, timeout);
                            System.Threading.Thread.Sleep(timeout);
                            LogMessage("Retrying connection to {0}", ep);
                            retires--;
                            break;
                        default:
                            return false;
                    }
                }
            }
            while (retires > 0);
            return false;
        }
        static public string ReadAllData(this TcpClient tcpClient)
        {
            using (var strm = tcpClient.GetStream())
            {
                try
                {
                    var rd = new StreamReader(strm);
                    return rd.ReadToEnd();
                }
                finally
                {
                    strm.Close();
                }
            }
        }
    }
}
