
function sidebar() {  /// for mobile view sidebar
    var temp = $("#side-bar").css("display");
    if (temp == "none") {
        $("#side-bar").css("display","block");
    } else {
        $("#side-bar").css("display", "none");
    }
}

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
})

function setTab(temp){
    localStorage.setItem("tab", `tab${temp}`);
}
