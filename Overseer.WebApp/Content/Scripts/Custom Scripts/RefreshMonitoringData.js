var interval = $('#Machineseer').data('refreshrate');
//window.alert(interval);

function refresh() {
    var appRoot = $('#Machineseer').data('baseurl');
    var machineId = $('#Machineseer').data('machineid');

    $.ajax({
        type: "GET",
        url: appRoot + "/Machine/_MonitoringSystemInfo",
        data: ("machineId="+machineId),
        success: function (data) {
            $('#SystemInfo').html(data);
        }
    });

    $.ajax({
        type: "GET",
        url: appRoot + "/Machine/_MonitoringSummary",
        data: ("machineId="+machineId),
        success: function (data) {
            $('#MonitoringData').html(data);
        }
    });

    // change interval if necessary here

    setTimeout(refresh, interval);
}

setTimeout(refresh, interval);