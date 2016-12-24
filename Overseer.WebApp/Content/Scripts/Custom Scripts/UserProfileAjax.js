$(document).ready(function () {
    $('#ProfileEditorForm').submit(function () {
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
                    $('#responsemsg').html(result['responsemsg']);
                }
            });
        }
        return false;
    });
});