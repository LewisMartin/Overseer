$(document).ready(function () {
    var activeSubmits;
    startDelListener();

    function startDelListener() {
        deleteEvent();
        confirmDeleteEvent();
        deleteConfirmedEvent();
    }

    function stopDelListener() {
        $('.initial-del').off('click');
        $('.confirm-del').off('click');
        $('.del-confirmed').off('click');
    }

    function deleteEvent() {
        $('.initial-del').on('click', function () {
            // disable all other forms on the page
            activeSubmits = $('input[type=submit]:enabled');
            $(activeSubmits).prop('disabled', true);

            $('.confirm-del').show(200);
        });
    }

    function confirmDeleteEvent() {
        $('.del-cancel').on('click', function () {
            $('.confirm-del').hide(200);

            $(activeSubmits).prop('disabled', false);
        });
    }

    function deleteConfirmedEvent() {
        $('.del-confirmed').on('click', function () {
            stopDelListener();   // disable click event to prevent repeat submission

            var AppRoot;
            var DelUrl;
            var ReqData;
            var Container;

            $('.confirm-del').hide(200);

            // clear any existing response message
            resetFormResponse();

            switch ($('.confirm-del').data('deltype')) {
                case "Machine":
                    Container = "#MachineConfig";
                    AppRoot = $('#MachineConfig').data('baseurl');
                    DelUrl = AppRoot + "/Machine/MachineDeletion";
                    console.log(DelUrl);
                    ReqData = "machineId=" + $('#MachineConfig').data('machineid');
                    break;
                case "Environment":
                    Container = "#EnvironmentConfig";
                    AppRoot = $('#EnvironmentConfig').data('baseurl');
                    DelUrl = AppRoot + "/Environment/EnvironmentDeletion";
                    console.log(DelUrl);
                    ReqData = "environmentId=" + $('#EnvironmentConfig').data('environmentid');
                    break;
            }

            $.ajax({
                type: "GET",
                url: DelUrl,
                data: ReqData,
                success: function (result) {

                    if (result['success']) {
                        // disable all links within configuration section
                        $((Container + " *")).off();
                        $((Container + " a")).remove();

                        console.log(result['successmsg']);

                        $('#DeleteResponse .del-success').text(result['successmsg']);

                        $('#DeleteResponse').show(200);

                        // refresh ' environment' section of side bar to reflect changes
                        refreshSidenav();
                    }
                    else {
                        $('#DeleteResponse > .del-failure').text(result['error']);

                        // re-enable configuration form
                        $(activeSubmits).prop('disabled', false);

                        // restart deletion listener
                        startDelListener();
                    }
                }
            });
        });
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

    function resetFormResponse() {
        $('#DeleteResponse > .del-failure').html("");
        $('#DeleteResponse > .del-success').html("");
        $('#DeleteResponse').hide();
    }
});