
var url = "/Physician/GetReceipts";
$(document).on("click", ".receipts", async function () {
    $.ajax({
        url: url,
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            physicianId: $("#physicianSelect").val(),
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
            setData(response);
        }
    })
})

$(document).on("click", ".openSheet", function () {
    localStorage.setItem("date", $(".changeDate").val());
    $.ajax({
        url: "/Physician/GetWeeklyTimeSheet",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            date: $(".changeDate").val(),
        },
        success: function (response) {
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

function setData(response) {
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
        $(".approve").attr("id", response.invoiceId);
        if (response.status == "Pending") {
            $(".status").removeClass("d-none");
        }
    }
}


//// Admin Side

function adminInvoiceGetData() {
    $.ajax({
        url: "/Admin/GetInvoice",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            physicianId: $("#physicianSelect").val(),
            date: $("#timeSelect").val(),
        },
        success: async function (response) {
            localStorage.setItem("date", $("#timeSelect").val());
            if (response.endDate != null && response.isApprove) {
                $("#statusTable").addClass("d-none");
                $.ajax({
                    url: "/Admin/GetWeeklyTimeSheet",
                    type: "Get",
                    contentType: "application/json",
                    async: true,
                    data: {
                        physicianId: $("#physicianSelect").val(),
                        date: $("#timeSelect").val(),
                    },
                    success: async function (response) {
                        $("#timeSheet").html(response);
                        url = "/Admin/GetReceipts";
                        await $(".receipts").click();
                        $(".physicianColumn").remove();
                        $("#saveButtons").html("");
                        $("#timeSheet").prop("disabled", true);
                    }
                })
            }
            else {
                $("#timeSheet").html("");
                setData(response);
            }
        }
    })
}

$(document).on("change", ".adminSelect", async function () {
    adminInvoiceGetData();
})

$(document).on("click", ".approve", function () {
    $.ajax({
        url: "/Admin/GetWeeklyTimeSheet",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            physicianId: $("#physicianSelect").val(),
            date: $("#timeSelect").val(),
        },
        success: async function (response) {
            $("#timeSheet").html(response);
            url = "/Admin/GetReceipts";
            await $(".receipts").click();
            $(".physicianColumn").remove();
            $("#statusTable").addClass("d-none");
            $("#saveButtons").html("");
            $(".adminRows").removeClass("d-none");
            $(".finalSubmit").removeClass("d-none");
            $("#timeSheet").prop("disabled", false);   
            $(".timeSheet").prop("disabled", true);
        }
    })
})

$(document).on("click", ".finalSubmit", function () {
    $.ajax({
        url: "/Admin/ApproveInvoice",
        type: "Get",
        contentType: "application/json",
        async: true,
        data: {
            invoiceId: $(this).attr("id"),
            totalAmount: $("#totalAmount").val(),
            bounsAmount: $("#bounsAmount").val(),
            notes: $("#notes").val(),
        },
        success: function (response) {
            window.location.href = response.redirect;
        }
    })
})

$(document).ready(function () {
    if ($("#AdminStatus").length) {
        adminInvoiceGetData();
    }
})