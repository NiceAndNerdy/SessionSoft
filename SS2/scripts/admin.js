$(document).ready(function () {

    var popUpClose = '<input type="submit" id="popup-close" value="Close" />';

    var getSelectedPrinter = function () {
        $.ajax({
            type: "POST",
            url: "Controller.aspx/GetSelectedPrinter",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var printer = response.d;
                if (printer == 1) {
                    $('#envisionware').addClass('selected-printer');
                }
                else {
                    $('#circulation-desk').addClass('selected-printer');
                }
            }
        });
    }();

    $('.side-menu-item').on('click', function () {
        $('.side-menu-item').css('color', '#FFFFFF');
        $('.content').hide();
        $(this).css('color', 'yellow');
        var contentItem = $(this).data('content');
        $('#' + contentItem).show();
    });

    $(document).on('change', '.manage-select', function () {
        target = $(this).closest('div').next('div').find('select');
        getIDs($(this).val(), target);
    });

    $('#retrieve-client').on('click', function (e) {
        e.preventDefault();
        getClient($('#manage-id-select').val());
    });

    $('#submit-manager').on('click', function (e) {
        e.preventDefault();
        var id = $('#client-fields').data('id');
        saveClient(id);
    });

    $('#submit-message').on('click', function (e) {
        e.preventDefault();
        var id = $('#message-ids').val();
        var message = $('#message').val();
        submitMessage(id, message);
    });

    $('.command').on('click', function () {
        var command = $(this).data('command');
        buildCommandBox(command);
    });

    function buildCommandBox(command) {
        Popup.Open('<span>This will ' + command + ' every public computer in the library.  Do you wish to proceed?</span><br /><input data-command="' + command + '" type="submit" id="yes-command" value="Yes"/>' + popUpClose, 1);
        $('#popup-close').attr('value', 'No');
    }

    $(document).on('click', '#yes-command', function () {
        submitCommand($(this).data('command'));
    });

    $('.autocheck').on('click', function () { 
        var property = $(this).prop('checked');
        var region = $(this).data('region');
        submitAutorenew(region, property);
    });

    $('#submit-date').on('click', function (e) {
        e.preventDefault();
        var radioValue = $("input[name='limiters']:checked").val();
        var startDate = $('#start-date').val();
        var endDate = $('#end-date').val();
        getStatData(radioValue, startDate, endDate);
    });

    $('.printer').on("click", function (e) {
        e.preventDefault();
        $('.printer').removeClass('selected-printer');
        var printer = $(this).data('printer');
        $(this).addClass('selected-printer');
        submitPrinter(printer);
    });

    $('#logoff').on('click', function () {
        $.ajax({
            type: "POST",
            url: "Controller.aspx/LogOff",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) { window.location.replace("Login.aspx"); }
            }
        });
    });

    $('.cancel').on('click', function () {
        window.location.replace() 
    });

    $(document).on('click', '#popup-close', function (e) {
        e.preventDefault();
        Popup.Close();
    });

    var fillInitialTarget = function () {
        $('.ids').each(function () {
            getIDs("Kiosk", $(this));
        });
        $('#client-registered').val('');
    }();

    function getClient(id) {
        $.ajax({
            type: "POST",
            data: JSON.stringify({ id: id }),
            url: "Controller.aspx/GetClient",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var clientData = JSON.parse(response.d);
                $('#client-fields').data('id', clientData.ID);
                setSelect(clientData.Region, $('#client-region option'));
                setSelect(clientData.Kiosk_ID, $('#client-id option'));
                var availability = (clientData.Registered) ? 'Registered' : 'Available';
                setSelect(availability, $('#client-registered option'));
                var printer = (clientData.SelectedPrinter == 1) ? 'Envisionware' : 'Circulation Desk';
                setSelect(printer, $('#client-printer option'));
                $('#client-mac').val(clientData.Mac);
                $('#client-fields').show();
            }
        });
    }

    function setSelect(text, select) {
        var matchingOption = $(select).filter(function () {
            return $(this).text() == text;
        });
        matchingOption.prop('selected', true);
    }

    function saveClient(id) {
        var kiosk_id = $('#client-id').val();
        var region = $('#client-region').val();
        var mac = $('#client-mac').val();
        var registered = $('#client-registered').val();
        var printer = $('#client-printer').val();
        var password = $('#password').val();

        $.ajax({
            type: "POST",
            data: JSON.stringify({ id: id, kiosk_id: kiosk_id, region: region, mac: mac, registered: registered, printer: printer, password: password }),
            url: "Controller.aspx/UpdateAdminClient",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d == '1-error') {
                    Popup.Open('<span>Could not save this client. Check data integrity of the fields!</span><br />' + popUpClose, 1);
                } else if (response.d == '2-error') {
                    $('#password').val('');
                    Popup.Open('<span>Invalid password!</span><br />' + popUpClose, 1);
                } else {
                    $('#client-id').val('');
                    $('#client-region').val('');
                    $('#client-mac').val('');
                    $('#client-registered').val('');
                    $('#client-printer').val('');
                    Popup.Open('<span>Client Saved!</span><br />' + popUpClose, 1);
                }
            }
        });
    }

    function submitAutorenew(region, property) {
        $.ajax({
            type: "POST",
            data: JSON.stringify({ region: region, property: property }),
            url: "Controller.aspx/SubmitAutorenew",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    Popup.Open('<span>' + response.d + '</span><br />' + popUpClose, 1);
                }
                else {
                    Popup.Open('<span>Autorenew setting has been updated!</span><br />' + popUpClose, 1);
                }
            }
        });
    }

    function submitCommand(command) {
        $.ajax({
            type: "POST",
            data: JSON.stringify({ command: command }),
            url: "Controller.aspx/SendCommand",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    Popup.Open('<span>' + response.d + '</span><br />' + popUpClose, 1);
                }
                else {
                    Popup.Open('<span>Command sent successfully!</span><br /' + popUpClose, 1);
                }
            }
        });
    }

    function submitPrinter(printer) {
        var printerText = (printer == 1) ? "Envisionware" : "the circulation desk";
        $.ajax({
            type: "POST",
            data: JSON.stringify({ printer: printer }),
            url: "Controller.aspx/SendPrinter",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    Popup.Open('<span>' + response.d + '</span><br />' + popUpClose, 1);
                }
                else {
                    Popup.Open('<span>Printers have been rerouted to ' + printerText + '!  May take up to five minutes to load.</span><br />' + popUpClose, 1);
                }
            }
        });
    }

    function submitMessage(id, message) {
        $.ajax({
            type: "POST",
            data: JSON.stringify({ id: id, message: message }),
            url: "Controller.aspx/SendMessage",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    Popup.Open('<span>' + response.d + '</span><br />' + popUpClose, 1);
                }
                else {
                    Popup.Open('<span>Message sent successfully!</span><br />' + popUpClose, 1);
                }
            }
        });
    }

    function getStatData(radioValue, startDate, endDate) {
        $.ajax({
            type: "POST",
            data: JSON.stringify({ radioValue: radioValue, startDate: startDate, endDate: endDate }),
            url: "Controller.aspx/GetStats",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    showResults(response.d);
                }
                else {
                    Popup.Open('<span>Message sent successfully!</span><br />' + popUpClose, 1);
                }
            }
        });
    }

    function showResults(content) {

        var html = '<html>' +
            '<head><style>body { font-family:tahoma; font-size:1.2em; } h1, h2 {margin-bottom:-20px; padding-bottom:-20px;} td {padding: 3px 15px;}</style></head>' +
            '<body>' + content + '</body>' +
            '<html>';

        var WindowObject = window.open('', 'PrintWindow');
        WindowObject.document.writeln(html);
        WindowObject.document.close();
        WindowObject.focus();
    }

    function getIDs(region, target) {
        var html = [];
        $.ajax({
            type: "POST",
            data: JSON.stringify({ region: region }),
            url: "Controller.aspx/GetClientIDs",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var clientData = JSON.parse(response.d);
                const keys = Object.keys(clientData);
                for (const key of keys) {
                    html.push('<option value="' + key + '">' + clientData[key] + '</option>');
                }
                target.html(html.join(''));
            }
        });
    }
});