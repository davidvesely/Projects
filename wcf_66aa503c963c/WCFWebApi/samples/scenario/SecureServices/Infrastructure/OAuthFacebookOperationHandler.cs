// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace SecureServices.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    using Microsoft.ApplicationServer.Http.Dispatcher;

    public class OAuthFacebookOpHandler : HttpOperationHandler<HttpRequestMessage, HttpRequestMessage>
    {
        private readonly AuthorizeAttribute authorizationAttribute;

        private readonly string facebookAppId;

        private readonly Uri facebookBaseAuthUri = new Uri("https://www.facebook.com/dialog/oauth");

        public OAuthFacebookOpHandler()
            : base("response")
        {
        }

        public OAuthFacebookOpHandler(AuthorizeAttribute authorizeAttribute, string appId)
            : this()
        {
            this.authorizationAttribute = authorizeAttribute;
            this.facebookAppId = appId;
        }

        // ripped from MVC AuthorizeAttribute (http://aspnet.codeplex.com/SourceControl/changeset/view/70574#266447) 
        // kept the logic but removed dependency on HttpContextBase and use local attribute field (since we're obviously no longer in the AuthorizeAttribute class 
        // calling local copy (and creating local vars for users/roles
        // also removed virtual since it doesn't matter for this sample - may be worthwhile considering if we want a base class for something like AuthorizationOperationHandler
        protected bool AuthorizeCore(IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                return false;
            }

            string[] usersSplit = SplitString(this.authorizationAttribute.Users);
            if (usersSplit.Length > 0 && !usersSplit.Contains(principal.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            string[] rolesSplit = SplitString(this.authorizationAttribute.Roles);
            return rolesSplit.Length <= 0 || rolesSplit.Any(principal.IsInRole);
        }

        protected override HttpRequestMessage OnHandle(HttpRequestMessage input)
        {
            IIdentity identity = GetIdentity();

            if (identity == null)
            {
                var challengeMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                challengeMessage.Headers.WwwAuthenticate.Add(
                    new AuthenticationHeaderValue("oauth", "location=\"" + this.BuildFacebookAuthUri(input) + "\""));
                throw new HttpResponseException(challengeMessage);
            }

            var principle = new GenericPrincipal(identity, new string[0]);

            // set the thread context
            Thread.CurrentPrincipal = principle;

            if (!this.AuthorizeCore(principle))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return input;
        }

        private static IIdentity GetIdentity()
        {
            HttpCookie ticketCookie = HttpContext.Current.Request.Cookies["ticket"];
            if (ticketCookie == null)
            {
                return null;
            }

            string val = ticketCookie.Value;
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(val);
            var ident = new FormsIdentity(ticket);
            return ident;
        }

        // ripped from MVC AuthorizeAttribute (http://aspnet.codeplex.com/SourceControl/changeset/view/70574#266447) 
        private static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            IEnumerable<string> split = from piece in original.Split(',')
                                        let trimmed = piece.Trim()
                                        where !string.IsNullOrEmpty(trimmed)
                                        select trimmed;
            return split.ToArray();
        }

        private Uri BuildFacebookAuthUri(HttpRequestMessage request) {
            var returnUriBuilder = new UriBuilder
            {
                Host = request.Headers.Host,
                Path = request.RequestUri.LocalPath + "/authtoken"
            };

            var builder = new UriBuilder(this.facebookBaseAuthUri)
                {
                    Query =
                        string.Format(
                            "client_id={0}&redirect_uri={1}&response_type=code",
                            this.facebookAppId,
                            returnUriBuilder.Uri)
                };
            return builder.Uri;
        }
    }
}