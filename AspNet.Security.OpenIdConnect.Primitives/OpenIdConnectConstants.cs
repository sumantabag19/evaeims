﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

namespace AspNet.Security.OpenIdConnect.Primitives
{
    public static class OpenIdConnectConstants
    {
        public static class Algorithms
        {
            public const string EcdsaSha256 = "ES256";
            public const string EcdsaSha384 = "ES384";
            public const string EcdsaSha512 = "ES512";
            public const string HmacSha256 = "HS256";
            public const string HmacSha384 = "HS384";
            public const string HmacSha512 = "HS512";
            public const string RsaSha256 = "RS256";
            public const string RsaSha384 = "RS384";
            public const string RsaSha512 = "RS512";
            public const string RsaSsaPssSha256 = "PS256";
            public const string RsaSsaPssSha384 = "PS384";
            public const string RsaSsaPssSha512 = "PS512";
        }

        public static class Claims
        {
            public const string AccessTokenHash = "at_hash";
            public const string Active = "active";
            public const string Address = "address";
            public const string Audience = "aud";
            public const string AuthenticationContextReference = "acr";
            public const string AuthenticationMethodReference = "amr";
            public const string AuthenticationTime = "auth_time";
            public const string AuthorizedParty = "azp";
            public const string Birthdate = "birthdate";
            public const string ClientId = "client_id";
            public const string CodeHash = "c_hash";
            public const string ConfidentialityLevel = "cfd_lvl";
            public const string Country = "country";
            public const string Email = "email";
            public const string EmailVerified = "email_verified";
            public const string ExpiresAt = "exp";
            public const string FamilyName = "family_name";
            public const string Formatted = "formatted";
            public const string Gender = "gender";
            public const string GivenName = "given_name";
            public const string IssuedAt = "iat";
            public const string Issuer = "iss";
            public const string Locale = "locale";
            public const string Locality = "locality";
            public const string JwtId = "jti";
            public const string KeyId = "kid";
            public const string MiddleName = "middle_name";
            public const string Name = "name";
            public const string Nickname = "nickname";
            public const string Nonce = "nonce";
            public const string NotBefore = "nbf";
            public const string PhoneNumber = "phone_number";
            public const string PhoneNumberVerified = "phone_number_verified";
            public const string Picture = "picture";
            public const string PostalCode = "postal_code";
            public const string PreferredUsername = "preferred_username";
            public const string Profile = "profile";
            public const string Region = "region";
            public const string Role = "role";
            public const string Scope = "scope";
            public const string StreetAddress = "street_address";
            public const string Subject = "sub";
            public const string TokenType = "token_type";
            public const string TokenUsage = "token_usage";
            public const string UpdatedAt = "updated_at";
            public const string Username = "username";
            public const string Website = "website";
            public const string Zoneinfo = "zoneinfo";
        }

        public static class ClientAuthenticationMethods
        {
            public const string ClientSecretBasic = "client_secret_basic";
            public const string ClientSecretPost = "client_secret_post";
        }

        public static class CodeChallengeMethods
        {
            public const string Plain = "plain";
            public const string Sha256 = "S256";
        }

        public static class ConfidentialityLevels
        {
            public const string Private = "private";
            public const string Public = "public";
        }

        public static class Destinations
        {
            public const string AccessToken = "access_token";
            public const string IdentityToken = "id_token";
        }

        public static class Errors
        {
            public const string AccessDenied = "access_denied";
            public const string AccountSelectionRequired = "account_selection_required";
            public const string ConsentRequired = "consent_required";
            public const string InteractionRequired = "interaction_required";
            public const string InvalidClient = "invalid_client";
            public const string InvalidGrant = "invalid_grant";
            public const string InvalidRequest = "invalid_request";
            public const string InvalidRequestObject = "invalid_request_object";
            public const string InvalidRequestUri = "invalid_request_uri";
            public const string InvalidScope = "invalid_scope";
            public const string InvalidToken = "invalid_token";
            public const string LoginRequired = "login_required";
            public const string RegistrationNotSupported = "registration_not_supported";
            public const string RequestNotSupported = "request_not_supported";
            public const string RequestUriNotSupported = "request_uri_not_supported";
            public const string ServerError = "server_error";
            public const string TemporarilyUnavailable = "temporarily_unavailable";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string UnsupportedGrantType = "unsupported_grant_type";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string UnsupportedTokenType = "unsupported_token_type";
        }

        public static class GrantTypes
        {
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string Implicit = "implicit";
            public const string Password = "password";
            public const string RefreshToken = "refresh_token";
        }

