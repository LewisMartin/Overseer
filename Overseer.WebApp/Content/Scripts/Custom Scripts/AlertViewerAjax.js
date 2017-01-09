$(document).ready(function () {
    detectAlertTypeChange();
    detectEnvironmentFilterChange();
    detectAlertFormSubmission();
    detectAlertArchive();
});

function detectAlertTypeChange() {
    $('.horiz-select-option').click(function () {
        // remove selected class from all
        $('.horiz-select-option').each(function () {
            $(this).removeClass('horiz-select-selected');
        });

        $(this).addClass('horiz-select-selected');

        $('#AlertType').val($(this).data('alrtval'));
    })
}

function detectEnvironmentFilterChange() {
    $('#EnvironmentFilter').change(function () {
        var appRoot = $('#AlertViewer').data('baseurl');
        var envId = this.value;
        var machineDropDown = $('#MachineFilter');

        machineDropDown.empty();

        $.ajax({
            type: "GET",
            url: appRoot + "/Alert/RetrieveMachineOptions",
            data: ("environmentId=" + envId),
            complete: function () {
                
            },
            success: function (updatedOptions) {
                $(updatedOptions).each(function () {
                    $(document.createElement('option'))
                        .attr('value', this.Value)
                        .text(this.Text)
                        .appendTo(machineDropDown);
                });
            }
        });
    });
}

function detectAlertFormSubmission() {
    $('#AlertFilterForm').submit(function () {
        if ($(this).valid()) {
            $('#AlertFilterForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#AlertFilterForm').find(':submit').attr("disabled", false);
                },
                success: function (data) {
                    $('#FilteredAlerts').html(data);

                    // add click event to all new matched alerts
                    detectAlertArchive();
                }
            });
        }
        return false;
    });
}

function detectAlertArchive() {
    $('.alert-archive').click(function () {
        var appRoot = $('#AlertViewer').data('baseurl');

        var alertId = $(this).data('alrtid');
        var alertContainer = whichAlert(this, 'matched-alert');
        $(alertContainer).hide();

        $.ajax({
            type: "GET",
            url: appRoot + "/Alert/ArchiveAlert",
            data: ("alertId=" + alertId),
            complete: function () {

            },
            success: function (data) {
                $('#ArchiveNotif').html(data['msg']);
            }
        });
    });
}

function whichAlert(element, selector) {
    while ((element = element.parentNode) && !element.classList.contains(selector));
    console.log(element);
    return element;
}