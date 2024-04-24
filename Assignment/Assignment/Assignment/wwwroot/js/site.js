
var records = 5;

function getTableData(pageNo) {
    $.ajax({
        url: "Student/GetTabledata",
        type: 'GET',
        contentType: 'application/json',
        data: {
            pageNo: pageNo,
            records: records,
            searchElement: $("#searchStudent").val(),
        },
        success: function (response) {
            $(".tabledata").html(response);
        }
    })
}

$(document).on("keyup", "#searchStudent", function () {
    getTableData(1);
})

$(document).on("change", "#records", function () {
    records = $(this).val();
    getTableData(1);
})

function EditStudent(id) {
    $.ajax({
        url: "Student/GetStudentData",
        type: 'GET',
        contentType: 'application/json',
        data: {
            id: id,
        },
        success: function (response) {
            $("#firstName").val(response.data["firstname"]);
            $("#lastname").val(response.data["lastname"]);
            $("#email").val(response.data["email"]);
            $("#date").val(response.data["date"]);
            $("#grade").val(response.data["grade"]);
            $("#course").val(response.data["course"]);
            $(".form-check-input").prop("checked", false);
            if (response.data["gender"] == 1) {
                $("#flexRadioDefault1").prop("checked", true);
            }
            else if (response.data["gender"] == 2) {
                $("#flexRadioDefault2").prop("checked", true);
            }
            else {
                $("#flexRadioDefault3").prop("checked", true);
            }
        }
    })
}