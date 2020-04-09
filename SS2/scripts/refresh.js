function setIdle(cb, seconds) {
    var timer; 
    var interval = seconds * 1000;
    function refresh() {
            clearInterval(timer);
            timer = setTimeout(cb, interval);
    };
    $(document).on('keypress click mousemove', refresh);
    refresh();
}

setIdle(function() {
    var region = $('#region-name').text();
    location.href = window.location.pathname + "?" + $.param({ 'region': region });
}, 5);