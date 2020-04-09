var Popup = function () {	
	function showPopup (popupContent, dialog) {					
	    $('#popup-content').removeClass('loading-content-class');
	    $('#popup-content').removeClass('popup-content-class');
	        if ($('#overlay').length == 0)
			{
  				$('body').append('<div id="overlay"></div>');
			}		
			if (dialog) {
			    $('#pop-up').html('<img src="/SessionSoftWebClient/img/white-X.png" id="close-popup" alt="Close" />' +
                          '<div id="popup-content" class="popup-content-class">' + popupContent + '</div>');
			    $('#overlay').addClass('dark');
			}
			else {
			    $('#pop-up').html('<div id="popup-content" class="loading-content-class">' +
                                  '<img src="/SessionSoftWebClient/img/loading.png" class="loading" /></div>');
			    $('#overlay').addClass('light');
			}		
			$('#pop-up').show();
		}
	
	function closePopup (callback) {
	    $('#pop-up').hide();
	    $('#overlay').removeClass('dark');
	    $('#overlay').removeClass('light');
	    if (callback) { return callback(); }
	    else { return true; }
	}
	
	return {
		Open : showPopup,
		Close : closePopup	
	} 
}();
	
$('#pop-up').ready(function () {
	$('#pop-up').on('click','#close-popup', function () {
	    Popup.Close();
	});
});	
