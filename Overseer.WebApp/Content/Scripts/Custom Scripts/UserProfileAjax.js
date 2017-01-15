$(document).ready(function () {
    var hideTimeout;

    $('#ProfileEditorForm').submit(function () {
        // clear any existing timeout & immediately hide
        resetFormResponse();

        if ($(this).valid()) {
            $('#ProfileEditorForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#ProfileEditorForm').find(':submit').attr("disabled", false);
                },
                success: function (result) {
                    if (result['success']) {
                        $('#form-success').html(result['responsemsg']);
                    } else {
                        $('#form-failure').html(result['responsemsg']);
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
});