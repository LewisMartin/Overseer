var activeUserOps = 0;
var activeEnvOps = 0;
var activeMachineOps = 0;

$(document).ready(function () {
    detectSearchTypeChange();
    detectSearchFormSubmission();
    updateActiveSearchOptions();
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
            $(".active-search-options").html(activeUserOps + ' active');
            break;
        case 'environment':
            $('#EnvironmentAdvOps').show();
            $(".active-search-options").html(activeEnvOps + ' active');
            break;
        case 'machine':
            $('#MachineAdvOps').show();
            $(".active-search-options").html(activeMachineOps + ' active');
            break;
    }
}

function updateActiveSearchOptions() {
    $(".user-search-option").change(function () {
        if (this.checked) {
            activeUserOps++;
            $(".active-search-options").html(activeUserOps + ' active');
        } else {
            activeUserOps--;
            $(".active-search-options").html(activeUserOps + ' active');
        }
    })

    $(".env-search-option").change(function () {
        if (this.checked) {
            activeEnvOps++;
            $(".active-search-options").html(activeEnvOps + ' active');
        } else {
            activeEnvOps--;
            $(".active-search-options").html(activeEnvOps + ' active');
        }
    })

    $(".machine-search-option").change(function () {
        if (this.checked) {
            activeMachineOps++;
            $(".active-search-options").html(activeMachineOps + ' active');
        } else {
            activeMachineOps--;
            $(".active-search-options").html(activeMachineOps + ' active');
        }
    })
}