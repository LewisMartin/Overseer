$(document).ready(function () {

	// Top Menu Bar toggle functionality
	$('#Nav-Toggle').on('change', function () {

		if($('#Nav-Toggle').is(':checked'))
		{
			// hide nav
			$('#Dashboard-Nav').css(
				'left', '-180px'
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
				'padding-left', '180px'
			);
		}

	});

    // Sidebar 1st level menu drop down & animation:
	$('.menu-drop-down').click(function () {
	    console.log('menu item clicked!');

	    // get this menu item's drop down list
	    var subMenu = $(this).siblings('.nav-sub-menu');

	    console.log($(subMenu).height());

	    // animate height and apply 'selected' class based on whether sub-menu is currently open/closed
	    if ($(subMenu).height() === 0) {
	        console.log('increasing height');
	        subMenu.css('height', 'auto');
	        subMenu.addClass('nav-selected');
	        subMenu.siblings('.menu-drop-down').addClass('nav-selected');
	    }
	    else {
	        console.log('back to 0');
	        subMenu.css('height', '0');
	        subMenu.removeClass('nav-selected');
	        subMenu.siblings('.menu-drop-down').removeClass('nav-selected');
	    }
	})

});

// now just need to find out how to get it to hide again
$('#Environments-Menu-Item').on('mouseover', '.env-menu-item', function () {
    $(this).find('.pop-out-menu').css('height', 'auto');
});
$('#Environments-Menu-Item').on('mouseout', '.env-menu-item', function () {
    $(this).find('.pop-out-menu').css('height', '0');
});
