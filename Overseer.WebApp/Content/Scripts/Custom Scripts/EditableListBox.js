$(document).ready(function () {

    $('.mon-list-add').click(function () {
        console.log("Add Clicked");

        var listParent = whichList(this, 'dynamic-mon-list');

        var value = $(listParent).find('.mon-list-input').val();

        $(listParent).find('.mon-list-box').append('<option value="' + value + '">' + value + '</option>');
    });

    $('.mon-list-remove').click(function () {
        console.log("Remove Clicked");
        var listParent = whichList(this, 'dynamic-mon-list');

        $(listParent).find('.mon-list-box').children('option:selected').each(function () {
            console.log($(this).val());
            $(this).remove();
        });
    });

    // method to find which list was edited
    function whichList(element, selector) {
        while ((element = element.parentNode) && !element.classList.contains(selector));
        console.log(element);
        return element;
    }
});