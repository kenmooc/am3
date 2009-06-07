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
    using System.Web.Security;
    using System.Diagnostics;

    #endregion

    public class MaintenanceModeModule : HttpModuleBase
    {
        private HttpApplication _currentApplication;
        private bool _enabled;
        private string _loginUrl;
        private string _landingPage;
        private string _allowedRoles;
        private string _allowedUsers;
        private List<string> _allowedPaths;
        private MembershipUser _currentUser;
        private string _requestedUrlAbsolutePath;

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

            _allowedPaths = new List<string>();

            //
            // Extract the settings and verify them if needed.
            //

            bool enabled = Convert.ToBoolean(GetSetting(config, "enabled", bool.FalseString));
            if (!enabled) return;

            string landingPage = GetSetting(config, "landingPage");
            /*if (!IsValidPath(application, landingPage))
                throw new ApplicationException(string.Format(
                        "The provided landing page '{0}' is not valid.", landingPage));
            _allowedPaths.Add(landingPage);*/

            string loginUrl = GetSetting(config, "loginUrl", string.Empty);
            //if (IsValidPath(application, loginUrl)) _allowedPaths.Add(loginUrl);

            string allowedRoles = GetSetting(config, "allowedRoles", string.Empty);
            string allowedUsers = GetSetting(config, "allowedUsers", string.Empty);


            // If everything goes as planned, hook up to the required events.

            application.AuthenticateRequest += new EventHandler(application_AuthenticateRequest);
            application.AuthorizeRequest += new EventHandler(application_AuthorizeRequest);
            application.BeginRequest += new EventHandler(application_BeginRequest);


            //
            // Finally, commit the state of the module if we got this far.
            // Anything beyond this point should not cause an exception.
            //

            _currentApplication = application;
            _enabled = enabled;
            _allowedRoles = allowedRoles;
            _allowedUsers = allowedUsers;
            _landingPage = landingPage;
            _loginUrl = loginUrl;

        }



        /// <summary>
        /// Determines whether the module will be registered for discovery
        /// in partial trust environments or not.
        /// </summary>

        protected override bool SupportDiscoverability
        {
            get { return true; }
        }

        /// <summary>
        /// The handler called when a request is passed on to the module
        /// </summary>

        protected virtual void application_BeginRequest(object sender, EventArgs e)
        {
            _currentApplication.Context.Trace.Warn("-- Inside Begin Request --");
        }

        /// <summary>
        /// The handler called when the Identity of the user has been establised
        /// </summary>

        protected virtual void application_AuthenticateRequest(object sender, EventArgs e)
        {
            _currentApplication.Context.Trace.Warn("-- Inside Authenticate --");

            string reqUrl = _currentApplication.Request.Path;
            string destinationPath = string.Empty;

            if (reqUrl.Contains("trace.axd")) return;

            _requestedUrlAbsolutePath = _currentApplication.Server.MapPath(reqUrl);

            if (String.Compare(_currentApplication.Server.MapPath(LoginUrl), _requestedUrlAbsolutePath, true) == 0 ||
                String.Compare(_currentApplication.Server.MapPath(LandingPage), _requestedUrlAbsolutePath, true) == 0)
                return;

            try
            {
                HttpContext context = HttpContext.Current;
                _currentUser = Membership.GetUser(context.User.Identity.Name);
                
                if (_currentUser != null && IsUserAllowed()) return;
                else
                {
                    //
                }

            }
            catch (Exception)
            {
                _currentApplication.Context.Trace.Warn("Current User: null");
                _currentApplication.Response.Redirect(LandingPage);
            }
        }

        protected virtual void application_AuthorizeRequest(object sender, EventArgs e)
        {
            _currentApplication.Context.Trace.Warn("Inside Authorize Request");
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
            return Configuration.GetSubsection("maintenanceMode");
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

        private bool IsPathAllowed(string path)
        {
            return true;
        }

        private bool IsValidPath(HttpApplication application, string path)
        {
            bool exists = false;
            try
            {
                application.Server.MapPath(path);
                exists = true;
            }
            catch (Exception)
            {
            }
            return exists;
        }

        private bool IsUserAllowed()
        {
            _currentApplication.Context.Trace.Warn("--- Checking if user is allowed ---");
            _currentApplication.Context.Trace.Warn("Current User: " + _currentUser.UserName);
            _currentApplication.Context.Trace.Warn("Allowed Users: " + _allowedUsers);

            if (_allowedUsers.Contains(_currentUser.UserName))
                return true;

            _currentApplication.Context.Trace.Warn("--- Checking if role is allowed ---");
            string[] allowedRoles = _allowedRoles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string role in allowedRoles)
            {
                _currentApplication.Context.Trace.Warn("Role: " + role);
                if (Roles.IsUserInRole(_currentUser.UserName, role.Trim())) return true;
            }


            _currentApplication.Context.Trace.Warn("--- User is not allowed ---");

            return false;
        }

    }
}
