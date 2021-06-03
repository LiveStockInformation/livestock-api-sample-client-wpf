﻿using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Identity.Client;

namespace livestock_api_samples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// B2C tenant name
        /// </summary>
        private static readonly string TenantName = ConfigurationManager.AppSettings["TenantName"];
        private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";

        /// <summary>
        /// ClientId for the application which initiates the login functionality (this app)  
        /// </summary>
        private static readonly string ClientId = ConfigurationManager.AppSettings["ClientId"];


        /// <summary>
        /// Should be one of the choices on the Azure AD B2c / [This App] / Authentication blade
        /// </summary>
        private static readonly string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";

        /// <summary>
        /// From Azure AD B2C / UserFlows blade
        /// </summary>
        public static string PolicySignUpSignIn = ConfigurationManager.AppSettings["PolicySignUpSignIn"];
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = ConfigurationManager.AppSettings["PolicyResetPassword"];

        /// <summary>
        /// Note: AcquireTokenInteractive will fail to get the AccessToken if "Admin Consent" has not been granted to this scope.  To achieve this:
        /// 
        /// 1st: Azure AD B2C / App registrations / [API App] / Expose an API / Add a scope
        /// 2nd: Azure AD B2C / App registrations / [This App] / API Permissions / Add a permission / My APIs / [API App] / Select & Add Permissions
        /// 3rd: Azure AD B2C / App registrations / [This App] / API Permissions / ... (next to add a permission) / Grant Admin Consent for [tenant]
        /// </summary>
        public static string[] ApiScopes = { ConfigurationManager.AppSettings["ApiScopes"] };

        /// <summary>
        /// URL for API which will receive the bearer token corresponding to this authentication
        /// </summary>
        public static string ApiEndpoint = ConfigurationManager.AppSettings["APiEndpoint"];

        // Shouldn't need to change these:
        private static string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignUpSignIn = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";

        public static IPublicClientApplication PublicClientApp { get; private set; }

        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignUpSignIn)
                .WithRedirectUri(RedirectUri)
                .WithLogging(Log, LogLevel.Info, false) // don't log P(ersonally) I(dentifiable) I(nformation) details on a regular basis
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }

        private static void Log(LogLevel level, string message, bool containsPii)
        {
            string logs = $"{level} {message}{Environment.NewLine}";
            File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalLogs.txt", logs);
        }
    }
}