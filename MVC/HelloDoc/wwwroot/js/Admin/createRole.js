$(document).on("change", "#type", function () {
    if ($(this).val().length != 0) {
        $.ajax({
            url: "/Admin/ChangeMenusByRole",
            type: "Get",
            contentType: "application/json",
            data: {
                roleId: $(this).val(),
            },
            success: function (response) {
                $("#menuValidation").text("");
                $(".checkBoxs").html(response);
            }
        })
    }
})


$(document).on("submit", "#createRoleForm", function (e) {
    var selectedMenus = false;
    $(".form-check-input:checked").each(function () {
        selectedMenus = true;
    });
    if (!selectedMenus && $("#type").val() != 1) {
        e.preventDefault();
        $("#menuValidation").text("Select Any One Filed");
    }
})

/// create Admin - page

$(document).on("submit", "#createdAdminForm", function (e) {
    if (!$(".regioncheckBoxs").is(":checked")) {
        e.preventDefault();
        $("#checkboxValidation").text("This Region Field is required.");
    }
})

$(document).on("change", ".regioncheckBoxs", function () {
    if (!$(".regioncheckBoxs").is(":checked")) {
        $("#checkboxValidation").text("This Region Field is required.");
    }
    else {
        $("#checkboxValidation").text("");
    }
})