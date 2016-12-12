$(document).ready(function () {

    // enable/disable field on document ready
    $('.tf-checkbox').each(function () {
        if ($(this).is(':checked')) {
            $(this).siblings('.tf-target').prop('disabled', false);
            $(this).siblings('.tf-inverse-target').prop('disabled', true);
        }
        else {
            $(this).siblings('.tf-target').prop('disabled', true);
            $(this).siblings('.tf-inverse-target').prop('disabled', false);
        }
    })

    // enable/disable field whenever checkbox is changed
    $('.tf-checkbox').change(function () {
        if (this.checked) {
            $(this).siblings('.tf-target').prop('disabled', false);
            $(this).siblings('.tf-inverse-target').prop('disabled', true);
        }
        else {
            $(this).siblings('.tf-target').prop('disabled', true);
            $(this).siblings('.tf-inverse-target').prop('disabled', false);
        }
    });
});