<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SessionSoft Server Login</title>
    <style>
        body {
            font-family: Tahoma;
            background-color: #2C3539;
            color:#FFFFFF;
        }

        hr {
            width:200px;
            margin-left:0px;
            color:#FFFFFF;
        }
        
        #login-menu {
            margin:125px auto;
            width:260px;
            padding:30px 10px 20px 60px;
            background-color:#408080;
            border-radius:7px;
            box-shadow: 1px 1px #AAAAAA;
        }

        #login-menu p {
            font-size:1.5em;
            margin-bottom:-5px;
        }

        #login-menu input[type="text"], input[type="password"] {
            margin-right:20px;
            margin-bottom:10px;
            border-radius:7px;
            height:30px;
            width:200px;
            font-size:1.3em;
        }

        #login-menu input[type="submit"] {
            margin-right:20px;
            margin-bottom:10px;
            margin-top:10px;
            border-radius:7px;
            height:30px;
            width:80px;
            font-size:1em;   
        }

        #login-menu label {
            font-size:1.1em;   
        }

        #login-menu span {
            margin-bottom:15px;
        }

        footer {
	        position:absolute;
            bottom:5px;
            right:5px;
            font-size:2em;
            color:#2C3539;
            text-shadow: 2px 2px #a5a5a5;
        }


        
    </style>
    <script>
        window.onload = function () {
            document.getElementById('btnClear').onclick = function (e) {
                e.preventDefault();
                e.returnValue = false;
                document.getElementById("txtUsername").value = "";
                document.getElementById("txtPassword").value = "";
            };
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="login-menu">
        <p>Please login</p><hr />
        <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox><br />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />
        <asp:Label ID="lblError" runat="server" Visible="false" Text="Invalid username or password!" ForeColor="White"></asp:Label><br />
        <asp:Button ID="btnSubmit" runat="server" Text="Login" OnClick="btnSubmit_Click" />
        <input type="submit" id="btnClear" value="Clear" /><br />
        <asp:CheckBox ID="chkPersist" runat="server" Text="Keep me logged in" />
    </div>
    <footer>SessionSoft Server &copy; 2018</footer>
    </form>
</body>
</html>
