$(document).ready(function() {
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
});