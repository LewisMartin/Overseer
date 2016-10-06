$(function () {
    $('form').submit(function () {
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
                    var isSuccessful = (result['success'])

                    if (isSuccessful) {
                        $('#form-success').html(result['successmsg']);

                        var fieldsToIgnore = ["parentEnv"];

                        resetForm($('form'), fieldsToIgnore);
                        refreshSidenav();
                    }
                    else {
                        $('#dupe-machine-msg').html(result['error']);
                    }
                }
            });
        }
        return false;
    });
});

function resetForm(form, fieldIgnoreList) {
    console.log("resetForm was called..");

    if(arguments.length == 1)
    {
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
    else if (arguments.length == 2)
    {
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
    else
    {
        // invalid params given
    }
}

function refreshSidenav() {
    $.ajax({
        type: "GET",
        url: "/Navigation/_EnvironmentNavigation",
        success: function (data) {
            $('#Environments-Menu-Item>ul').html(data);
        }
    });
}