// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace SecureServices.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel.Description;
    using System.Web.Mvc;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.ApplicationServer.Http.Description;

    public static class ConfigOAuthExtensions
    {
        /// <summary>
        /// Registers Facebook OAuth 2 functionality for all service operations decorated with <see cref="AuthorizeAttribute"/>
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> instance on which to set the OAuth extension objects.</param>
        /// <param name="client">The <see cref="FacebookOAuthClient"/> instance containing information for a registered Facebook application.</param>
        public static void RegisterFacebookOAuth(this HttpConfiguration config, FacebookOAuthClient client)
        {
            Action<Collection<HttpOperationHandler>, ServiceEndpoint, HttpOperationDescription> existingFactory =
                config.RequestHandlers;

            config.RequestHandlers = (c, e, od) =>
                {
                    if (existingFactory != null)
                    {
                        existingFactory(c, e, od);
                    }

                    AuthorizeAttribute authorizeAttribute = od.Attributes.OfType<AuthorizeAttribute>().FirstOrDefault();
                    if (authorizeAttribute == null)
                    {
                        return;
                    }

                    c.Add(new OAuthFacebookOpHandler(authorizeAttribute, client.AppId));
                };

            var handlers = new List<DelegatingHandler>();
            if (config.MessageHandlerFactory != null)
            {
                handlers.AddRange(config.MessageHandlerFactory());
            }

            config.MessageHandlerFactory = () =>
                {
                    handlers.Add(new OAuthFacebookMessageHandler(client.AppId, client.Secret));
                    return handlers;
                };
        }
    }

    public class FacebookOAuthClient
    {
        public FacebookOAuthClient(string appId, string secret)
        {
            this.AppId = appId;
            this.Secret = secret;
        }

        public string AppId { get; private set; }

        public string Secret { get; private set; }
    }
}