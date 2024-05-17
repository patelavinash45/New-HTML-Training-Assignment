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

$(document).on("click", "#short", function () {
    if ($(this).hasClass("bi-caret-up-fill")) {
        $(this).removeClass("bi-caret-up-fill");
        $(this).addClass("bi-caret-down-fill");
    }
    else {
        $(this).removeClass("bi-caret-down-fill");
        $(this).addClass("bi-caret-up-fill");
    }
    $('tbody').each(function () {
        var list = $(this).children('tr');
        $(this).html(list.get().reverse());
    });
})

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


/// chat

function OpenChat(requestId, isPatient){
    $.ajax({
        url: "/Patient/OpenChat",
        type: "Get",
        contentType: "application/json",
        data: {
            requestId: requestId,
            type: isPatient ? 1 : 3,
        },
        success: function (response) {
            $("#chatDiv").html(response);
        }
    })
}

$(document).on("click", ".chat", function () {
    OpenChat($(this).attr("id"), true);
})

$(document).on("click", ".chatPhysician", function () {
    OpenChat($(this).attr("id"), false);
})