        public static class MessageTypes
        {
            public const string AuthorizationRequest = "authorization_request";
            public const string AuthorizationResponse = "authorization_response";
            public const string ConfigurationRequest = "configuration_request";
            public const string ConfigurationResponse = "configuration_response";
            public const string CryptographyRequest = "cryptography_request";
            public const string CryptographyResponse = "cryptography_response";
            public const string IntrospectionRequest = "introspection_request";
            public const string IntrospectionResponse = "introspection_response";
            public const string LogoutRequest = "logout_request";
            public const string LogoutResponse = "logout_response";
            public const string RevocationRequest = "revocation_request";
            public const string RevocationResponse = "revocation_response";
            public const string TokenRequest = "token_request";
            public const string TokenResponse = "token_response";
            public const string UserinfoRequest = "userinfo_request";
            public const string UserinfoResponse = "userinfo_response";
        }

        public static class Metadata
        {
            public const string AcrValuesSupported = "acr_values_supported";
            public const string AuthorizationEndpoint = "authorization_endpoint";
            public const string ClaimsLocalesSupported = "claims_locales_supported";
            public const string ClaimsParameterSupported = "claims_parameter_supported";
            public const string ClaimsSupported = "claims_supported";
            public const string ClaimTypesSupported = "claim_types_supported";
            public const string CodeChallengeMethodsSupported = "code_challenge_methods_supported";
            public const string DisplayValuesSupported = "display_values_supported";
            public const string EndSessionEndpoint = "end_session_endpoint";
            public const string GrantTypesSupported = "grant_types_supported";
            public const string IdTokenEncryptionAlgValuesSupported = "id_token_encryption_alg_values_supported";
            public const string IdTokenEncryptionEncValuesSupported = "id_token_encryption_enc_values_supported";
            public const string IdTokenSigningAlgValuesSupported = "id_token_signing_alg_values_supported";
            public const string IntrospectionEndpoint = "introspection_endpoint";
            public const string IntrospectionEndpointAuthMethodsSupported = "introspection_endpoint_auth_methods_supported";
            public const string IntrospectionEndpointAuthSigningAlgValuesSupported = "introspection_endpoint_auth_signing_alg_values_supported";
            public const string Issuer = "issuer";
            public const string JwksUri = "jwks_uri";
            public const string OpPolicyUri = "op_policy_uri";
            public const string OpTosUri = "op_tos_uri";
            public const string RequestObjectEncryptionAlgValuesSupported = "request_object_encryption_alg_values_supported";
            public const string RequestObjectEncryptionEncValuesSupported = "request_object_encryption_enc_values_supported";
            public const string RequestObjectSigningAlgValuesSupported = "request_object_signing_alg_values_supported";
            public const string RequestParameterSupported = "request_parameter_supported";
            public const string RequestUriParameterSupported = "request_uri_parameter_supported";
            public const string RequireRequestUriRegistration = "require_request_uri_registration";
            public const string ResponseModesSupported = "response_modes_supported";
            public const string ResponseTypesSupported = "response_types_supported";
            public const string RevocationEndpoint = "revocation_endpoint";
            public const string RevocationEndpointAuthMethodsSupported = "revocation_endpoint_auth_methods_supported";
            public const string RevocationEndpointAuthSigningAlgValuesSupported = "revocation_endpoint_auth_signing_alg_values_supported";
            public const string ScopesSupported = "scopes_supported";
            public const string ServiceDocumentation = "service_documentation";
            public const string SubjectTypesSupported = "subject_types_supported";
            public const string TokenEndpoint = "token_endpoint";
            public const string TokenEndpointAuthMethodsSupported = "token_endpoint_auth_methods_supported";
            public const string TokenEndpointAuthSigningAlgValuesSupported = "token_endpoint_auth_signing_alg_values_supported";
            public const string UiLocalesSupported = "ui_locales_supported";
            public const string UserinfoEncryptionAlgValuesSupported = "userinfo_encryption_alg_values_supported";
            public const string UserinfoEncryptionEncValuesSupported = "userinfo_encryption_enc_values_supported";
            public const string UserinfoEndpoint = "userinfo_endpoint";
            public const string UserinfoSigningAlgValuesSupported = "userinfo_signing_alg_values_supported";
        }

