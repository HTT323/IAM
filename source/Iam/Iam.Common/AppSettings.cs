#region

using System.Configuration;

#endregion

namespace Iam.Common
{
    public static class AppSettings
    {
        #region IAM General Settings

        public static readonly string IamConnectionString =
            ConfigurationManager.AppSettings["IamConnectionString"];

        public static readonly string AuthDomain =
            ConfigurationManager.AppSettings["AuthDomain"];

        public static readonly string AdminDomain =
            ConfigurationManager.AppSettings["AdminDomain"];

        public static readonly string IamClientId =
            ConfigurationManager.AppSettings["IamClientId"];

        public static readonly string IamClientName =
            ConfigurationManager.AppSettings["IamClientName"];

        public static readonly string IamHomeFullUrl =
            ConfigurationManager.AppSettings["IamHomeFullUrl"];

        public static readonly string IamAdminFullUrl =
            ConfigurationManager.AppSettings["IamAdminFullUrl"];

        #endregion

        #region Identity Server

        public static readonly string IdsConnectionString =
            ConfigurationManager.AppSettings["IdsConnectionString"];

        public static readonly string IdpAuthority =
            ConfigurationManager.AppSettings["IdpAuthority"];

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