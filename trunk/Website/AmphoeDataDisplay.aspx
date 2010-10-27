<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AmphoeDataDisplay.aspx.cs" Inherits="AmphoeDataDisplay" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    Changwat
        <asp:DropDownList ID="cbx_changwat" runat="server" AutoPostBack="True" 
            Height="25px" Width="172px" 
            onselectedindexchanged="cbx_changwat_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        Amphoe
        <asp:DropDownList ID="cbx_amphoe" runat="server" AutoPostBack="True" 
            onselectedindexchanged="cbx_amphoe_SelectedIndexChanged" Height="16px" 
            Width="171px">
        </asp:DropDownList>
    
    </div>
    </form>
</body>
</html>
