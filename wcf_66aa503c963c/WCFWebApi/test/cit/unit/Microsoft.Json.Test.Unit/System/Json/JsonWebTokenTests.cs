// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.TestCommon;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Security;

    [TestClass, UnitTestLevel(UnitTestLevel.None)]
    public class JsonWebTokenTests : UnitTest<JsonWebToken>
    {
        #region Type
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonWebTokenTest is public class")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties<JsonWebToken>(TypeAssert.TypeProperties.IsPublicVisibleClass);
        }
        #endregion

        #region Constructor

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonWebToken()")]
        public void DefaultCtor()
        {
            JsonWebToken token = new JsonWebToken();
            Assert.AreEqual(2, token.Headers.Count);
            Assert.AreEqual(0, token.Claims.Count);
            Assert.IsNull(token.Actor);
            Assert.AreEqual(token.Algorithm, "none");
            Assert.IsNull(token.Audience);
            Assert.IsTrue(token.ExpirationTime == DateTime.MinValue);
            Assert.IsNull(token.IdentityProvider);
            Assert.IsFalse(token.IsSigned);
            Assert.IsTrue(token.IssuedAt == DateTime.MinValue);
            Assert.IsNull(token.Issuer);
            Assert.IsNull(token.NameIdentifier);
            Assert.IsTrue(token.NotBefore == DateTime.MinValue);
        }
        #endregion


        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Parse(string)")]
        public void ParseAndSerialize()
        {
            string headers = "{\"alg\":\"none\"}";

            // {}
            foreach (string[] ws in GetSetOfWhiteSpaces(3))
            {
                string claims = string.Concat(ws[0], "{", ws[1], "}", ws[2]);
                JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                for (int i = 0; i < 2; i++)
                {
                    Assert.IsNotNull(jwt);
                    Assert.AreEqual(1, jwt.Headers.Count);
                    Assert.AreEqual(0, jwt.Claims.Count);
                    Assert.AreEqual("none", jwt.Algorithm);
                    jwt = JsonWebToken.Parse(jwt.Serialize());
                }
            }

            // {name:string}
            foreach (string name in new string[] { "a", "  a  " })
            {
                foreach (string value in new string[] { "b", "  b  " })
                {
                    foreach (string[] ws in GetSetOfWhiteSpaces(6))
                    {
                        string claims = string.Concat(ws[0], "{", ws[1], "\"", name, "\"", ws[2], ":", ws[3], "\"", value, "\"", ws[4], "}", ws[5]);
                        JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                        for (int i = 0; i < 2; i++)
                        {
                            Assert.IsNotNull(jwt);
                            Assert.AreEqual(1, jwt.Headers.Count);
                            Assert.AreEqual(1, jwt.Claims.Count);
                            Assert.AreEqual("none", jwt.Algorithm);
                            Assert.AreEqual(value, jwt.Claims[name].ReadAs<string>());
                            jwt = JsonWebToken.Parse(jwt.Serialize());
                        }
                    }
                }
            }

            // {name:number}
            foreach (string name in new string[] { "A a", "  A a  " })
            {
                foreach (int value in new int[] { 0, 1, -12 })
                {
                    foreach (string[] ws in GetSetOfWhiteSpaces(6))
                    {
                        string claims = string.Concat(ws[0], "{", ws[1], "\"", name, "\"", ws[2], ":", ws[3], value, ws[4], "}", ws[5]);
                        JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                        for (int i = 0; i < 2; i++)
                        {
                            Assert.IsNotNull(jwt);
                            Assert.AreEqual(1, jwt.Headers.Count);
                            Assert.AreEqual(1, jwt.Claims.Count);
                            Assert.AreEqual("none", jwt.Algorithm);
                            Assert.AreEqual(value, jwt.Claims[name].ReadAs<int>());
                            jwt = JsonWebToken.Parse(jwt.Serialize());
                        }
                    }
                }
            }

            // {name:true}  {name:false}
            foreach (string name in new string[] { "a b", "  a b  " })
            {
                foreach (bool value in new bool[] { true, false })
                {
                    foreach (string[] ws in GetSetOfWhiteSpaces(6))
                    {
                        string claims = string.Concat(ws[0], "{", ws[1], "\"", name, "\"", ws[2], ":", ws[3], value.ToString().ToLowerInvariant(), ws[4], "}", ws[5]);
                        JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                        for (int i = 0; i < 2; i++)
                        {
                            Assert.IsNotNull(jwt);
                            Assert.AreEqual(1, jwt.Headers.Count);
                            Assert.AreEqual(1, jwt.Claims.Count);
                            Assert.AreEqual("none", jwt.Algorithm);
                            Assert.AreEqual(value, jwt.Claims[name].ReadAs<bool>());
                            jwt = JsonWebToken.Parse(jwt.Serialize());
                        }
                    }
                }
            }

            // {name:null}
            foreach (string name in new string[] { "1 2", "  1 2  " })
            {
                foreach (string[] ws in GetSetOfWhiteSpaces(6))
                {
                    string claims = string.Concat(ws[0], "{", ws[1], "\"", name, "\"", ws[2], ":", ws[3], "null", ws[4], "}", ws[5]);
                    JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                    for (int i = 0; i < 2; i++)
                    {
                        Assert.IsNotNull(jwt);
                        Assert.AreEqual(1, jwt.Headers.Count);
                        Assert.AreEqual(1, jwt.Claims.Count);
                        Assert.AreEqual("none", jwt.Algorithm);
                        Assert.IsNull(jwt.Claims[name]);
                        jwt = JsonWebToken.Parse(jwt.Serialize());
                    }
                }
            }

            // {" a\"\\\//\b\f\n\r\t\u12345 ":-12,"\tb\\\\ ":true}
            {
                string claims = "{\" a\\\"\\\\\\//\\b\\f\\n\\r\\t\\u12345 \":-12, \"\\tb\\\\\\\\ \":true}";
                JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt(headers, claims));
                for (int i = 0; i < 2; i++)
                {
                    Assert.IsNotNull(jwt);
                    Assert.AreEqual(1, jwt.Headers.Count);
                    Assert.AreEqual(2, jwt.Claims.Count);
                    Assert.AreEqual("none", jwt.Algorithm);
                    Assert.AreEqual(-12, jwt.Claims[" a\"\\//\b\f\n\r\t\u12345 "].ReadAs<int>());
                    Assert.IsTrue(jwt.Claims["\tb\\\\ "].ReadAs<bool>());
                    jwt = JsonWebToken.Parse(jwt.Serialize());
                }
            }

            // Sample token from ACS:
            // {
            //    "aud":"microsoft.sharepoint@taowan101", 
            //    "iss":"taowan101", 
            //    "cre":129538589082005093,
            //    "exp":129538625082005093, 
            //    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":"0#.w|REDMOND\\taowan", 
            //    "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor":"Microsoft.SharePoint.Workflow.fda4d8c7-d293-4c66-bfcb-02ed32b19b9c", 
            //    "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider":"unknown"
            // }
            {
                string claims = "{\"aud\":\"microsoft.sharepoint@taowan101\", \"iss\":\"taowan101\", \"cre\":129538589082005093,\"exp\":129538625082005093, \"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\":\"0#.w|REDMOND\\\\taowan\", \"http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor\":\"Microsoft.SharePoint.Workflow.fda4d8c7-d293-4c66-bfcb-02ed32b19b9c\", \"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider\":\"unknown\"}";
                JsonWebToken jwt = JsonWebToken.Parse(GetUnsignedJwt("{\"alg\":\"none\"}", claims));
                Assert.IsNotNull(jwt);
                Assert.AreEqual(1, jwt.Headers.Count);
                Assert.AreEqual(7, jwt.Claims.Count);
                Assert.AreEqual("none", jwt.Algorithm);
                Assert.AreEqual("microsoft.sharepoint@taowan101", jwt.Audience);
                Assert.AreEqual("taowan101", jwt.Issuer);
                Assert.AreEqual(129538589082005093, jwt.Claims["cre"].ReadAs<long>());
                Assert.AreEqual(129538625082005093, jwt.Claims["exp"].ReadAs<long>());
                Assert.AreEqual("0#.w|REDMOND\\taowan", jwt.Claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"].ReadAs<string>());
                Assert.AreEqual("Microsoft.SharePoint.Workflow.fda4d8c7-d293-4c66-bfcb-02ed32b19b9c", jwt.Claims["http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor"].ReadAs<string>());
                Assert.AreEqual("unknown", jwt.Claims["http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider"].ReadAs<string>());
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseAndAuthenticate(string,string)")]
        [DeploymentItem("WildcardCert.pfx")]
        public void SignAndAuthenticate()
        {
            // Sample token from the JWT spec:
            // {"typ":"JWT", "alg":"HS256"}.{"iss":"joe", "exp":1300819380, "http://example.com/is_root":true}
            string key = Convert.ToBase64String(new byte[] { 3, 35, 53, 75, 43, 15, 165, 188, 131, 126, 6, 101, 119, 123, 166, 143, 90, 179, 40, 230, 240, 84, 201, 40, 169, 15, 132, 178, 210, 80, 46, 191, 211, 251, 90, 146, 210, 6, 71, 239, 150, 138, 180, 195, 119, 98, 61, 34, 61, 46, 33, 114, 5, 46, 79, 8, 192, 205, 154, 245, 103, 208, 128, 163 });
            JsonWebToken jwt = JsonWebToken.ParseAndAuthenticate("eyJ0eXAiOiJKV1QiLA0KICJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ.dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk", key);
            Assert.IsNotNull(jwt);
            Assert.AreEqual(2, jwt.Headers.Count);
            Assert.AreEqual(3, jwt.Claims.Count);
            Assert.AreEqual("HS256", jwt.Algorithm);
            Assert.AreEqual("JWT", jwt.Headers["typ"].ReadAs<string>());
            Assert.AreEqual("joe", jwt.Issuer);
            Assert.AreEqual(1300819380, jwt.Claims["exp"].ReadAs<double>());
            Assert.IsTrue(jwt.Claims["http://example.com/is_root"].ReadAs<bool>());
            Assert.IsTrue(jwt.IsSigned);

            // HMAC-SHA signatures.
            foreach (string algorithm in new string[] { "HS256", "HS384", "HS512" })
            {
                JsonWebToken jwt1 = new JsonWebToken();
                jwt1.Algorithm = algorithm;
                jwt1.Issuer = "Adrian";
                jwt1.ExpirationTime = DateTime.Now.AddDays(1);
                Assert.IsFalse(jwt1.IsSigned);
                string str = jwt1.Sign(key);
                Assert.IsTrue(jwt1.IsSigned);

                JsonWebToken jwt2 = JsonWebToken.ParseAndAuthenticate(str, key);
                Assert.IsNotNull(jwt2);
                Assert.AreEqual(2, jwt2.Headers.Count);
                Assert.AreEqual(2, jwt2.Claims.Count);
                Assert.AreEqual(algorithm, jwt2.Algorithm);
                Assert.AreEqual(jwt1.Issuer, jwt2.Issuer);
                Assert.AreEqual(jwt1.ExpirationTime, jwt2.ExpirationTime);
                Assert.IsTrue(jwt2.IsSigned);

                // Invalidate the signature.
                jwt1.Headers.Add("new header", "some value");
                Asserters.Exception.Throws<SecurityException>(
                    () => { JsonWebToken.ParseAndAuthenticate(jwt1.Serialize(), key); },
                    (Exception) => { });
            }

            // RSA-SHA signatures.
            X509Certificate2 certificate = GetCertificate();
            foreach (string algorithm in new string[] { "RS256", "RS384", "RS512" })
            {
                JsonWebToken jwt1 = new JsonWebToken();
                jwt1.Algorithm = algorithm;
                jwt1.Issuer = "Adrian";
                jwt1.ExpirationTime = DateTime.Now.AddDays(1);
                Assert.IsFalse(jwt1.IsSigned);
                string str = jwt1.Sign(certificate);
                Assert.IsTrue(jwt1.IsSigned);

                JsonWebToken jwt2 = JsonWebToken.ParseAndAuthenticate(str, certificate);
                Assert.IsNotNull(jwt2);
                Assert.AreEqual(3, jwt2.Headers.Count);
                Assert.AreEqual(2, jwt2.Claims.Count);
                Assert.AreEqual(algorithm, jwt2.Algorithm);
                Assert.AreEqual(jwt1.Issuer, jwt2.Issuer);
                Assert.AreEqual(jwt1.ExpirationTime, jwt2.ExpirationTime);
                Assert.IsTrue(jwt2.IsSigned);

                // Invalidate the signature.
                jwt1.Headers.Add("new header", "some value");

                Asserters.Exception.Throws<SecurityException>(
                    () => { JsonWebToken.ParseAndAuthenticate(jwt1.Serialize(), certificate); },
                    (exception) => { });
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("ParseAndAuthenticate(string,byte[])")]
        [DeploymentItem("WildcardCert.pfx")]
        public void TestExceptions()
        {
            JsonWebToken jwt = new JsonWebToken();
            X509Certificate2 certificate = GetCertificate();
            string validJwtString = GetUnsignedJwt("{\"alg\":\"none\"}", "{}");

            // Invalid algorithm values.
            Asserters.Exception.ThrowsArgument("value", () => { jwt.Algorithm = "some string"; });

            // Serialize before required signing.
            jwt.Algorithm = "HS256";
            Asserters.Exception.Throws<InvalidOperationException>(
                () => { jwt.Serialize(); },
                (exception) => { });

            // Parse method exceptions.
            foreach (string token in new string[] { null, string.Empty })
            {
                Asserters.Exception.ThrowsArgument("rawToken", () => { JsonWebToken.Parse(token); });
                Asserters.Exception.ThrowsArgument("rawToken", () => { JsonWebToken.ParseAndAuthenticate(token, new byte[] { 1, 2, 3 }); });
            }

            // Invalid key for signing or authentication.
            jwt.Algorithm = "HS384";
            foreach (string key in new string[] { null, string.Empty })
            {
                Asserters.Exception.ThrowsArgument("key", () => { jwt.Sign(key); });
                Asserters.Exception.ThrowsArgument("key", () => { JsonWebToken.ParseAndAuthenticate(validJwtString, key); });
            }

            // Invalid key for signing or authentication.
            jwt.Algorithm = "HS384";
            foreach (byte[] key in new byte[][] { null, new byte[0] })
            {
                Asserters.Exception.ThrowsArgument("key", () => { jwt.Sign(key); });
                Asserters.Exception.ThrowsArgument("key", () => { JsonWebToken.ParseAndAuthenticate(validJwtString, key); });
            }

            // Invalid certificate for signing.
            jwt.Algorithm = "RS256";
            Asserters.Exception.ThrowsArgument("certificate", () => { jwt.Sign((X509Certificate2)null); });

            // Invalid certificate for authentication.            
            Asserters.Exception.ThrowsArgument("certificate", () => { JsonWebToken.ParseAndAuthenticate(validJwtString, (X509Certificate2)null); });

            // Incompatible Algorithm property and Sign method call.
            jwt.Algorithm = "RS256";
            Asserters.Exception.Throws<InvalidOperationException>(
                () => { jwt.Sign(new byte[] { 1, 2, 3 }); },
                (exception) => { });

            Asserters.Exception.Throws<InvalidOperationException>(
                () => { jwt.Sign(Convert.ToBase64String(new byte[] { 1, 2, 3 })); },
                (exception) => { });

            jwt.Algorithm = "HS256";
            Asserters.Exception.Throws<InvalidOperationException>(
                () => { jwt.Sign(certificate); },
                (exception) => { });
        }

        private static IEnumerable<string[]> GetSetOfWhiteSpaces(int count)
        {
            Assert.IsTrue(count >= 1);

            string[] whiteSpaces = new string[] { "", " ", "  ", " \t \n \r " };
            int[] indexes = new int[count];

            for (int i = count; i >= 0; )
            {
                if (i == count)
                {
                    string[] a = new string[count];
                    for (int j = 0; j < count; j++)
                    {
                        a[j] = whiteSpaces[indexes[j]];
                    }
                    yield return a;
                    --i;
                }
                else
                {
                    indexes[i]++;
                    if (indexes[i] == whiteSpaces.Length)
                    {
                        indexes[i--] = 0;
                    }
                    else
                    {
                        i = count;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an unsigned JSON Web Token with specified headers and claims.
        /// </summary>
        private static string GetUnsignedJwt(string jsonHeaders, string jsonClaims)
        {
            return string.Format("{0}.{1}.", Encode(jsonHeaders), Encode(jsonClaims));
        }

        private static string Encode(string s)
        {
            s = Convert.ToBase64String(Encoding.UTF8.GetBytes(s)); // standard base64 encoder
            s = s.TrimEnd('='); // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        private static X509Certificate2 GetCertificate()
        {
            FileInfo filePfx = new FileInfo(@"WildcardCert.pfx");
            Assert.IsTrue(filePfx.Exists, filePfx.FullName + " does not exist!");
            X509Certificate2 wildcardCert = new X509Certificate2(filePfx.FullName, "1234");
            Assert.IsTrue(wildcardCert.HasPrivateKey, "wildcardCer must have private key!");
            return wildcardCert;
        }
    }
}