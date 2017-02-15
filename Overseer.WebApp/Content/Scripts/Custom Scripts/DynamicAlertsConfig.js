$(document).ready(function () {
    var hideTimeout;    // variable to hold timeout hide() of form response

    $('#OpenDynamicAlertsConfig').click(function () {
        detectDynamicMonitoringSelection();

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
                $('#DynamicAlertsConfig').html(data);

                $('#DynamicAlertsConfig').show();

                $("html, body").animate({ scrollTop: $(document).height() }, 1000);

                // add event handler to 'close' button
                $('#CloseDynamicAlertsConfig').click(function () {
                    console.log("close");

                    $('#DynamicAlertsConfig').hide(200);

                    toggleEnvConfigForm(true);
                });

                $('#AlertsConfigForm').submit(function () {
                    // clear any existing timeout & immediately hide
                    resetFormResponse();

                    var formResponse = $('#AlertsConfigForm .form-response');
                    var successMsg = $('#AlertsConfigForm .form-success');
                    var errorMsg = $('#AlertsConfigForm .form-failure');

                    console.log(errorMsg);
                    console.log(successMsg);

                    if ($(this).valid()) {
                        $('#AlertsConfigForm').find(':submit').attr("disabled", true);

                        $.ajax({
                            url: this.action,
                            type: this.method,
                            data: $(this).serialize(),
                            complete: function () {
                                $('#AlertsConfigForm').find(':submit').attr("disabled", false);
                            },
                            success: function (result) {
                                if (result['success']) {
                                    $(successMsg).html(result['successmsg']);
                                }
                                else {
                                    $(errorMsg).html(result['errormsg']);
                                }

                                $(formResponse).show(200);
                                hideTimeout = setTimeout(function () { $(formResponse).hide(200); }, 10000)
                            }
                        });
                    }
                    return false;   // stops non-ajax submission
                });
            }
        });
    });

    function detectDynamicMonitoringSelection() {
        $('#DynamicAlertsConfig').on('change', '#DynamicConfigChooser', function () {
            console.log("got em");

            console.log($(this).val());
            console.log($(this).children("option").filter(":selected").text());

            // ensure all dynamic alert config sections are hidden
            $('.dyn-config').hide();

            $('.dyn-config[data-type="' + $(this).val() + '"][data-config="' + $(this).children("option").filter(":selected").text() + '"]').show();
        });
    }

    function toggleEnvConfigForm(state) {
        if (state) {
            // enable form
            $('#EnvironmentAjaxForm').find(':submit').attr("disabled", false);
        } else {
            // disable form
            $('#EnvironmentAjaxForm').find(':submit').attr("disabled", true);
        }
    }

    function resetFormResponse() {
        clearTimeout(hideTimeout);
        $('#AlertsConfigForm .form-failure').html("");
        $('#AlertsConfigForm .form-success').html("");
        $('#AlertsConfigForm .form-response').hide();
    }
});