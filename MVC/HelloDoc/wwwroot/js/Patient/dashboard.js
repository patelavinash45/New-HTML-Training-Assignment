function sidebar() {  /// for mobile view sidebar
    var temp = $("#header-buttom").css("display");
    if (temp == "none") {
        $("#header-buttom").css("display", "flex");
    } else {
        $("#header-buttom").css("display", "none");
    }
}

function ProfileEdit(temp) {
    if (temp) {
        $('#edit').css("display", "none");
        $('#submit').css("display", "flex");
        $('#submit').css("justifyContent", "end");
        $('#Fieldset').prop("disabled", false);
    }
    else {
        $('#Fieldset').prop("disabled", true);
        $('#edit').css("display", "flex");
        $('#edit').css("justifyContent", "end");
        $('#submit').css("display", "none");
    }
}

function changeRadio(element) {
    if (element) {
        $('.forMe').css("background-color", "cadetblue");
        $('.forMe').css("color", "white");
        $('.forSomeOne').css("background-color", "white");
        $('.forSomeOne').css("color", "cadetblue");
        $("#forMe").css("display", "block");
        $("#forSomeOne").css("display", "none");
    }
    else {
        $('.forSomeOne').css("background-color", "cadetblue");
        $('.forSomeOne').css("color", "white");
        $('.forMe').css("background-color", "white");
        $('.forMe').css("color", "cadetblue");
        $("#forSomeOne").css("display", "block");
        $("#forMe").css("display", "none");
    }
}

function NavigatToViewDocument(id) {
    $.ajax({
        url: '/Patient/SetRequestId',
        type: 'GET',
        contentType: 'application/json',
        data: { requestId: id },
        success: function (response) {
            window.location.href = response.redirect;
        }
    })
}

// header

$(document).ready(function () { 
    var id = localStorage.getItem("tab");
    if (id == null || id == "undefined") {
        id = "tab1";
    }
    $(`#${id}`).append(`<hr class="h1 m-0 text-black hrheight rounded-top "/>`);
    $(`#${id}`).addClass("text-black");
    $(`.${id}`).addClass("text-black");
})

$(document).on("click", ".headerTab", function () {
    localStorage.setItem("tab", $(this).attr("id"));
})
$(document).on("click", "#log-out", function () {
    localStorage.removeItem("tab");
    localStorage.removeItem("status");
})

function setTab(temp) {
    localStorage.setItem("tab", `tab${temp}`);
}