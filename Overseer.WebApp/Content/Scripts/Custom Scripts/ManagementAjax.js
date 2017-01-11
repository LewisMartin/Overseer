$(document).ready(function () {
    detectEventCreationFormSubmission();
});

function detectEventCreationFormSubmission() {
    $('#CreateCalendarEventForm').submit(function () {
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
                    var isSuccessful = (result['success'])

                    if (isSuccessful) {
                        $('#form-success').html(result['successmsg']);
                    }
                    else {
                        errorMsg.html('error');
                    }
                }
            });
        }
        return false;
    });
}