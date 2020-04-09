$(document).ready(function () {

    $(document).on("click", "#registration", function (e) {
        e.preventDefault();
        getAvailableKiosks();

    });

    $(document).on('change', '#regions', function () {
        var html = [];
        for (var i = 0; i < availableKiosks[$('#regions').val()].length; i++)
        {
            html.push('<option value="' + availableKiosks[$('#regions').val()][i] + '">' + availableKiosks[$('#regions').val()][i] + '</option>")');
        }

        $('#kiosks').html(html.join(''));
    });


    $(document).on('click', '#submit-kiosk', function (e) {
        e.preventDefault();
        registerKiosk();
    });

    function getAvailableKiosks() {
        $.ajax({
            type: "POST",
            url: "/SessionSoftWebClient/Controller.aspx/GetAvailableKiosks",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                availableKiosks = JSON.parse(response.d);
                var html = [];
                html.push('<input type="text" placeholder="Mac Address - no :\'s or spaces" id="macAddress"/><br /><select id="regions">');
                for (property in availableKiosks)
                {
                    html.push('<option value="' + property + '">' + property + '</option>")');
                }
                html.push('</select><br /><select id="kiosks"></select><br /><input id="password" type="password" placeholder="Admin password" /><br /><input type="submit" value="Register Kiosk" id="submit-kiosk" />');
                $('#registration-container').html(html.join(''));
                var first = '';
                for (property in availableKiosks) {
                    first = property;
                    break;
                }
                var first = availableKiosks[first];
                var html2 = [];
                for (var i = 0; i < first.length; i++) {
                    html2.push('<option value="' + first[i] + '">' + first[i] + '</option>")');
                }

                $('#kiosks').html(html2.join(''));
                $('#macAddress').val(querySt('mac'));
            }
        });

    }

    function registerKiosk() {
        var mac = $('#macAddress').val();
        var region = $('#regions').val();
        var kiosk_id = $('#kiosks').val();
        var password = $('#password').val();

        $.ajax({
            type: "POST",
            url: "/SessionSoftWebClient/Controller.aspx/RegisterKiosk",
            data: JSON.stringify({ mac: mac, region: region, kiosk_id: kiosk_id, password: password }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d == "x") 
                {
                    alert("Could not register kiosk!")
                }
                else if (response.d == "mac") {
                    alert('That mac address already appears to be registered!')
                }
                else {
                    $('#registration-container').html('<h1>Registration successful!</h1><br /><a href="/Default.aspx?id=' + response.d + '">Refresh page</a>');
                }
            }
        });
    }

    function querySt(Key) {
        var url = window.location.href;
        KeysValues = url.split(/[\?&]+/);
        for (i = 0; i < KeysValues.length; i++) {
            KeyValue = KeysValues[i].split("=");
            if (KeyValue[0] == Key) {
                return KeyValue[1];
            }
        }
    }

    var availableKiosks;

});