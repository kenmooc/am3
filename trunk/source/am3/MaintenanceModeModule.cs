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

    #endregion

    public class MaintenanceModeModule : HttpModuleBase
    {
        private bool _enabled;
        private string _loginUrl;
        private string _landingPage;
        private string _allowedRoles;
        private string _allowedUsers;
        private List<string> _allowedPaths;
        private MembershipUser _currentUser;

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

            application.BeginRequest += new EventHandler(application_BeginRequest);
            application.AuthenticateRequest += new EventHandler(application_AuthenticateRequest);


            //
            // Finally, commit the state of the module if we got this far.
            // Anything beyond this point should not cause an exception.
            //

            _enabled = enabled;
            _allowedRoles = allowedRoles;
            _allowedUsers = allowedUsers;
            _landingPage = landingPage;
            _loginUrl = loginUrl;

        }

        

        /// <summary>
        /// The handler called when a request is passed on to the module
        /// </summary>

        protected virtual void application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            string reqUrl = app.Request.RawUrl.ToLower();
            string destinationPath = string.Empty;
            string reqUrlAbsPath = app.Server.MapPath(reqUrl.ToLower());
            
            /*if (reqUrlAbsPath.Equals(app.Server.MapPath(_landingPage)) || reqUrlAbsPath.Equals(app.Server.MapPath(_loginUrl)))
                return;
            else if (app.Context.User.Identity.IsAuthenticated)
            {
                string username = app.Context.User.Identity.Name;
                
                // Check if the user belongs to allowed users list
                if (_allowedUsers.Contains(username))
                    return;
                else
                {
                    string[] roles = _allowedRoles.Split(',');
                    foreach (string role in roles)
                    {
                        if (Roles.IsUserInRole(username, role))
                        {
                            return;
                        }
                    }
                }

            }
            else
                destinationPath = _landingPage;*/

            if(_currentUser != null)
                app.Response.Write(_currentUser.UserName);

            //app.Response.Write(app.Server.MapPath(reqUrl) + " " + app.Server.MapPath(destinationPath.ToLower()));
            /*if (!reqUrlAbsPath.Equals(app.Server.MapPath(destinationPath.ToLower())))
            {
                //app.Response.StatusCode = 301; // make a permanent redirect
                app.Response.Redirect(destinationPath);
                //app.Response.End();
            }*/
        }

        /// <summary>
        /// The handler called when the Identity of the user has been establised
        /// </summary>
        
        protected virtual void application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            _currentUser = Membership.GetUser(context.User.Identity.Name);
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

    }
}
