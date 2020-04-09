<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Administration</title>
    <link href="style/admin.css" rel="stylesheet" />
    <link href="style/menu.css" rel="stylesheet" />
    <script src="http://code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
    <script src="scripts/menu.js"></script>
    <script src="scripts/admin.js"></script>
    <link href="style/popup.css" rel="stylesheet" />
    <script src="scripts/popup.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="main-container">
            <div id="left-menu">
                <ul>
                    <li class="side-menu-item" data-content="manageContainer">Manage Kiosk Settings</li>
                    <li class="side-menu-item" data-content="shutdown-container">Shutdown/Restart</li>
                    <li class="side-menu-item" data-content="autorenewContainer">Manage Autorenew</li>
                    <li class="side-menu-item" data-content="message-container">Send Message to Kiosk</li>
                    <li class="side-menu-item" data-content="stat-container">Generate Stats</li>
                    <li class="side-menu-item" data-content="printer-container">Manage Printers</li>
                    <li class="side-menu-item" id="logoff">Log Off</li>
                </ul>
            </div>
            <div id="content">

                <div id="manageContainer" class="content">
                    <h2>Manage Clients</h2>
                    <div id="manageSelect" runat="server" class="select-container">
                    </div>
                    <div id="manage-ids" class="select-container">
                        <select id="manage-id-select" class="ids">
                        </select>
                    </div>
                    <div class="select-container">
                        <input type="submit" id="retrieve-client" value="Retrieve Client" />
                    </div>
                    <div id="client-fields" data-id="-1">
                        <div id="subManageSelect" runat="server"></div>
                        <br />
                        <select id="client-id" class="ids"></select><br />
                        <input type="text" id="client-mac" placeholder="Client MAC Address" /><br />
                        <select id="client-registered">
                            <option value="false">Available</option>
                            <option value="true">Registered</option>
                        </select><br />
                        <select id="client-printer" style="margin-top:30px">
                            <option value="1">Envisionware</option>
                            <option value="2">Circulation Desk</option>
                        </select><br />
                        <input type="password" id="password" placeholder="Admin Password" /><br />
                        <input type="submit" value="Submit" class="submit" id="submit-manager" /><input type="submit" value="Cancel" class="cancel" id="submit-cancel" />
                    </div>
                </div>

                <div id="shutdown-container" class="content">
                    <h2 style="margin-bottom: 0px;">Select an option</h2>
                    <br />
                    <img src="images/shutdown.png" data-command="shutdown" title="Shutdown" class="command" />
                    <img src="images/restart.png" data-command="restart" title="Restart" class="command" /><br />
                    <input type="submit" value="Cancel" class="cancel" />
                </div>

                <div id="autorenewContainer" class="content" runat="server">
                </div>

                <div id="message-container" class="content">
                    <h2>Send Message to Client</h2>
                    <div id="messageSelect" runat="server" class="select-container">
                    </div>
                    <div class="select-container">
                        <select id="message-ids" class="ids">
                        </select><br />
                    </div>
                    <div>
                        <textarea id="message" placeholder="Enter Message"></textarea><br />
                        <input type="submit" value="Submit" class="submit" id="submit-message" /><input type="submit" value="Cancel" class="cancel" />
                    </div>
                </div>

                <div id="stat-container" class="content">
                    <h2 style="margin-bottom: 10px">Generate Statistics</h2>
                    <div id="radio-container">
                        <input type="date" class="date-picker" id="start-date" /><input type="date" class="date-picker" id="end-date" /><br />
                        <label for="total">Total</label>
                        <input type="radio" name="limiters" id="total" checked="checked" value="total" />
                        <label for="total">Grouped by Region</label>
                        <input type="radio" name="limiters" id="region" value="region" />
                        <label for="total">Grouped by Login Type</label>
                        <input type="radio" name="limiters" id="login-type" value="login" /><br />
                    </div>
                    <input type="submit" value="Submit" class="submit" id="submit-date" /><input type="submit" value="Cancel" class="cancel" />
                </div>

                <div id="printer-container" class="content">
                    <h2 style="margin-bottom: 10px">Designate Printer</h2>
                    <div id="printer-href-container">
                        <input type="submit" id="envisionware" class="printer submit" data-printer="1" style="margin-right:40px; text-decoration:none; width:200px;" value="Envisionware" />
                        <input type="submit" id="circulation-desk" class="printer submit" data-printer="2" style="text-decoration:none; width:200px;" value="Circulation Desk" />
                    </div>
                </div>

            </div>
        </div>
        <div id="menu">
            <ul id="regionmenu" runat="server">
            </ul>
        </div>
    </form>
    <div id="pop-up"></div>
</body>
</html>
