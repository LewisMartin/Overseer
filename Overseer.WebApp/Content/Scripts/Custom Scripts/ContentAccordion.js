$(document).ready(function () {

    // whenever an accord-toggle button/div is clicked
    $('.accord-toggle').click(function ()
    {
        var accord = whichAccord(this, 'content-accord');
        var accordContent = $(accord).find('.accord-content');

        if ($(accordContent).hasClass('accord-content-hidden')) {
            $(accordContent).animate({height: $(accordContent).get(0).scrollHeight}, 600);
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