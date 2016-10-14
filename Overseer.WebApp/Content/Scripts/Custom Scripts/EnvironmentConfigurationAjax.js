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
                        refreshSidenav();
                    }
                    else {
                        $('#dupe-env-msg').html(result['error']);
                    }
                }
            });
        }
        return false;
    });
});

function refreshSidenav() {
    $.ajax({
        type: "GET",
        url: "/Navigation/_EnvironmentNavigation",
        success: function (data) {
            $('#Environments-Menu-Item>ul').html(data);
        }
    });
}