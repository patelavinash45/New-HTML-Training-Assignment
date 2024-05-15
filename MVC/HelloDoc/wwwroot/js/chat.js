"use strict";
$("#ChatCanvas").offcanvas('show');
var user = localStorage.getItem("user");
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        transport: signalR.HttpTransportType.WebSockets,
        query: {
            userId: user,
            username: randomString(),
        }
    }).build();

// var connection = new signalR.HubConnectionBuilder().withUrl(`/chatHub?UserName=${Math.random().toString(10)}&Id=${user}`).build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var mess = `<div class="text-end mb-4"><spam class="border-2 border sendMessage p-2 border-dark">${message}</spam></div>`;
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

function randomString() {  
    var characters = "ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";  
    var lenString = 10;  
    var randomstring = '';  
    for (var i=0; i<lenString; i++) {  
    var rnum = Math.floor(Math.random() * characters.length);  
    randomstring += characters.substring(rnum, rnum+1);  
    }  
    return randomString; 
}  