using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Horseshoe.NET.Http;
using System.Net;

namespace Horseshoe.NET.Jwt
{
    public static class Extensions
    {
        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="authenticationBuilder">An instance of <c>AuthenticationBuilder</c></param>
        /// <param name="tokenKey">A token key</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <param name="encoding">The text encoding to be applied to <c>tokenKey</c></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtAuthentication
        (
            this AuthenticationBuilder authenticationBuilder,
            string tokenKey,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true,
            Encoding encoding = null
        )
        {
            return AddJwtAuthentication(authenticationBuilder, (encoding ?? Encoding.Default).GetBytes(tokenKey), authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="authenticationBuilder">An instance of <c>AuthenticationBuilder</c></param>
        /// <param name="tokenKeyBytes">The token key (for signature validation)</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddJwtAuthentication
        (
            this AuthenticationBuilder authenticationBuilder,
            byte[] tokenKeyBytes,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true
        )
        {
            authenticationBuilder
                .AddJwtBearer(authenticationScheme, opt =>
                {
                    opt.SaveToken = saveToken;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = validAudience,
                        ValidAudiences = validAudiences,
                        ValidateAudience = validAudience != null || validAudiences != null,
                        ValidIssuer = validIssuer,
                        ValidIssuers = validIssuers,
                        ValidateIssuer = validIssuer != null || validIssuers != null,
                        IssuerSigningKey = tokenKeyBytes != null ? new SymmetricSecurityKey(tokenKeyBytes) : null,
                        ValidateIssuerSigningKey = tokenKeyBytes != null,
                        RequireExpirationTime = requireExpirationTime
                    };
                });
            return authenticationBuilder;
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKey">The token key (for signature validation)</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication
        (
            this IServiceCollection services,
            string tokenKey,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true,
            Encoding encoding = null
        )
        {
            return AddJwtAuthentication(services, (encoding ?? Encoding.Default).GetBytes(tokenKey), authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKeyBytes">The token key (for signature validation)</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication
        (
            this IServiceCollection services,
            byte[] tokenKeyBytes,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true
        )
        {
            services
                .AddAuthentication()
                .AddJwtAuthentication(tokenKeyBytes, authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
            return services;
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKey">The token key (for signature validation)</param>
        /// <param name="policyName">The policy name</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationAndAuthorizationPolicy
        (
            this IServiceCollection services,
            string tokenKey,
            string policyName,
            string[] requiredRoles = null,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true,
            Encoding encoding = null
        )
        {
            AddJwtAuthentication(services, (encoding ?? Encoding.Default).GetBytes(tokenKey), authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
            AddAuthorizationPolicy(services, policyName, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKeyBytes">The token key (for signature validation)</param>
        /// <param name="policyName">The policy name</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationAndAuthorizationPolicy
        (
            this IServiceCollection services,
            byte[] tokenKeyBytes,
            string policyName,
            string[] requiredRoles = null,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true
        )
        {
            AddJwtAuthentication(services, tokenKeyBytes, authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
            AddAuthorizationPolicy(services, policyName, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKey">The token key (for signature validation)</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationAndDefaultAuthorizationPolicy
        (
            this IServiceCollection services,
            string tokenKey,
            string[] requiredRoles = null,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true,
            Encoding encoding = null
        )
        {
            AddJwtAuthentication(services, (encoding ?? Encoding.Default).GetBytes(tokenKey), authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
            AddDefaultAuthorizationPolicy(services, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// Use when layering authentication, otherwise use <c>services.AddJwtAuthenticationWithDefaultAuthorization()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="tokenKeyBytes">The token key (for signature validation)</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="requireExpirationTime">Indicates whether tokens must have an 'expiration' value.  Default is <c>true</c>.</param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationAndDefaultAuthorizationPolicy
        (
            this IServiceCollection services,
            byte[] tokenKeyBytes,
            string[] requiredRoles = null,
            string authenticationScheme = BearerDefaults.AuthenticationScheme,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool saveToken = false,
            bool requireExpirationTime = true
        )
        {
            AddJwtAuthentication(services, tokenKeyBytes, authenticationScheme: authenticationScheme, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, saveToken: saveToken, requireExpirationTime: requireExpirationTime);
            AddDefaultAuthorizationPolicy(services, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// See also <c>services.AddOpenIdJwtAuthenticationAndAuthorizationPolicy()</c> and
        /// <c>services.AddOpenIdJwtAuthenticationAndDefaultAuthorizationPolicy()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="metadataAddress">The discovery address for obtaining metadata</param>
        /// <param name="defaultAuthenticateScheme">An authentication scheme</param>
        /// <param name="defaultChallengeScheme">A challenge scheme</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="authority">The authority to use when making OpenID Connect calls</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="validateLifetime">Default is <c>true</c></param>
        /// <param name="validateIssuerSigningKey">Default is <c>true</c></param>
        public static IServiceCollection AddOpenIdJwtAuthentication
        (
            this IServiceCollection services,
            string metadataAddress,
            string defaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
            string defaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
            bool saveToken = false,
            string authority = null,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool validateLifetime = true,
            bool validateIssuerSigningKey = true
        )
        {
            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = defaultAuthenticateScheme;
                    opt.DefaultChallengeScheme = defaultChallengeScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.MetadataAddress = metadataAddress;
                    opt.SaveToken = saveToken;
                    opt.Authority = authority;

                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = validAudience,
                        ValidAudiences = validAudiences,
                        ValidateAudience = validAudience != null || validAudiences != null,
                        ValidIssuer = validIssuer,
                        ValidIssuers = validIssuers,
                        ValidateIssuer = validIssuer != null || validIssuers != null,
                        ValidateIssuerSigningKey = validateIssuerSigningKey,
                        ValidateLifetime = validateLifetime,
                    };
                });
            return services;
        }

        /// <summary>
        /// See also <c>services.AddOpenIdJwtAuthentication()</c> and
        /// <c>services.AddOpenIdJwtAuthenticationAndDefaultAuthorizationPolicy()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="policyName">The policy name</param>
        /// <param name="metadataAddress">The discovery address for obtaining metadata</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="defaultAuthenticateScheme">An authentication scheme</param>
        /// <param name="defaultChallengeScheme">A challenge scheme</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="authority">The authority to use when making OpenID Connect calls</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="validateLifetime">Default is <c>true</c></param>
        /// <param name="validateIssuerSigningKey">Default is <c>true</c></param>
        public static IServiceCollection AddOpenIdJwtAuthenticationAndAuthorizationPolicy
        (
            this IServiceCollection services,
            string policyName,
            string metadataAddress,
            string[] requiredRoles = null,
            string defaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
            string defaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
            bool saveToken = false,
            string authority = null,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool validateLifetime = true,
            bool validateIssuerSigningKey = true
        )
        {
            AddOpenIdJwtAuthentication(services, metadataAddress, defaultAuthenticateScheme: defaultAuthenticateScheme, defaultChallengeScheme: defaultChallengeScheme, saveToken: saveToken, authority: authority, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, validateLifetime: validateLifetime, validateIssuerSigningKey: validateIssuerSigningKey);
            AddAuthorizationPolicy(services, policyName, authenticationScheme: defaultAuthenticateScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// See also <c>services.AddOpenIdJwtAuthentication()</c> and
        /// <c>services.AddOpenIdJwtAuthenticationAndAuthorizationPolicy()</c>
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="metadataAddress">The discovery address for obtaining metadata</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <param name="defaultAuthenticateScheme">An authentication scheme</param>
        /// <param name="defaultChallengeScheme">A challenge scheme</param>
        /// <param name="saveToken">Whether the bearer token should be saved in <c>AuthenticationProperties</c></param>
        /// <param name="authority">The authority to use when making OpenID Connect calls</param>
        /// <param name="validAudience">A valid audience to be compared with the token's audience (optional)</param>
        /// <param name="validAudiences">Valid audiences to be compared with the token's audience (optional)</param>
        /// <param name="validIssuer">A valid issuer to be compared with the token's issuer (optional)</param>
        /// <param name="validIssuers">Valid issuers to be compared with the token's issuer (optional)</param>
        /// <param name="validateLifetime">Default is <c>true</c></param>
        /// <param name="validateIssuerSigningKey">Default is <c>true</c></param>
        public static IServiceCollection AddOpenIdJwtAuthenticationAndDefaultAuthorizationPolicy
        (
            this IServiceCollection services,
            string metadataAddress,
            string[] requiredRoles = null,
            string defaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
            string defaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
            bool saveToken = false,
            string authority = null,
            string validAudience = null,
            IEnumerable<string> validAudiences = null,
            string validIssuer = null,
            IEnumerable<string> validIssuers = null,
            bool validateLifetime = true,
            bool validateIssuerSigningKey = true
        )
        {
            AddOpenIdJwtAuthentication(services, metadataAddress, defaultAuthenticateScheme: defaultAuthenticateScheme, defaultChallengeScheme: defaultChallengeScheme, saveToken: saveToken, authority: authority, validAudience: validAudience, validAudiences: validAudiences, validIssuer: validIssuer, validIssuers: validIssuers, validateLifetime: validateLifetime, validateIssuerSigningKey: validateIssuerSigningKey);
            AddDefaultAuthorizationPolicy(services, authenticationScheme: defaultAuthenticateScheme, requiredRoles: requiredRoles);
            return services;
        }

        /// <summary>
        /// Adds a named authorization policy.
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="name">The policy name</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services, string name, string authenticationScheme = BearerDefaults.AuthenticationScheme, string[] requiredRoles = null)
        {
            return services.AddAuthorization
            (
                options => AddPolicy(options, name, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles)
            );
        }

        /// <summary>
        /// Adds a default authorization policy.
        /// </summary>
        /// <param name="services">An instance of <c>IServiceCollection</c></param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any supplied, to gain access.</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAuthorizationPolicy(this IServiceCollection services, string authenticationScheme = null, string[] requiredRoles = null)
        {
            return services.AddAuthorization
            (
                options => AddDefaultPolicy(options, authenticationScheme: authenticationScheme, requiredRoles: requiredRoles)
            );
        }

        /// <summary>
        /// Adds a named authorization policy.
        /// </summary>
        /// <param name="options">An instance of <c>AuthorizationOptions</c></param>
        /// <param name="name">The policy name</param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any, to gain access.</param>
        /// <returns></returns>
        public static AuthorizationOptions AddPolicy(this AuthorizationOptions options, string name, string authenticationScheme = BearerDefaults.AuthenticationScheme, string[] requiredRoles = null)
        {
            if (requiredRoles == null || !requiredRoles.Any())
            {
                options.AddPolicy(name, policy => policy
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(authenticationScheme)
                );
            }
            else
            {
                options.AddPolicy(name, policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole(requiredRoles)
                    .AddAuthenticationSchemes(authenticationScheme)
                );
            }
            return options;
        }

        /// <summary>
        /// Adds a default authorization policy.
        /// </summary>
        /// <param name="options">An instance of <c>AuthorizationOptions</c></param>
        /// <param name="authenticationScheme">An authentication scheme</param>
        /// <param name="requiredRoles">Token must contain one of these roles, if any, to gain access.</param>
        /// <returns></returns>
        public static AuthorizationOptions AddDefaultPolicy(this AuthorizationOptions options, string authenticationScheme = null, string[] requiredRoles = null)
        {
            if (requiredRoles == null || !requiredRoles.Any())
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(authenticationScheme == null ? Array.Empty<string>() : new[] { authenticationScheme })
                    .RequireAuthenticatedUser()
                    .Build();
            }
            else
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(authenticationScheme == null ? Array.Empty<string>() : new[] { authenticationScheme })
                    .RequireAuthenticatedUser()
                    .RequireRole(requiredRoles)
                    .Build();
            }
            return options;
        }

        /// <summary>
        /// Finds and parses the token in the request 'authorization' header, if applicable
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The parsed <c>Token</c></returns>
        public static AccessToken GetAuthToken(this HttpRequest request)
        {
            var authorizationHeader = request.Headers["Authorization"];
            return authorizationHeader.Count > 0
                ? TokenService.ParseToken(authorizationHeader[0])
                : null;
        }

        public static OAuthTokenEnvelope ToOAuthTokenEnvelope(this OAuthTokenEnvelope.Raw rawOAuthTokenEnvelope)
        {
            return new OAuthTokenEnvelope
            {
                AccessToken = TokenService.ParseToken(rawOAuthTokenEnvelope.access_token),
                TokenType = rawOAuthTokenEnvelope.token_type,
                ExpirationDate = DateTime.Now.AddSeconds(rawOAuthTokenEnvelope.expires_in),
                Resource = rawOAuthTokenEnvelope.resource,
                RefreshToken = rawOAuthTokenEnvelope.refresh_token,
                RefreshTokenExpirationDate = DateTime.Now.AddSeconds(rawOAuthTokenEnvelope.refresh_token_expires_in),
                Scope = rawOAuthTokenEnvelope.scope,
                IdToken = rawOAuthTokenEnvelope.id_token
            };
        }

        public static string GetFullyQualifiedUserId(this NetworkCredential cred)
        {
            if (cred.UserName == null)
                return "";
            if (cred.UserName.Contains('@') || cred.UserName.Contains('\\') || cred.UserName.Contains('/') || cred.Domain == null)
                return cred.UserName;
            if (cred.Domain.Contains('.'))
                return cred.UserName + "@" + cred.Domain;
            return cred.Domain + "\\" + cred.UserName;
        }

        public static void AddBearerToken(this WebHeaderCollection hdrs, string rawToken)
        {
            hdrs.AddAuthorization("Bearer " + rawToken);
        }

        public static void AddBearerToken(this WebHeaderCollection hdrs, AccessToken token)
        {
            hdrs.AddAuthorization("Bearer " + token);
        }
    }
}
