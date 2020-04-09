var Popup = function () {	
	function showPopup (popupContent, dialog) {					
	    $('#popup-content').removeClass('loading-content-class');
	    $('#popup-content').removeClass('popup-content-class');
	        if ($('#overlay').length == 0)
			{
  				$('body').append('<div id="overlay"></div>');
			}		
			if (dialog) {
			    $('#pop-up').html('<div id="popup-content" class="popup-content-class">' + popupContent + '</div>');
			    $('#overlay').addClass('light');
			}
			else {
			    $('#pop-up').html('<div id="popup-content" class="loading-content-class">' +
                                  '<img src="images/loading.png" class="loading" /></div>');
			    $('#overlay').addClass('dark');
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
