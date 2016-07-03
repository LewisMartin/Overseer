$(document).ready(function(){

	// Top Menu Bar toggle functionality
	$('#Nav-Toggle').on('change', function () {

		if($('#Nav-Toggle').is(':checked'))
		{
			// hide nav
			$('#Dashboard-Nav').css(
				'left', '-160px'
			);

			//extend main content area
			$('body').css(
				'padding-left', '0px'
			);

		} else {
			// show nav
			$('#Dashboard-Nav').css(
				'left', '0px'
			);

			//retract main content area
			$('body').css(
				'padding-left', '160px'
			);
		}

	});

	// functions for the side nav bar 
	// probably at some point you'll want to write the checkbox handling for sub-menus of the sidebar nav in javascript to make it a smoother transition
	$('#Sub-Nav-Toggle').css('change', function(){
		
	})
});
