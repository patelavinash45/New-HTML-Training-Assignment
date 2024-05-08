$(document).on("click", ".receipts", function () {
    $.ajax({
        url: "/Physician/GetReceipts",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            date: localStorage.getItem("date"),
        },
        success: function (response) {
            $("#receipts").html(response);
        }
    })
})

$(document).on("change", ".changeDate", function () {
    $.ajax({
        url: "/Physician/GetInvoice",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            date: $(this).val(),
        },
        success: function (response) {
            if (response.endDate == null) {
                $("#statusTable").addClass("d-none");
                $(".noData").removeClass("d-none");
            }
            else {
                $(".noData").addClass("d-none");
                $("#statusTable").removeClass("d-none");
                $("#startDate").html(response.startDate);
                $("#endDate").html(response.endDate);
                $("#status").html(response.status);
                if (response.status == "Pending") {
                    $(".status").removeClass("d-none");
                }
            }
        }
    })
})

$(document).on("click", ".openSheet", function () {
    $.ajax({
        url: "/Physician/GetWeeklyTimeSheet",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            date: $(".changeDate").val(),
        },
        success: function (response) {
            localStorage.setItem("date", $(".changeDate").val());
            $("#timeSheet").html(response);
        }
    })
})

$(document).on("change", ".fileInput", function () {
    showButtons(this);
})
$(document).on("change", ".item", function () {
    showButtons(this);
})
$(document).on("change", ".amount", function () {
    showButtons(this);
})

$(document).on("click", ".editButton", function () {
    var id = $(this).attr("id");
    $(`#${id} .buttons`).addClass("d-none");
    $(`#${id} .fileName`).addClass("d-none");
    $(`#${id} .fileInput`).removeClass("d-none");
    $(`#${id} .item`).prop("readonly", false);
    $(`#${id} .amount`).prop("readonly", false);
})

$(document).on("click", ".deleteButton", function () {
    var id = $(this).attr("id");
    $(`#${id} .buttons`).addClass("d-none");
    $(`#${id} .fileName`).addClass("d-none");
    $(`#${id} .fileInput`).removeClass("d-none");
    $(`#${id} .fileInput`).val("");
    $(`#${id} .item`).prop("readonly", false);
    $(`#${id} .item`).val("");
    $(`#${id} .amount`).prop("readonly", false);
    $(`#${id} .amount`).val("");
})

$(document).on("click", ".viewButton", function () {
    var id = $(this).attr("id");
    var link = $(`#${id} .fileInput`).val();
    window.open(link, "_blank");
})

function showButtons(temp) {
    var id = $(temp).attr("id");
    var item = $(`#${id} .item`).val();
    var amount = $(`#${id} .amount`).val();
    var file = $(`#${id} .fileInput`).val();
    if (item.length > 0 && amount.length > 0 && file.length > 0 && amount != 0) {
        $(`#${id} .fileInput`).addClass("d-none");
        $(`#${id} .fileName`).text($(`#${id} .fileInput`).val());
        $(`#${id} .buttons`).removeClass("d-none");
        $(`#${id} .item`).prop("readonly", true);
        $(`#${id} .amount`).prop("readonly", true);
    }
}

$(document).on("submit", "#invoiceForm", function (e) {
    console.log("hi");
    $(".reciptsTable").each(function () {
        var id = $(this).attr("id");
        if (!$(`#${id} .buttons`).hasClass("d-none")) {
            var item = $(`#${id} .item`).val();
            var amount = $(`#${id} .amount`).val();
            var file = $(`#${id} .fileInput`).val();
            if (item.length <= 0 || amount.length <= 0 || file.length <= 0 || amount == 0) {
                $(`#${id} .validation`).removeClass("d-none");
                e.preventDefault();
            }
        }
    })
})