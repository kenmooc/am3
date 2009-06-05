<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="AM3TestSite.CreateUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
        <br />
        Create Roles<br />
        <asp:TextBox ID="txtNewRole" runat="server" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnCreateRole" runat="server" Text="Create" 
            onclick="btnCreateRole_Click" />
        <br />
        <br />
        Assign Roles<br />
        <asp:DropDownList ID="drpUserList" runat="server">
        </asp:DropDownList>
        &nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="drpRoles" runat="server">
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnAssignRole" runat="server" Text="Assign" 
            onclick="btnAssignRole_Click" />
    </div>
    </form>
</body>
</html>
