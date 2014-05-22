// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Running AddFormatter...");
            AddFormatter.Run();

            Console.WriteLine("Running EnableHelpPage...");
            EnableHelpPage.Run();

            Console.WriteLine("Running AddMessageHandler...");
            AddMessageHandler.Run();

            Console.WriteLine("Running AddMessageHandlerByType...");
            AddMessageHandlerByType.Run();

            Console.WriteLine("Running SetInstanceProvider...");
            SetInstanceProvider.Run();

            Console.WriteLine("Running SetInstanceProviderMultipleHost...");
            SetInstanceProviderMultipleHost.Run();

            Console.WriteLine("Running AddRequestHandler...");
            AddRequestHandler.Run();

            Console.WriteLine("Running SetTrailingSlashMode...");
            SetTrailingSlashMode.Run();

            Console.WriteLine("Running AddResponseHandler...");
            AddResponseHandler.Run();

            Console.WriteLine("Running AddErrorHandler...");
            AddErrorHandler.Run();

            Console.WriteLine("Running IncludeExceptionDetail...");
            IncludeExceptionDetail.Run();

            Console.WriteLine("Running SetTransferMode...");
            SetTransferMode.Run();

            Console.WriteLine("Running SetMaxBufferSizeAndMaxReceivedMessageSize...");
            SetMaxBufferSizeAndMaxReceivedMessageSize.Run();

            Console.WriteLine("Running SetSecurity...");
            SetSecurity.Run();

            Console.WriteLine("Running UseDataContractSerializer...");
            UseDataContractSerializer.Run();

            Console.WriteLine("Running SetSerializer (XmlFormatter)...");
            SetSerializerOnXmlFormatter.Run();

            Console.WriteLine("Running SetSerializer (JsonFormatter)...");
            SetSerializerOnJsonFormatter.Run();
        }
    }
}
