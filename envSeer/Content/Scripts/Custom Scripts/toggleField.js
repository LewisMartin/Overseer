$(document).ready(function () {
    if ($('#tf-checkbox').is(':checked'))
    {
        $('.tf-target').prop('disabled', false);
    }
    else
    {
        $('.tf-target').prop('disabled', true);
    }

    $('#tf-checkbox').change(function () {
        if (this.checked) {
            $('.tf-target').prop('disabled', false);
        }
        else {
            $('.tf-target').prop('disabled', true);
        }
    });
});