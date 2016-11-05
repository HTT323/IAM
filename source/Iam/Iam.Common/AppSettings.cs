#region

using System.Configuration;

#endregion

namespace Iam.Common
{
    public static class AppSettings
    {
        public const string IamAdministratorRole = "IAM Administrator";

        #region Seed Data for IAM

        public static readonly string SeedUserName =
            ConfigurationManager.AppSettings["Seed.UserName"];

        public static readonly string SeedPassword =
            ConfigurationManager.AppSettings["Seed.Password"];

        #endregion

        #region IAM General Settings

        public static readonly string BaseUrl =
            ConfigurationManager.AppSettings["BaseUrl"];

        public static readonly string IamConnectionString =
            ConfigurationManager.AppSettings["IamConnectionString"];

        #endregion

        #region Identity Server
        
        public static readonly string IdentityServerPath =
            ConfigurationManager.AppSettings["IdentityServerPath"];

        public static readonly string IdentityServerSiteName =
            ConfigurationManager.AppSettings["IdentityServerSiteName"];

        public static readonly string CertificateSubject =
            ConfigurationManager.AppSettings["CertificateSubject"];

        #endregion

        #region Password Settings

        public static readonly bool RequireNonLetterOrDigit =
            bool.Parse(ConfigurationManager.AppSettings["RequireNonLetterOrDigit"]);

        public static readonly bool RequireDigit =
            bool.Parse(ConfigurationManager.AppSettings["RequireDigit"]);

        public static readonly bool RequireLowercase =
            bool.Parse(ConfigurationManager.AppSettings["RequireLowercase"]);

        public static readonly bool RequireUppercase =
            bool.Parse(ConfigurationManager.AppSettings["RequireUppercase"]);

        public static readonly int RequiredLength =
            int.Parse(ConfigurationManager.AppSettings["RequiredLength"]);

        #endregion
    }
}