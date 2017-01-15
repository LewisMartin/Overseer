$(document).ready(function () {

    // whenever an accord-toggle button/div is clicked
    $('.accord-toggle').on('click', function ()
    {
        var toggle = $(this);
        var accord = whichAccord(this, 'content-accord');

        // remove toggle if accordion is single user
        if($(accord).hasClass('accord-single-use')){
            $(toggle).removeClass('accord-toggle').off('click');
        }

        var accordContent = $(accord).find('.accord-content');

        if ($(accordContent).hasClass('accord-content-hidden')) {
            $(accordContent).animate({ height: $(accordContent).get(0).scrollHeight }, 600, function () {
                $(accordContent).css('height', 'auto'); // reset to auto height (for responsiveness)
            });
        } else {
            $(accordContent).animate({ height: 0 }, 600);
        }

        accordContent.toggleClass('accord-content-hidden');
    });


    // method to find which accordion to change content of
    function whichAccord(element, selector) {
        while ((element = element.parentNode) && !element.classList.contains(selector));
        console.log(element);
        return element;
    }
});