        public static class Parameters
        {
            public const string AccessToken = "access_token";
            public const string Active = "active";
            public const string AcrValues = "acr_values";
            public const string Assertion = "assertion";
            public const string Audience = "audience";
            public const string Claims = "claims";
            public const string ClaimsLocales = "claims_locales";
            public const string ClientAssertion = "client_assertion";
            public const string ClientAssertionType = "client_assertion_type";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
            public const string Code = "code";
            public const string CodeChallenge = "code_challenge";
            public const string CodeChallengeMethod = "code_challenge_method";
            public const string CodeVerifier = "code_verifier";
            public const string Display = "display";
            public const string Error = "error";
            public const string ErrorDescription = "error_description";
            public const string ErrorUri = "error_uri";
            public const string ExpiresIn = "expires_in";
            public const string GrantType = "grant_type";
            public const string IdentityProvider = "identity_provider";
            public const string IdToken = "id_token";
            public const string IdTokenHint = "id_token_hint";
            public const string LoginHint = "login_hint";
            public const string Keys = "keys";
            public const string MaxAge = "max_age";
            public const string Nonce = "nonce";
            public const string Password = "password";
            public const string PostLogoutRedirectUri = "post_logout_redirect_uri";
            public const string Prompt = "prompt";
            public const string Realm = "realm";
            public const string RedirectUri = "redirect_uri";
            public const string RefreshToken = "refresh_token";
            public const string Registration = "registration";
            public const string Request = "request";
            public const string RequestId = "request_id";
            public const string RequestUri = "request_uri";
            public const string Resource = "resource";
            public const string ResponseMode = "response_mode";
            public const string ResponseType = "response_type";
            public const string Scope = "scope";
            public const string State = "state";
            public const string Token = "token";
            public const string TokenType = "token_type";
            public const string TokenTypeHint = "token_type_hint";
            public const string UiLocales = "ui_locales";
            public const string Username = "username";
        }

        public static class Prompts
        {
            public const string Consent = "consent";
            public const string Login = "login";
            public const string None = "none";
            public const string SelectAccount = "select_account";
        }

        public static class Properties
        {
            public const string AccessTokenLifetime = ".access_token_lifetime";
            public const string AuthorizationCodeLifetime = ".authorization_code_lifetime";
            public const string Audiences = ".audiences";
            public const string CodeChallenge = ".code_challenge";
            public const string CodeChallengeMethod = ".code_challenge_method";
            public const string ConfidentialityLevel = ".confidentiality_level";
            public const string Destinations = ".destinations";
            public const string Error = ".error";
            public const string ErrorDescription = ".error_description";
            public const string ErrorUri = ".error_uri";
            public const string IdentityTokenLifetime = ".identity_token_lifetime";
            public const string MessageType = ".message_type";
            public const string Nonce = ".nonce";
            public const string OriginalRedirectUri = ".original_redirect_uri";
            public const string PostLogoutRedirectUri = ".post_logout_redirect_uri";
            public const string Presenters = ".presenters";
            public const string RefreshTokenLifetime = ".refresh_token_lifetime";
            public const string Resources = ".resources";
            public const string Scopes = ".scopes";
            public const string TokenId = ".token_id";
            public const string TokenUsage = ".token_usage";
            public const string ValidatedClientId = ".validated_client_id";
            public const string ValidatedRedirectUri = ".validated_redirect_uri";
        }

        public static class ResponseModes
        {
            public const string FormPost = "form_post";
            public const string Fragment = "fragment";
            public const string Query = "query";
        }

        public static class ResponseTypes
        {
            public const string Code = "code";
            public const string IdToken = "id_token";
            public const string None = "none";
            public const string Token = "token";
        }

        public static class Separators
        {
            public static readonly char[] Ampersand = { '&' };
            public static readonly char[] Space = { ' ' };
        }

        public static class Schemes
        {
            public const string Basic = "Basic";
            public const string Bearer = "Bearer";
        }

        public static class Scopes
        {
            public const string Address = "address";
            public const string Email = "email";
            public const string OfflineAccess = "offline_access";
            public const string OpenId = "openid";
            public const string Phone = "phone";
            public const string Profile = "profile";
        }

        public static class SubjectTypes
        {
            public const string Pairwise = "pairwise";
            public const string Public = "public";
        }

        public static class TokenTypeHints
        {
            public const string AccessToken = "access_token";
            public const string AuthorizationCode = "authorization_code";
            public const string IdToken = "id_token";
            public const string RefreshToken = "refresh_token";
        }

        public static class TokenTypes
        {
            public const string Bearer = "Bearer";
        }

        public static class TokenUsages
        {
            public const string AccessToken = "access_token";
            public const string AuthorizationCode = "authorization_code";
            public const string IdToken = "id_token";
            public const string RefreshToken = "refresh_token";
        }
    }
}
