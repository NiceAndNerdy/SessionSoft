<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>SessionSoft Server</title>
    <link href="style/default.css" rel="stylesheet" />
    <link href="style/menu.css" rel="stylesheet" />
    <link href="style/popup.css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
    <script src="scripts/refresh.js"></script>
    <script src="scripts/querystring.js"></script>
    <script src="scripts/dictionaries.js"></script>
    <script src="scripts/default.js"></script>
    <script src="scripts/popup.js"></script>
    <script src="scripts/actions.js"></script>
</head>

<body>
    <div id="main-container">
        <h1 id="region-name">Kiosk</h1>
        <ul id="header" class="client">
            <li>ID</li>
            <li>Status</li>
            <li>Minutes</li>
            <li>Login</li>
            <li>Password</li>
            <li class="extra">&nbsp;</li>
            <li>Actions&nbsp;&nbsp;&nbsp;</li>
        </ul>
        <div id="client-container"></div>
    </div>

    <div id="menu">
        <ul id="regionmenu" runat="server">
        </ul>
    </div>
    <div id="pop-up"></div>
</body>
</html>
