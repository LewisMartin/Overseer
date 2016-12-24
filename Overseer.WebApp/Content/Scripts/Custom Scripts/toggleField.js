$(document).ready(function () {

    // enable/disable field on document ready
    $('.tf-checkbox').each(function () {
        var toggle = whichToggle(this, 'tf-container');
        var targets = $(toggle).find('.tf-target');
        var invertedTargets = $(toggle).find('.tf-inverse-target');

        if ($(this).is(':checked')) {
            $(targets).each(function () { $(this).prop('disabled', false) });
            $(invertedTargets).each(function () { $(this).prop('disabled', true) });
        }
        else {
            $(targets).each(function () { $(this).prop('disabled', true) });
            $(invertedTargets).each(function () { $(this).prop('disabled', false) });
        }
    })

    // enable/disable field whenever checkbox is changed
    $('.tf-checkbox').change(function () {
        console.log("toggle");

        var toggle = whichToggle(this, 'tf-container');
        var targets = $(toggle).find('.tf-target');
        var invertedTargets = $(toggle).find('.tf-inverse-target');

        if (this.checked) {
            $(targets).each(function () { $(this).prop('disabled', false) });
            $(invertedTargets).each(function () { $(this).prop('disabled', true) });
        }
        else {
            console.log("disabling");
            $(targets).each(function () { $(this).prop('disabled', true) });
            $(invertedTargets).each(function () { $(this).prop('disabled', false) });
        }
    });

    // method to find which accordion to change content of
    function whichToggle(element, selector) {
        while ((element = element.parentNode) && !element.classList.contains(selector));
        console.log(element);
        return element;
    }
});