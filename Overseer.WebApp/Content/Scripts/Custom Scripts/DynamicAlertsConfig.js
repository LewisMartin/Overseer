$(document).ready(function () {

    $('#OpenDynamicAlertsConfig').click(function () {
        console.log("here");

        var appRoot = $('#MachineConfig').data('baseurl');
        var machineId = $('#MachineConfig').data('machineid');

        $.ajax({
            type: "GET",
            url: appRoot + "/Machine/_DynamicMonitoringAlertsConfig",
            data: ("machineId=" + machineId),
            success: function (data) {
                // disable environment config form
                toggleEnvConfigForm(false);

                // populate parent with partial view contents
                $('#DynamicAlertsConfig>div').html(data);

                // add event handler to 'close' button
                $('#CloseDynamicAlertsConfig').click(function () {
                    console.log("close");
                    $('#DynamicAlertsConfig>div').html("");

                    toggleEnvConfigForm(true);
                });

                $('#AlertsConfigForm').submit(function () {
                    if ($(this).valid()) {
                        $('#AlertsConfigForm').find(':submit').attr("disabled", true)

                        $.ajax({
                            url: this.action,
                            type: this.method,
                            data: $(this).serialize(),
                            complete: function () {
                                $('form').find(':submit').attr("disabled", false);
                            },
                            success: function (result) {

                                var isSuccessful = (result['success'])

                                if (isSuccessful) {
                                    $('#form-success').html(result['successmsg']);

                                    $('#DynamicAlertsConfig>div').html("");

                                    toggleEnvConfigForm(true);
                                }
                                else {
                                    $('#dupe-env-msg').html(result['error']);
                                }
                            }
                        });
                    }
                    return false;   // stops non-ajax submission
                });
            }
        });
    });

    function toggleEnvConfigForm(state)
    {
        if(state)
        {
            // enable form
            $('#EnvironmentAjaxForm').find(':submit').attr("disabled", false);
        } else {
            // disable form
            $('#EnvironmentAjaxForm').find(':submit').attr("disabled", true);
        }
    }
});