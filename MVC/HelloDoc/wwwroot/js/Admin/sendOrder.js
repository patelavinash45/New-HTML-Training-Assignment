function changeSelect() {
    $.ajax({
        url: "/Admin/GetBusinesses",
        type: "Get",
        contentType: "application/json",
        data: {
            professionId: $('#selectedProfession').val(),
        },
        success: function (response, xhr) {
            $("#selectedBusiness").children().remove();
            $("#selectedBusiness").append('<option disabled selected value="">Business</option>');
            $.each(response, function (index, item) {
                var option = `<option value="${index}">${item}</option>`;
                $("#selectedBusiness").append(option);
            });
            $(".contact").val("");
            $(".email").val("");
            $(".faxNumber").val("");
        }
    });
}

function changeBussiness() {
    var venderId = $('#selectedBusiness').val();
    if (venderId != null) {
        $.ajax({
            url: "/Admin/GetBusinessData",
            type: 'Get',
            contentType: 'appliaction/json',
            data: {
                venderId: venderId,
            },
            success: function (response) {
                var valus = Object.values(response);
                $(".contact").val(valus[15]);
                $(".email").val(valus[14]);
                $(".faxNumber").val(valus[3]);
            }
        })
    }
    else {
        $(".contact").val("");
        $(".email").val("");
        $(".faxNumber").val("");
    }
}

