$(document).ready(function () {
    $(document).on('click', '.start', function () {
        var element = $(this).closest('.client');
        var id = element.data('id');
        updateClient(id, "pending", element);
    });

    $(document).on('click', '.stop', function () {
        var element = $(this).closest('.client');
        var id = element.data('id');
        var status = element.data('status');
        if (status == "pending") { updateClient(id, "locked", element); }
        else { updateClient(id, "terminate", element); }
    });



    var renewalPopup = function (id) {
        return '<div class="renewal-container" data-id="' + id + '"><h2>Please select a renewal time.</h2>' +
            '<select id="time">' +
            '<option value="3600">60</option>' + 
            '<option value="2700">45</option>' + 
            '<option value="1800">30</option>' + 
            '<option value="9000">15</option>' + 
            '<option value="300">5</option>' + 
            '</select>' +
            '<br />' +
            '<input type="submit" id="submit-renewal" class="popup-buttons" value= "Submit" /><input type="submit" id="popup-close-button" class="popup-buttons" value="Cancel" /></div > ';
    }

    $(document).on('click', '.renew', function () {
        var element = $(this).closest('.client');
        var id = element.data('id');
        var renewalHtml = renewalPopup(id);
        Popup.Open(renewalHtml, 1);
    });

    $(document).on('click', '#submit-renewal', function () {
        var renewalElement = $(this).closest('.renewal-container');
        var id = renewalElement.data('id');
        var time = $('#time').val();
        var viewElement = $('#client-container').find("[data-id='" + id + "']"); 
        Popup.Close(updateClient(id, "renewed", viewElement, time));
    });



    var settingsPopup = function (id, status) {
        var enableButton = '';
        if (status == "disabled") {
            enableButton = '<input type="submit" id="enable" class="popup-buttons settings-buttons" value="Enable" data-action="locked" />';
        } else {
            enableButton = '<input type="submit" id="disable" class="popup-buttons settings-buttons" value="Disable" data-action="disabled" />';
        }
        return '<div class="settings-container" data-id="' + id + '"><h2>Choose a function.</h2>' +
            
            '<input type="submit" id="shutdown" class="popup-buttons settings-buttons" value="Shutdown" data-action="shutdown" /><input type="submit" id="restart" class="popup-buttons settings-buttons" value= "Restart" data-action="restart" />' +
            enableButton + '<br /><br /><input type="submit" id="popup-close-button" class="popup-buttons" value="Cancel" /></div>'
    }

    $(document).on('click', '.settings', function () {
        var element = $(this).closest('.client');
        var id = element.data('id');
        var status = element.data('status');
        var settingsHtml = settingsPopup(id, status);
        Popup.Open(settingsHtml, 1);
    });

    $(document).on('click', '.settings-buttons', function (e) {
        e.preventDefault();
        var action = $(this).data('action');
        var settingsElement = $(this).closest('.settings-container');
        var id = settingsElement.data('id');
        var viewElement = $('#client-container').find("[data-id='" + id + "']");
        Popup.Close(updateClient(id, action, viewElement));
    });



    $(document).on('click', ".logged", function () {
        var barcode = $(this).data('login');
        copyToClipboard(barcode);
    });

    function copyToClipboard(text) {
        var dummy = document.createElement("input");
        document.body.appendChild(dummy);
        dummy.setAttribute('value', text);
        dummy.select();
        document.execCommand("copy");
        document.body.removeChild(dummy);
    }

    $(document).on('click', '#popup-close-button', function () {
        Popup.Close();
    });


    $(document).on('click', '#popup-close', function () {
        Popup.Close();
    });




    function updateClient(id, status, element, time) {
        if (!time) { time = 3600; }
        $.ajax({
                type: "POST",
                data: JSON.stringify({ status: status, id: id, time: time }),
                url: "Controller.aspx/UpdateClient",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var clientData = JSON.parse(response.d);
                    updateElement(element, clientData);
                }
            });
    }

    function getRenewalScript(status, renewals) {
        if (status == "renewed") {
            return '<span class="renewal-count" title="' + renewals + '">' + status + '</span>';
        }
        else return status;
    }

    function getLoggedScript(login) {
        if ((login != "staff") && (login != "none")) {
            return '<span class="logged" title="Click to copy barcode" data-login="' + login + '">public</span>';
        }
        else return login;
    }

    function updateElement(element, clientData) {
        var html = [];
        html.push('<div class="element-container"><ul class="client ' + clientData.Status + '" data-id="' + clientData.ID + '" data-status="' + clientData.Status + '">' +
            '<li>' + clientData.Kiosk_ID + '</li>' +
            '<li>' + getRenewalScript(clientData.Status, clientData.Renewals) + '</li>' +
            '<li>' + Math.floor(clientData.Time / 60) + '</li>' +
            '<li>' + getLoggedScript(clientData.Login) + '</li>' +
            '<li>' + clientData.Password + '</li>' +
            '<li class="extra">&nbsp;</li>' +
            '<li><img src="images/settings.png" class="settings" />' + images[clientData.Status] || '' + '</li>' +
            '</ul></div>');
        var container = element.closest('.element-container');
        $(container).html(html.join(''));
        if (clientData.Status == "pending") {
            Popup.Open('<h2>The password for ' + clientData.Region + ' ' + clientData.Kiosk_ID + ' is ' + clientData.Password + '.<br /><input type="submit" id="popup-close" value= "Close" />', 1);
        } 
    }

  
});