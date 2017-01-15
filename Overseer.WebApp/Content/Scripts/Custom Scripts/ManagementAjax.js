$(document).ready(function () {
    var hideTimeout;

    $('#CreateCalendarEventForm').submit(function () {
        // clear any existing timeout & immediately hide
        resetFormResponse();

        if ($(this).valid()) {
            $('#CreateCalendarEventForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#CreateCalendarEventForm').find(':submit').attr("disabled", false);
                },
                success: function (result) {
                    if (result['success']) {
                        $('#form-success').html(result['successmsg']);
                        resetForm($('#EnvironmentAjaxForm'), ["parentEnv"]);
                    } else {
                        $('#form-failure').html(result['errormsg']);
                    }
                    $('#form-response').show(200);
                    hideTimeout = setTimeout(function () { $('#form-response').hide(200); }, 10000)
                }
            });
        }
        return false;
    });

    function resetFormResponse() {
        clearTimeout(hideTimeout);
        $('#form-failure').html("");
        $('#form-success').html("");
        $('#form-response').hide();
    }

    function resetForm(form, fieldIgnoreList) {
        console.log("resetForm was called..");

        if (arguments.length == 1) {
            $(form).find(':input').each(function () {
                console.log("input found!");

                switch (this.type) {
                    case 'select-one':
                    case 'text':
                        $(this).val('');
                    case 'checkbox':
                        this.checked = false;
                }
            });
        }
        else if (arguments.length == 2) {
            $(form).find(':input').each(function () {
                console.log("input found!");

                if (fieldIgnoreList.indexOf($(this).attr('id')) != -1) {
                    // do not reset this field
                }
                else {
                    switch (this.type) {
                        case 'select-one':
                        case 'text':
                        case 'number':
                            $(this).val('');
                        case 'checkbox':
                            this.checked = false;
                    }
                }
            });
        }
        else {
            // invalid params given
        }
    }
});