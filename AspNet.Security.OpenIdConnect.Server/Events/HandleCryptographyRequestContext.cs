/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using System.Collections.Generic;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AspNet.Security.OpenIdConnect.Server
{
    /// <summary>
    /// Represents the context class associated with the
    /// <see cref="OpenIdConnectServerProvider.HandleCryptographyRequest"/> event.
    /// </summary>
    public class HandleCryptographyRequestContext : BaseValidatingContext
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HandleCryptographyRequestContext"/> class.
        /// </summary>
        public HandleCryptographyRequestContext(
            HttpContext context,
            AuthenticationScheme scheme,
            OpenIdConnectServerOptions options,
            OpenIdConnectRequest request)
            : base(context, scheme, options, request)
        {
            Validate();
        }

        /// <summary>
        /// Gets the list of JSON Web Keys exposed by the JWKS endpoint.
        /// </summary>
        public IList<JsonWebKey> Keys { get; } = new List<JsonWebKey>();
    }
}
