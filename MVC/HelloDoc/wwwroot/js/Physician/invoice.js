$(document).on("click", ".receipts", function () {
    $.ajax({
        url: "/Physician/GetReceipts",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            time: "",
        },
        success: function (response) {
            $("#receipts").html(response);
        }
    })
})

$(document).on("change", ".fileInput", function () {
    var id = $(this).attr("id");
    $(this).addClass("d-none");
    $(`#${id} .fileName`).text($(this).val());
    $(`#${id} .buttons`).removeClass("d-none");
    $(`#${id} .item`).css("border", "none");
    $(`#${id} .item`).prop("disabled", true);
    $(`#${id} .amount`).css("border", "none");
    $(`#${id} .amount`).prop("disabled", true);
})

$(document).on("click", ".editButton", function () {

    $(this).addClass("d-none");
    $(`#${id} .fileName`).text($(this).val());
    $(`#${id} .buttons`).removeClass("d-none");
    $(`#${id} .item`).css("border", "none");
    $(`#${id} .item`).prop("disabled", true);
    $(`#${id} .amount`).css("border", "none");
    $(`#${id} .amount`).prop("disabled", true);
}