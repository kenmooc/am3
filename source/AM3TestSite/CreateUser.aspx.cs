using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace AM3TestSite
{
    public partial class CreateUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpUserList.DataSource = Membership.GetAllUsers();
                drpUserList.DataBind();

                drpRoles.DataSource = Roles.GetAllRoles();
                drpRoles.DataBind();
            }
        }

        protected void btnAssignRole_Click(object sender, EventArgs e)
        {
            Roles.AddUserToRole(drpUserList.SelectedValue, drpRoles.SelectedValue);
        }

        protected void btnCreateRole_Click(object sender, EventArgs e)
        {
            Roles.CreateRole(txtNewRole.Text);
        }
    }
}
