$(document).ready(function () {
    $('#menu').on('mouseenter', function () {
        $('#regionmenu').animate({
            'margin-right': '0px'
        });
        $('#arrow-left').html('&raquo;');
    });

    $(document).on('click', '#arrow-left', function () {
        $('#regionmenu').animate({
            'margin-right': '-685px'
        });
        $('#arrow-left').html('&laquo;');
    });

    $(document).on('click', '.region', function () {
        var region = $(this).text();
        getRegion(region);
    });

    var initializeRegion = function () {
        var region = QueryString("region") || "Kiosk";
        getRegion(region);
    }();

    function getRegion(region) {
        $('#region-name').text(region);
        $.ajax({
            type: "POST",
            data: JSON.stringify({ region: region }),
            url: "Controller.aspx/GetSelectedRegion",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var clients = JSON.parse(response.d);
                buildRegion(clients);
            }
        });
    }

    function getRenewalScript(status, renewals)
    {
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

    function buildRegion(clients) {
        var html = [];
        for (var i = 0; i < clients.length; i++) {
            html.push('<div class="element-container"><ul class="client ' + clients[i].Status + '" data-id="' + clients[i].ID + '" data-status="' + clients[i].Status + '">' +
                '<li>' + clients[i].Kiosk_ID + '</li>' +
                '<li>' + getRenewalScript(clients[i].Status, clients[i].Renewals) + '</li>' +
                '<li>' + Math.floor(clients[i].Time / 60) + '</li>' +
                '<li>' + getLoggedScript(clients[i].Login) + '</li>' +
                '<li>' + clients[i].Password + '</li>' +
                '<li class="extra">&nbsp;</li>' +
                '<li><img src="images/settings.png" class="settings" />' + (images[clients[i].Status] || "") + '</li>' +
                '</ul></div>');
        }
        $('#client-container').html(html.join(''));
    }     
});