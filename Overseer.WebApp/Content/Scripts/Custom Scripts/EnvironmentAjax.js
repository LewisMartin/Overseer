$(function () {
    $('form').submit(function () {
        console.log("submitted");

        var form = $('form').data('ajax');

        var formReset;
        var successMsg;
        var errorMsg;

        switch (form) {
            case "EnvCreate":
                formReset = true;
                successMsg = $('#form-success');
                errorMsg = $('#dupe-env-msg');
                break;
            case "EnvConfig":
                formReset = false;
                successMsg = $('#form-success');
                errorMsg = $('#dupe-env-msg');
                break;
            case "MachineCreate":
                formReset = true;
                successMsg = $('#form-success');
                errorMsg = $('#dupe-machine-msg');
                break;
            case "MachineConfig":
                formReset = false;
                successMsg = $('#form-success');
                errorMsg = $('#dupe-machine-msg');
                break;
        }

        toggleMonitoringLists(true);

        if ($(this).valid()) {
            $('form').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('form').find(':submit').attr("disabled", false);
                },
                success: function (result) {
                    toggleMonitoringLists(false);

                    var isSuccessful = (result['success'])

                    if (isSuccessful) {
                        successMsg.html(result['successmsg']);

                        if (formReset) {
                            var fieldsToIgnore = ["parentEnv"];
                            resetForm($('form'), fieldsToIgnore);
                        }

                        refreshSidenav();
                    }
                    else {
                        errorMsg.html(result['error']);
                    }
                }
            });
        }
        return false;
    });
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

