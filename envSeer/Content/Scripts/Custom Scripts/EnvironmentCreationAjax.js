$(function () {
    $('form').submit(function () {
        if ($(this).valid()) {
            $('form').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function() {
                    $('form').find(':submit').attr("disabled", false);
                },
                success: function (result) {
                    var isSuccessful = (result['success'])

                    if (isSuccessful) {
                        $('#form-success').html(result['successmsg']);
                        resetForm($('form'));
                        refreshSidenav();
                    }
                    else
                    {
                        $('#dupe-env-msg').html(result['error']);
                    }
                }
            });
        }
        return false;
    });
});

function resetForm(form) {
    console.log("resetForm was called..");

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

function refreshSidenav()
{
    $.ajax({
        type: "GET",
        url: "/Navigation/_EnvironmentNavigation",
        success: function (data) {
            $('#Environments-Menu-Item>ul').html(data);
        }
    });
}