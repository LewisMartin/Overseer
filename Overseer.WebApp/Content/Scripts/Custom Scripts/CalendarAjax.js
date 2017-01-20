$(document).ready(function () {
    detectCalendarFormSubmission();
    detectEventSelections();
});

function detectCalendarFormSubmission() {
    $('#UpdateCalendarForm').submit(function () {
        if ($(this).valid()) {
            $('#UpdateCalendarForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#UpdateCalendarForm').find(':submit').attr("disabled", false);
                },
                success: function (data) {
                    $('#RetrievedCalendar').html(data);
                    detectEventSelections();
                }
            });
        }
        return false;
    });
}

function detectEventSelections()
{
    $('.event-true').click(function () {
        var day = $(this).data('day');
        var selector = "#event-no-" + day;

        console.log(selector);

        $('.event-detail').hide();

        $(selector).show(200);
    });
}