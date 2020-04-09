var sentinel = false;

var loginWindow = '<h1>Please choose a login option</h1>' +
    '<div id="icon-container">' +
    '<div class="icon">' +
    '<img src="img/staff.png" id="staff" /><br />' +
    '<p>Password you got from a Staff Member</p>' +
    '</div>' +
    '<div class="icon" id="koha">' +
    '<img src="img/books.png" /><br />' +
    '<p>Library Username and Password</p>' +
    '</div>' +
    '<div class="icon">' +
    '<img src="img/barcode.png" id="barcode" /><br />' +
    '<p>Library Barcode</p>' +
    '</div>' +
    '</div>';

var koha = '<div class="login-container">' +
    '<h1>Please enter your library username and pin.</h1>' +
    '<input type="text" id="kohaUserName" placeholder="Username" /><br />' +
    '<input type="password" id="kohaPin" placeholder="Pin number" /><br />' +
    '<input type="submit" id="submitKoha" class="submit-button" value="Submit" /><input type="submit" class="main-menu" value="Go Back" />' +
    '</div>';

var barcode = '<div class="login-container">' +
    '<h1>Please enter your library barcode number.</h1>' +
    '<input type="text" id="libraryBarcode" placeholder="Library Barcode" /><br />' +
    '<input type="submit" id="submitBarcode" class="submit-button" value="Submit" /><input type="submit" class="main-menu" value="Go Back" />' +
    '</div>';

var staff = '<div class="login-container">' +
    '<h1>Please enter the password you were given by staff.</h1>' +
    '<input type="password" id="staffPassword" placeholder="Password" /><br />' +
    '<input type="submit" id="submitPassword" class="submit-button" value="Submit" /><input type="submit" class="main-menu" value="Go Back" />' +
    '</div>';

$(document).ready(function () {
    $(document).mousemove(function () {
        hideSplash();
    });

    $(document).keyup(function () {
        hideSplash();
    });

    function hideSplash() {
        if (!sentinel) {
            $('#main-container').html(loginWindow);
            sentinel = true;
        }
    }

    $('#main-container').html('<div class="login-container" ><img src="img/logo.jpg" /></div>');

    $(document).on('click', '#koha', function () {
        $('#main-container').html(koha);
    });

    $(document).on('click', '#barcode', function () {
        $('#main-container').html(barcode);
    });

    $(document).on('click', '#staff', function () {
        $('#main-container').html(staff);
    });

    $(document).on('click', '.main-menu', function () {
        $('#main-container').html(loginWindow);
    });



    $(document).on('click', '#submitKoha', function (e) {
        e.preventDefault();
        var username = $('#kohaUserName').val();
        var pin = $('#kohaPin').val();
        id = querySt('id');
        Popup.Open('splash', 0);
        $.ajax({
            type: "POST",
            url: "/SessionSoftWebClient/Controller.aspx/LoginKoha",
            data: JSON.stringify({ username: username, pin: pin, id :id }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d != "True")
                {
                    Popup.Close();
                    alert(response.d);
                    $('#kohaPin').val('');
                }
            }
        });
    });

    $(document).on('click', '#submitBarcode', function (e) {
        e.preventDefault();
        var barcode = $('#libraryBarcode').val();
        id = querySt('id');
        Popup.Open('splash', 0);
        $.ajax({
            type: "POST",
            url: "/SessionSoftWebClient/Controller.aspx/LoginSIP",
            data: JSON.stringify({ barcode: barcode, id: id}),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d != "True") {
                    Popup.Close();
                    alert(response.d);
                }
            }
        });
    });

    $(document).on('click', '#submitPassword', function (e) {
        e.preventDefault();
        id = querySt('id');
        var password = $('#staffPassword').val();
        Popup.Open('splash', 0);
        if (password) {
            $.ajax({
                type: "POST",
                url: "/SessionSoftWebClient/Controller.aspx/LoginLocal",
                data: JSON.stringify({ password: password, id: id }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d != "True") {
                        Popup.Close();
                        alert(response.d);
                        $('#staffPassword').val('');
                    }
                }
            });
        }
        else {
            Popup.Close();
            alert('Please enter a password!');
        }
    });


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

});