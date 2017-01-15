$(function () {
    var hideTimeout;

    $('#EnvironmentAjaxForm').submit(function () {
        var form = $('#EnvironmentAjaxForm').data('ajax');

        // clear any existing timeout & immediately hide
        resetFormResponse();

        var formReset;
        var successMsg = $('#EnvironmentAjaxForm #form-success');
        var errorMsg = $('#EnvironmentAjaxForm #form-failure')

        switch (form) {
            case "EnvCreate":
                formReset = true;
                break;
            case "EnvConfig":
                formReset = false;
                break;
            case "MachineCreate":
                formReset = true;
                break;
            case "MachineConfig":
                formReset = false;
                toggleMonitoringLists(true);
                break;
        }

        if ($(this).valid()) {
            $('#EnvironmentAjaxForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#EnvironmentAjaxForm').find(':submit').attr("disabled", false);
                },
                success: function (result) {
                    toggleMonitoringLists(false);

                    if (result['success']) {
                        successMsg.html(result['successmsg']);

                        // reset form fields if necessary
                        if (formReset) {
                            var fieldsToIgnore = ["parentEnv"];
                            resetForm($('#EnvironmentAjaxForm'), fieldsToIgnore);
                        }

                        // refresh ' environment' section of side bar to reflect changes
                        refreshSidenav();
                    }
                    else {
                        errorMsg.html(result['error']);
                    }

                    $('#form-response').show(200);
                    hideTimeout = setTimeout(function () { $('#form-response').hide(200); }, 10000)
                }
            });
        }
        return false;
    });

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

    function refreshSidenav() {
        var appRoot = $('.form-footer > .pag-btn').data('baseurl');

        $.ajax({
            type: "GET",
            url: appRoot + "/Navigation/_EnvironmentNavigation",
            success: function (data) {
                $('#Environments-Menu-Item>ul').html(data);
            }
        });
    }

    function toggleMonitoringLists(toggle) {
        $('.mon-list-box').each(function () {
            $(this).children('option').each(function () {
                $(this).prop("selected", toggle);
            })
        });
    }

    function resetFormResponse() {
        clearTimeout(hideTimeout);
        $('#form-failure').html("");
        $('#form-success').html("");
        $('#form-response').hide();
    }
});
