$(document).ready(function () {

    $('.mon-list-add').click(function () {
        console.log("Add Clicked");
        var value = $(this).siblings('.mon-list-input').val();

        console.log(value);

        $(this).siblings('.mon-list-box').append('<option value="' + value + '">' + value + '</option>');
    });

    $('.mon-list-remove').click(function () {
        console.log("Remove Clicked");
        $(this).siblings('.mon-list-box').children('option:selected').each(function () {
            console.log($(this).val());
            $(this).remove();
        });
    });

});