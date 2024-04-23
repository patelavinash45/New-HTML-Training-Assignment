function enableEdit(temp) {
    if (temp) {
        $("#editbutton").css("display", "none");
        $(".edit").css("display", "block");
        $("#fieldset").prop("disabled", false);
    }
    else {
        $("#editbutton").css("display", "block");
        $(".validation").text("");
        $(".edit").css("display", "none");
        $("#fieldset").prop("disabled", true);
    }
}

$(function () {
    $('[type="date"]').prop('max', function () {
        return new Date().toJSON().split('T')[0];
    });
});

var isArrowUp = true;
function shortTable() {
    if (isArrowUp) {
        isArrowUp = false;
        $(".bi-arrow-up").css("display", "none");
        $(".bi-arrow-down").css("display", "inline-block");
    }
    else {
        isArrowUp = true;
        $(".bi-arrow-up").css("display", "inline-block");
        $(".bi-arrow-down").css("display", "none");
    }
    $('tbody').each(function () {
        var list = $(this).children('tr');
        $(this).html(list.get().reverse());
    });
}