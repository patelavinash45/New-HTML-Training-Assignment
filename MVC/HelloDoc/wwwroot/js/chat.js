"use strict";
$("#ChatCanvas").offcanvas('show');
$("body").css("height","200px");

var connection = new signalR.HubConnectionBuilder().withUrl(`/chatHub`).build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message, time, aspNetUserId) {
    var user = localStorage.getItem("user");
    var isSend = aspNetUserId == user;
    if(isSend){
        var mess = `<div class="mb-2 ms-auto text-break border-2 border sendMessage p-2 border-dark"><span>${message}</span><span class="time">${time}</span></div>`;
    }
    else{
        var mess = `<div class="mb-2 me-auto text-break border-2 border reciveMessage p-2 border-dark"><span>${message}</span><span class="time">${time}</span></div>`;
    }
    $(".chatDisplay").append(mess);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = $("#messageInput").val();
    if(message.length > 0){
        connection.invoke("SendMessage", message).catch(function (err) {    
            return console.error(err.toString());
        });
    }
    $("#messageInput").val("");
    event.preventDefault();
}); 
    
$(document).on("click", "#closeChat", function(){
    connection.stop();
})
