// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Json;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.Server.Common;

    /// <summary>
    /// Represents a JSON Web Token (JWT) as defined by the spec http://self-issued.info/docs/draft-jones-json-web-token.html.
    /// </summary>
    public class JsonWebToken
    {
        // Header Parameter Names
        private const string AlgorithmHeader = "alg";
        private const string X509ThumbprintHeader = "x5t";
        private const string TypeHeader = "typ";

        // Claim Names
        private const string ExpirationTimeClaim = "exp";
        private const string NotBeforeClaim = "nbf";
        private const string IssuedAtClaim = "iat";
        private const string IssuerClaim = "iss";
        private const string AudienceClaim = "aud";
        private const string NameIdentifierClaim = "nameid";
        private const string ActorClaim = "actortoken";
        private const string IdentityProviderClaim = "identityprovider";

        private static readonly DateTime epochTimeBase = new DateTime(1970, 1, 1);

        private JsonObject headers = new JsonObject(new KeyValuePair<string, JsonValue>(TypeHeader, "JWT"));
        private JsonObject claims = new JsonObject();
        private string headerSegment;
        private string claimSegment;
        private string signature;

        /// <summary>
        /// Creates a JSON Web Token (JWT) without any headers, claims or signature.
        /// </summary>
        public JsonWebToken()
        {
            this.Algorithm = JsonWebTokenAlgorithms.None;
        }

        /// <summary>
        /// Gets all the headers within the token, including the Algorithm.
        /// </summary>
        public JsonObject Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        /// Gets all the claims within the token, including the ExpirationTime, NotBefore, IssuedAt, Issuer and Audience.
        /// </summary>
        public JsonObject Claims
        {
            get
            {
                return this.claims;
            }
        }

        /// <summary>
        /// Gets or sets the alg (algorithm) header parameter identifies the cryptographic algorithm used to secure the JWT token.
        /// </summary>
        /// <value>
        /// The algorithm.
        /// </value>
        public string Algorithm
        {
            get
            {
                JsonValue value;
                return this.headers.TryGetValue(AlgorithmHeader, out value) && (value != null) ? value.ReadAs<string>() : null;
            }

            set
            {
                // The algorithm header value is case sensitive.
                switch (value)
                {
                    case JsonWebTokenAlgorithms.None:
                    case JsonWebTokenAlgorithms.HmacSha256:
                    case JsonWebTokenAlgorithms.HmacSha384:
                    case JsonWebTokenAlgorithms.HmacSha512:
                    case JsonWebTokenAlgorithms.RsaSha256:
                    case JsonWebTokenAlgorithms.RsaSha384:
                    case JsonWebTokenAlgorithms.RsaSha512:
                        this.headers[AlgorithmHeader] = value;
                        break;
                    
                    default:
                        throw Fx.Exception.Argument("value", string.Format(SR.Culture, SR.JsonWebToken_InvalidAlgorithm));
                }
            }
        }

        /// <summary>
        /// Gets or sets the exp (expiration time) claim identifying the time on or after which the token MUST NOT be accepted for processing.
        /// The processing of the exp claim requires that the current date/time MUST be before the expiration date/time listed in the exp claim.
        /// Implementers MAY provide for some small leeway, usually no more than a few minutes, to account for clock skew. This claim is OPTIONAL.
        /// </summary>
        /// <remarks>The JWT header stores the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.</remarks>
        /// <value>
        /// The expiration time.
        /// </value>
        public DateTime ExpirationTime
        {
            get
            {
                return this.GetDateTimeClaim(ExpirationTimeClaim);
            }

            set
            {
                this.SetDateTimeClaim(ExpirationTimeClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the nbf (not before) claim identifying the time before which the token MUST NOT be accepted for processing.
        /// The processing of the nbf claim requires that the current date/time MUST be after or equal to the not-before date/time listed in the nbf claim.
        /// Implementers MAY provide for some small leeway, usually no more than a few minutes, to account for clock skew. This claim is OPTIONAL.
        /// </summary>
        /// <remarks>The JWT header stores the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.</remarks>
        public DateTime NotBefore
        {
            get
            {
                return this.GetDateTimeClaim(NotBeforeClaim);
            }

            set
            {
                this.SetDateTimeClaim(NotBeforeClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the iat (issued at) claim identifying the time at which the JWT was issued. This claim can be used to determine the age of the token.
        /// This claim is OPTIONAL.
        /// </summary>
        /// <remarks>The JWT header stores the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.</remarks>
        public DateTime IssuedAt
        {
            get
            {
                return this.GetDateTimeClaim(IssuedAtClaim);
            }

            set
            {
                this.SetDateTimeClaim(IssuedAtClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the iss (issuer) claim identifying the principal that issued the JWT. The processing of this claim is generally application specific.
        /// The iss value is case sensitive. This claim is OPTIONAL. 
        /// </summary>
        public string Issuer
        {
            get
            {
                return this.GetStringClaim(IssuerClaim);
            }

            set
            {
                this.SetStringClaim(IssuerClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the aud (audience) claim identifying the audience that the JWT is intended for. The principal intended to process the JWT MUST be identified 
        /// by the value of the audience claim. If the principal processing the claim does not identify itself with the identifier in the aud claim value 
        /// then the JWT MUST be rejected. The interpretation of the contents of the audience value is generally application specific. 
        /// The value is case sensitive. This claim is OPTIONAL.
        /// </summary>
        public string Audience
        {
            get
            {
                return this.GetStringClaim(AudienceClaim);
            }

            set
            {
                this.SetStringClaim(AudienceClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the name identifier claim.
        /// </summary>
        public string NameIdentifier
        {
            get
            {
                return this.GetStringClaim(NameIdentifierClaim);
            }

            set
            {
                this.SetStringClaim(NameIdentifierClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the actor claim.
        /// </summary>
        public string Actor
        {
            get
            {
                return this.GetStringClaim(ActorClaim);
            }

            set
            {
                this.SetStringClaim(ActorClaim, value);
            }
        }

        /// <summary>
        /// Gets or sets the identity provider claim.
        /// </summary>
        public string IdentityProvider
        {
            get
            {
                return this.GetStringClaim(IdentityProviderClaim);
            }

            set
            {
                this.SetStringClaim(IdentityProviderClaim, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the token is signed or not.
        /// </summary>
        public bool IsSigned
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.signature);
            }
        }

        /// <summary>
        /// Creates a JsonWebToken from the specified string representation. No signature verification is performed.
        /// </summary>
        /// <param name="rawToken">The JSON Web Token</param>
        /// <returns>The <see cref="JsonWebToken"/>.</returns>
        public static JsonWebToken Parse(string rawToken)
        {
            // The JWT MUST contain two period characters. The JWT MUST be split on the two period characters resulting in three segment strings. 
            // The first segment is the JWT Header Segment; the second is the JWT Claim Segment; the third is the JWT Crypto Segment. 
            if (string.IsNullOrEmpty(rawToken))
            {
                throw Fx.Exception.ArgumentNullOrEmpty("rawToken");
            }

            JsonWebToken token;
            string errorMessage;
            if (!TryParseInternal(rawToken, out token, out errorMessage))
            {
                throw Fx.Exception.Argument("rawToken", errorMessage);
            }

            return token;
        }

        /// <summary>
        /// Try to parse a JsonWebToken from the specified string representation. No signature verification is performed. 
        /// </summary>
        /// <param name="rawToken">The raw token string to be parsed</param>
        /// <param name="token">The JSON Web Token object parsed from the input</param>
        /// <returns>Boolean result to indicate whether the parsing has succeeded.</returns>
        public static bool TryParse(string rawToken, out JsonWebToken token)
        {
            string errorMessage;
            return TryParseInternal(rawToken, out token, out errorMessage);
        }

        /// <summary>
        /// Creates a JsonWebToken from the specified string representation and validates the signature.
        /// </summary>
        /// <param name="rawToken">The JSON Web Token</param>
        /// <param name="key">The base64 encoded key used by the cryptographic algorithm to validate the signature.</param>
        /// <remarks>The JSON web token is expected to be signed with an HMAC with SHA-256 cryptographic algorithm. An exception is thrown otherwise.</remarks>
        /// <returns>The validated <see cref="JsonWebToken"/>.</returns>
        public static JsonWebToken ParseAndAuthenticate(string rawToken, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw Fx.Exception.ArgumentNullOrEmpty("key");
            }

            return ParseAndAuthenticate(rawToken, Convert.FromBase64String(key));
        }

        /// <summary>
        /// Creates a JsonWebToken from the specified string representation and validates the signature.
        /// </summary>
        /// <param name="rawToken">The JSON Web Token</param>
        /// <param name="key">The key used by the cryptographic algorithm to validate the signature.</param>
        /// <remarks>The JSON web token is expected to be signed with an HMAC with SHA-256 cryptographic algorithm. An exception is thrown otherwise.</remarks>
        /// <returns>The validated <see cref="JsonWebToken"/>.</returns>
        public static JsonWebToken ParseAndAuthenticate(string rawToken, byte[] key)
        {
            if ((key == null) || (key.Length == 0))
            {
                throw Fx.Exception.ArgumentNullOrEmpty("key");
            }

            JsonWebToken result = Parse(rawToken);

            // The expectation is for the token to be signed; if the token uses no algorithm (and hence no signature), 
            // we consider that authentication failed (otherwise authentication is bypassed by not signing the token).
            string algorithm = result.Algorithm;
            if (algorithm == JsonWebTokenAlgorithms.None)
            {
                Fx.Assert(string.IsNullOrEmpty(result.signature), "The Parse method should have validated that signature is an empty string when the token's algorithm is 'none'.");
                throw Fx.Exception.AsError(new SecurityException(SR.JsonWebToken_InvalidSignature));
            }

            result.VerifySignature(key);
            return result;
        }

        /// <summary>
        /// Creates a JsonWebToken from the specified string representation and validates the signature.
        /// </summary>
        /// <param name="rawToken">The JSON Web Token</param>
        /// <param name="certificate">The X.509 certificate used by the cryptographic algorithm to validate the signature.</param>
        /// <remarks>The JSON web token is expected to be signed with a RSA with SHA-256 cryptographic algorithm. An exception is thrown otherwise.</remarks>
        /// <returns>The validated <see cref="JsonWebToken"/>.</returns>
        public static JsonWebToken ParseAndAuthenticate(string rawToken, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw Fx.Exception.ArgumentNull("certificate");
            }

            JsonWebToken result = Parse(rawToken);

            // The expectation is for the token to be signed; if the token uses no algorithm (and hence no signature), 
            // we consider that authentication failed (otherwise authentication is bypassed by not signing the token).
            string algorithm = result.Algorithm;
            if (algorithm == JsonWebTokenAlgorithms.None)
            {
                Fx.Assert(string.IsNullOrEmpty(result.signature), "The Parse method should have validated that signature is an empty string when the token's algorithm is 'none'.");
                throw Fx.Exception.AsError(new SecurityException(SR.JsonWebToken_InvalidSignature));
            }

            result.VerifySignature(certificate);
            return result;
        }
        
        /// <summary>
        /// Verify the signature of the JSON web token given a key blob
        /// </summary>
        /// <param name="key">The key used by the cryptographic algorithm to validate the signature.</param>
        public void VerifySignature(byte[] key)
        {
            // Verify the signature.
            using (HMAC cipher = JsonWebTokenAlgorithms.GetHmac(this.Algorithm, key))
            {
                if (cipher == null)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForAuthentication)));
                }

                byte[] data = Encoding.UTF8.GetBytes(string.Concat(this.headerSegment, ".", this.claimSegment));
                string authenticatingSignature = EncodeBase64UrlWithNoPadding(cipher.ComputeHash(data));
                if (!authenticatingSignature.Equals(this.signature))
                {
                    throw Fx.Exception.AsError(new SecurityException(SR.JsonWebToken_InvalidSignature));
                }
            }
        }

        /// <summary>
        /// Verify the signature of the JSON web token given a certificate
        /// </summary>
        /// <param name="certificate">The X.509 certificate used by the cryptographic algorithm to validate the signature.</param>
        public void VerifySignature(X509Certificate2 certificate)
        {
            byte[] data = Encoding.UTF8.GetBytes(string.Concat(this.headerSegment, ".", this.claimSegment));
            byte[] signature = DecodeBase64UrlWithNoPadding(this.signature);

            RSACryptoServiceProvider rsa = certificate.PublicKey.Key as RSACryptoServiceProvider;
            if (rsa == null)
            {
                throw Fx.Exception.Argument("certificate", string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForAuthentication));
            }

            // Prepare the hash algorithm to use with the RSACryptoServiceProvider.
            string hashAlgorithm = JsonWebTokenAlgorithms.GetHashAlgorithmForRsa(this.Algorithm);
            if (hashAlgorithm == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForAuthentication)));
            }

            // Verify the signature.
            if (!rsa.VerifyData(data, hashAlgorithm, signature))
            {
                throw Fx.Exception.AsError(new SecurityException(SR.JsonWebToken_InvalidSignature));
            }
        }

        /// <summary>
        /// Provides the string representation of the JSON Web Token.
        /// </summary>
        /// <returns>The encoded JSON web token.</returns>
        public string Serialize()
        {
            // The algorithm header value is case sensitive.
            if (this.Algorithm == JsonWebTokenAlgorithms.None)
            {
                this.signature = string.Empty;
            }
            else if (string.IsNullOrWhiteSpace(this.signature))
            {
                throw Fx.Exception.AsError(new InvalidOperationException(string.Format(SR.Culture, SR.JsonWebToken_SignatureRequired)));
            }

            return string.Concat(this.GetHeaderSegment(), ".", this.GetClaimSegment(), ".", this.signature);
        }

        /// <summary>
        /// Signs the JSON web token using the specified symmetric key and the HMAC cryptographic algorithm specified by the Algorithm property.
        /// </summary>
        /// <param name="key">Base 64 encoded symmetric key value.</param>
        /// <returns>The encoded JSON web token.</returns>
        public string Sign(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw Fx.Exception.ArgumentNullOrEmpty("key");
            }

            return this.Sign(Convert.FromBase64String(key));
        }

        /// <summary>
        /// Signs the JSON web token using the specified symmetric key and the HMAC cryptographic algorithm specified by the Algorithm property.
        /// </summary>
        /// <param name="key">The symmetric key value.</param>
        /// <returns>The encoded JSON web token.</returns>
        public string Sign(byte[] key)
        {
            if ((key == null) || (key.Length == 0))
            {
                throw Fx.Exception.ArgumentNullOrEmpty("key");
            }

            using (HMAC cipher = JsonWebTokenAlgorithms.GetHmac(this.Algorithm, key))
            {
                if (cipher == null)
                {
                    throw Fx.Exception.AsError(new InvalidOperationException(string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForSigning)));
                }

                string signingInput = string.Concat(this.GetHeaderSegment(), ".", this.GetClaimSegment());
                this.signature = EncodeBase64UrlWithNoPadding(cipher.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
                return string.Concat(signingInput, ".", this.signature);
            }
        }

        /// <summary>
        /// Signs the JSON web token using the key from the specified certificate and the RSA cryptographic algorithm specified by the Algorithm property.
        /// </summary>
        /// <param name="certificate">The X.509 certificate used to sign the token.</param>
        /// <returns>The encoded JSON web token.</returns>
        public string Sign(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw Fx.Exception.ArgumentNull("certificate");
            }
            else if (!certificate.HasPrivateKey)
            {
                throw Fx.Exception.Argument("certificate", string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForSigning));
            }

            RSACryptoServiceProvider rsa = certificate.PrivateKey as RSACryptoServiceProvider;
            if (rsa == null)
            {
                throw Fx.Exception.Argument("certificate", string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForSigning));
            }

            // Prepare the hash algorithm to use with the RSACryptoServiceProvider.
            string hashAlgorithm = JsonWebTokenAlgorithms.GetHashAlgorithmForRsa(this.Algorithm);
            if (hashAlgorithm == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(string.Format(SR.Culture, SR.JsonWebToken_InvalidKeyOrCertificateForSigning)));
            }

            // Add the "x5t" thumbprint header when signing using an X.509 certificate as defined by http://self-issued.info/docs/draft-jones-json-web-encryption.html
            this.headers[X509ThumbprintHeader] = EncodeBase64UrlWithNoPadding(StringToByteArray(certificate.Thumbprint));

            string signingInput = string.Concat(this.GetHeaderSegment(), ".", this.GetClaimSegment());
            byte[] data = Encoding.UTF8.GetBytes(signingInput);
            const int PROV_RSA_AES = 24;    // CryptoApi provider type for an RSA provider supporting the SHA digital signatures we use

            if (rsa.CspKeyContainerInfo.ProviderType == PROV_RSA_AES)
            {
                this.signature = EncodeBase64UrlWithNoPadding(rsa.SignData(data, hashAlgorithm));
            }
            else
            {
                CspParameters csp = new CspParameters();
                csp.ProviderType = PROV_RSA_AES;
                csp.KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName;
                csp.KeyNumber = (int)rsa.CspKeyContainerInfo.KeyNumber;
                if (rsa.CspKeyContainerInfo.MachineKeyStore)
                {
                    csp.Flags = CspProviderFlags.UseMachineKeyStore;
                }

                using (RSACryptoServiceProvider rsaProxy = new RSACryptoServiceProvider(csp))
                {
                    this.signature = EncodeBase64UrlWithNoPadding(rsaProxy.SignData(data, hashAlgorithm));
                }
            }

            return string.Concat(signingInput, ".", this.signature);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents an un-encoded representation of the JSON web token.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents an un-encoded representation of the JSON web token.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Actor))
            {
                return string.Concat(this.Headers, Environment.NewLine, this.Claims);
            }
            else
            {
                try
                {
                    string actorClaimPlaceholder = Guid.NewGuid().ToString();

                    JsonWebToken clone = JsonWebToken.Parse(this.Serialize());
                    clone.Actor = null;

                    JsonWebToken actor = JsonWebToken.Parse(this.Actor);
                    JsonObject actorObject = new JsonObject();
                    actorObject.Add("__headers", actor.Headers);
                    actorObject.Add("__claims", actor.Claims);
                    clone.Claims.Add(actorClaimPlaceholder, actorObject);

                    return clone.ToString().Replace(actorClaimPlaceholder, ActorClaim);
                }
                catch
                {
                    // this typically means the actor token is not a valid JWT
                    return string.Concat(this.Headers, Environment.NewLine, this.Claims);
                }
            }
        }

        /// <summary>
        /// Converts a string-encoded hex value (e.g. an X.509 Certificate thumbprint) into a byte array.
        /// </summary>
        /// <param name="str">The string to be converted to a byte array.</param>
        /// <returns>Return the byte array corresponding to the input string.</returns>
        private static byte[] StringToByteArray(string str)
        {
            Fx.Assert(str.Length % 2 == 0, "Only even length-strings are expected.");

            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < str.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            }

            return bytes;
        }

        /// <summary>
        /// Encodes the specified bytes as a base64url string (as defined in RFC4648), with no padding characters ('=').
        /// </summary>
        /// <param name="arg">The bytes to encode as a base64url string with no padding.</param>
        /// <returns>Returns the encoded base64url string, with no padding.</returns>
        private static string EncodeBase64UrlWithNoPadding(byte[] arg)
        {
            // The base64url encoding is a variation of base64 encoding, described at:
            // http://en.wikipedia.org/wiki/Base64#Variants_summary_table
            string s = Convert.ToBase64String(arg); // standard base64 encoder
            s = s.TrimEnd('='); // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        /// <summary>
        /// Decodes the specified base64url string (as defined in RFC4648) that has no padding characters ('=').
        /// </summary>
        /// <param name="arg">The base64url string that has no padding.</param>
        /// <returns>Returns the bytes decoded from the base64url string with no padding.</returns>
        private static byte[] DecodeBase64UrlWithNoPadding(string arg)
        {
            // The base64url encoding is a variation of base64 encoding, described at:
            // http://en.wikipedia.org/wiki/Base64#Variants_summary_table
            arg = arg.Replace('-', '+'); // 62nd char of encoding
            arg = arg.Replace('_', '/'); // 63rd char of encoding
            // Pad with trailing '='s
            switch (arg.Length % 4)
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    arg += "==";
                    break; // Two pad chars
                case 3:
                    arg += "=";
                    break; // One pad char
                default:
                    throw Fx.Exception.AsError(new ArgumentException(string.Format(SR.Culture, SR.JsonWebToken_InvalidTokenFormat)));
            }

            return Convert.FromBase64String(arg); // standard base64 decoder
        }

        private static bool TryDeserialize(string jsonString, out JsonObject jsonObject)
        {
            try
            {
                jsonObject = JsonValue.Parse(jsonString) as JsonObject;
            }
            catch (ArgumentException)
            {
                jsonObject = null;
            }

            return jsonObject != null;
        }

        /// <summary>
        /// TryParseInternal parses the rawToken.
        /// </summary>
        /// <param name="rawToken">The rawToken to be parsed</param>
        /// <param name="token">The returned JsonWebToken</param>
        /// <param name="errorMessage">The error message due to bad argument of the rawToken</param>
        /// <returns>Boolean result to indicate whether the parsing has succeeded.</returns>
        private static bool TryParseInternal(string rawToken, out JsonWebToken token, out string errorMessage)
        {
            token = null;
            errorMessage = null;

            // The JWT MUST contain two period characters. The JWT MUST be split on the two period characters resulting in three segment strings. 
            // The first segment is the JWT Header Segment; the second is the JWT Claim Segment; the third is the JWT Crypto Segment. 
            if (string.IsNullOrEmpty(rawToken))
            {
                return false;
            }

            string[] segments = rawToken.Split('.');
            if (segments.Length != 3)
            {
                errorMessage = string.Format(SR.Culture, SR.JsonWebToken_InvalidTokenFormat);
                return false;
            }

            JsonWebToken result = new JsonWebToken();
            result.headers.Clear();
            result.claims.Clear();
            result.headerSegment = segments[0];
            result.claimSegment = segments[1];
            result.signature = segments[2];

            // The JWT Header Segment MUST be successfully base64url decoded following the restriction that no padding characters have been used.
            if (!TryDeserialize(Encoding.UTF8.GetString(DecodeBase64UrlWithNoPadding(result.headerSegment)), out result.headers))
            {
                errorMessage = string.Format(SR.Culture, SR.JsonWebToken_InvalidTokenFormat);
                return false;
            }

            // The "alg" (algorithm) header is required and its value is case sensitive. If it's "none", the JWT Crypto Segment MUST be 
            // the empty string, otherwise the JWT Crypto Segment MUST be non-empty.
            if (string.IsNullOrWhiteSpace(result.Algorithm))
            {
                errorMessage = string.Format(SR.Culture, SR.JsonWebToken_AlgorithmHeaderIsRequired);
                return false;
            }
            else if ((result.Algorithm == JsonWebTokenAlgorithms.None) ^ string.IsNullOrEmpty(result.signature))
            {
                errorMessage = string.Format(SR.Culture, SR.JsonWebToken_InvalidTokenFormat);
                return false;
            }

            // The JWT Claim Segment MUST be successfully base64url decoded following the restriction that no padding characters have been used.
            if (!TryDeserialize(Encoding.UTF8.GetString(DecodeBase64UrlWithNoPadding(result.claimSegment)), out result.claims))
            {
                errorMessage = string.Format(SR.Culture, SR.JsonWebToken_InvalidTokenFormat);
                return false;
            }

            token = result;
            return true;
        }

        /// <summary>
        /// Returns the JWT Header Segment.
        /// </summary>
        /// <remarks>
        /// A JWT Token Segment containing a base64url encoded JSON object that describes the cryptographic operations applied to the JWT Header Segment and 
        /// the JWT Claim Segment. The JWT Header Segment is the JWS Header Input for creating a JSON Web Signature for the JWT.
        /// </remarks>
        /// <returns>The JWT header segment.</returns>
        private string GetHeaderSegment()
        {
            // headers -> JSON string -> UTF8 bytes -> base64url encoding with no padding
            return EncodeBase64UrlWithNoPadding(Encoding.UTF8.GetBytes(this.headers.ToString(JsonSaveOptions.None)));
        }

        /// <summary>
        /// Returns the JWT Claim Segment.
        /// </summary>
        /// <remarks>
        /// A JWT Token Segment containing a base64url encoded JSON object that encodes the claims contained in the JWT. The JWT Claim Segment is the JWS Payload Input 
        /// for creating a JSON Web Signature for the JWT. 
        /// </remarks>
        /// <returns>The JWT Claims Segment.</returns>
        private string GetClaimSegment()
        {
            // claims -> JSON string -> UTF8 bytes -> base64url encoding with no padding
            return EncodeBase64UrlWithNoPadding(Encoding.UTF8.GetBytes(this.claims.ToString(JsonSaveOptions.None)));
        }

        private DateTime GetDateTimeClaim(string claimName)
        {
            JsonValue value;
            return this.claims.TryGetValue(claimName, out value) && (value != null) ? epochTimeBase.AddSeconds(value.ReadAs<double>()) : DateTime.MinValue;
        }

        private void SetDateTimeClaim(string claimName, DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                this.claims.Remove(claimName);
            }
            else
            {
                this.claims[claimName] = value.Subtract(epochTimeBase).TotalSeconds;
            }
        }

        private string GetStringClaim(string claimName)
        {
            JsonValue value;
            return this.claims.TryGetValue(claimName, out value) && (value != null) ? value.ReadAs<string>() : null;
        }

        private void SetStringClaim(string claimName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                this.claims.Remove(claimName);
            }
            else
            {
                this.claims[claimName] = value;
            }
        }

        /// <summary>
        /// Defines algorithms used to sign the contents of the JSON Web Token (JWT) Header and Claim Segments, to produce the Crypto Segment value.
        /// </summary>
        private static class JsonWebTokenAlgorithms
        {
            /// <summary>
            /// The JSON Web Token (JWT) is not signed.
            /// </summary>
            public const string None = "none";

            /// <summary>
            /// HMAC using SHA-256 hash algorithm.
            /// </summary>
            public const string HmacSha256 = "HS256";

            /// <summary>
            /// HMAC using SHA-384 hash algorithm.
            /// </summary>
            public const string HmacSha384 = "HS384";

            /// <summary>
            /// HMAC using SHA-512 hash algorithm.
            /// </summary>
            public const string HmacSha512 = "HS512";

            /// <summary>
            /// RSA using SHA-256 hash algorithm.
            /// </summary>
            public const string RsaSha256 = "RS256";

            /// <summary>
            /// RSA using SHA-384 hash algorithm.
            /// </summary>
            public const string RsaSha384 = "RS384";

            /// <summary>
            /// RSA using SHA-512 hash algorithm.
            /// </summary>
            public const string RsaSha512 = "RS512";

            /// <summary>
            /// ECDSA using P-256 curve and SHA-256 hash algorithm.
            /// </summary>
            public const string EcdsaP256Sha256 = "ES256";

            /// <summary>
            /// ECDSA using P-384 curve and SHA-384 hash algorithm.
            /// </summary>
            public const string EcdsaP384Sha384 = "ES384";

            /// <summary>
            /// ECDSA using P-512 curve and SHA-512 hash algorithm.
            /// </summary>
            public const string EcdsaP512Sha512 = "ES512";

            internal static string GetHashAlgorithmForRsa(string algorithm)
            {
                switch (algorithm)
                {
                    case JsonWebTokenAlgorithms.RsaSha256: return "SHA256";
                    case JsonWebTokenAlgorithms.RsaSha384: return "SHA384";
                    case JsonWebTokenAlgorithms.RsaSha512: return "SHA512";
                    default: return null;
                }
            }

            internal static HMAC GetHmac(string algorithm, byte[] key)
            {
                switch (algorithm)
                {
                    case JsonWebTokenAlgorithms.HmacSha256: return new HMACSHA256(key);
                    case JsonWebTokenAlgorithms.HmacSha384: return new HMACSHA384(key);
                    case JsonWebTokenAlgorithms.HmacSha512: return new HMACSHA512(key);
                    default: return null;
                }
            }
        }
    }
}