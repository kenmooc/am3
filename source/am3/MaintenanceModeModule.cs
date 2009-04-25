using System;
using System.Collections.Generic;
using System.Text;

namespace AM3
{
    #region Imports

    using System;
    using System.Security;
    using System.Web;
    using IDictionary = System.Collections.IDictionary;

    #endregion

    public class MaintenanceModeModule : HttpModuleBase
    {
        private bool _enabled;
        private string _loginUrl;
        private string _landingPage;
        private string _allowedRoles;
        private string _allowedUsers;

        /// <summary>
        /// Initializes the module and prepares it to handle requests.
        /// </summary>

        protected override void OnInit(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            //
            // Get the configuration section of this module.
            // If it's not there then there is nothing to initialize or do.
            // In this case, the module is as good as mute.
            //

            IDictionary config = (IDictionary)GetConfig();

            if (config == null)
                return;

            //
            // Extract the settings.
            //

            bool enabled = Convert.ToBoolean(GetSetting(config, "enabled", bool.FalseString));
            if (!enabled) return;

            string landingPage = GetSetting(config, "landingPage");
            string loginUrl = GetSetting(config, "loginUrl", string.Empty);
            string allowedRoles = GetSetting(config, "allowedRoles", string.Empty);
            string allowedUsers = GetSetting(config, "allowedUsers", string.Empty);
        }

        /// <summary>
        /// Returns true if AM3 is enabled in the configuration file
        /// </summary>
        
        protected virtual bool Enabled
        {
            get { return _enabled; }
        }

        /// <summary>
        /// Gets the relative path to the web applications login page
        /// </summary>
        
        protected virtual string LoginUrl
        {
            get { return _loginUrl; }
        }

        /// <summary>
        /// Gets the relative path to the landing page that will be displayed to users
        /// when the AM3 is enabled.
        /// </summary>
        
        protected virtual string LandingPage
        {
            get { return _landingPage; }
        }

        /// <summary>
        /// Gets a comma-delimited list of roles that will be allowed access.
        /// </summary>
        
        protected virtual string AllowedRoles
        {
            get { return _allowedRoles; }
        }

        /// <summary>
        /// Gets a comma-delimited list of users that will be allowed access.
        /// </summary>
        
        protected virtual string AllowedUsers
        {
            get { return _allowedUsers; }
        }

        /// <summary>
        /// Gets the configuration object used by <see cref="OnInit"/> to read
        /// the settings for module.
        /// </summary>

        protected virtual object GetConfig()
        {
            return Configuration.GetSection("am3");
        }

        private static string GetSetting(IDictionary config, string name)
        {
            return GetSetting(config, name, null);
        }

        private static string GetSetting(IDictionary config, string name, string defaultValue)
        {
            string value = Utility.MaskNullString((string)config[name]);

            if (value.Length == 0)
            {
                if (defaultValue == null)
                {
                    throw new ApplicationException(string.Format(
                        "The required configuration setting '{0}' is missing for module.", name));
                }

                value = defaultValue;
            }

            return value;
        }
    }
}
