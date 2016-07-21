<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="Practice12.New" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>This is New Test </title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Lets Begin Form </h1>
        Submission Name:
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        Type:
        <asp:DropDownList ID="DropDownList1" runat="server">
            <asp:ListItem Value="a" Text="Male" />
            <asp:ListItem Value ="b" Text ="Female" />
        </asp:DropDownList>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="Save" />
        <asp:Literal ID="Feedback" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>

