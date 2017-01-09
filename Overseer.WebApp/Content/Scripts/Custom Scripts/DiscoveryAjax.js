$(document).ready(function () {
    detectSearchTypeChange();
    detectSearchFormSubmission();
});

function detectSearchTypeChange(){
    $('.horiz-select-option').click(function () {
        // remove selected class from all
        $('.horiz-select-option').each(function () {
            $(this).removeClass('horiz-select-selected');
        });

        $(this).addClass('horiz-select-selected');

        $('#SearchType').val($(this).data('srchval'));

        toggleAdvOpsDisplay($(this).data('srchval'));
    })
}

function detectSearchFormSubmission() {
    $('#DiscoverySearchForm').submit(function () {
        if ($(this).valid()) {
            $('#DiscoverySearchForm').find(':submit').attr("disabled", true)

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                complete: function () {
                    $('#DiscoverySearchForm').find(':submit').attr("disabled", false);
                },
                success: function (data) {
                    $('#DiscoverySearchResults').html(data);
                }
            });
        }
        return false;
    });
}

function toggleAdvOpsDisplay(searchType) {
    $('#UserAdvOps').hide();
    $('#EnvironmentAdvOps').hide();
    $('#MachineAdvOps').hide();

    switch (searchType) {
        case 'user':
            $('#UserAdvOps').show();
            break;
        case 'environment':
            $('#EnvironmentAdvOps').show();
            break;
        case 'machine':
            $('#MachineAdvOps').show();
            break;
    }
}
