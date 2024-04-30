var requesterTextColor = [ "", "text-danger", "text-success", "text-warning", "text-primary", "text-info" ];
var requester = ["", "Business", "Patient", "Family", "Concierge", "VIP"];

function changeSelect(document) {
    $.ajax({
        url: "/Admin/GetPhysicians",
        type: "Get",
        contentType: "application/json",
        data: {
            regionId: $(document).val(),
        },
        success: function (response) {
            $(".physician").html('<option disabled selected value="">Physicians</option>');
            $.each(response, function (index, item) {
                var option = `<option value="${index}">${item}</option>`;
                $(".physician").append(option);
            });
        }
    });
}

function displayPopUp(id) {
    $(".patientRequestId").val(id);
    $(".patientName").text($(`#name-${id}`).text());
    $('.validation').css("display", "none");
}

function displaySendAgreementPopUp(id) {
    $(".patientRequestId").val(id);
    $.ajax({
        url: "/Admin/GetEmailAndMobileNumber",
        type: "Get",
        async: false,
        contentType: "application/json",
        data: {
            requestId: id,
        },
        success: function (response) {
            $("#patientEmail").val(response.item1);
            $("#patientNumber").val(response.item2);
            $("#requesterIcon").addClass(requesterTextColor[response.item3]);
            $("#requesterName").html(requester[response.item3]);
        }
    })
}

function clearCase() {
    $.ajax({
        url: "/Admin/ClearPopUp",
        type: "Get",
        async: false,
        contentType: "application/json",
        data: {
            requestId: $(".patientRequestId").val(),
        },
        success: function (response) {
            window.location.href = response.redirect;
        }
    })
}

$("#transferForm").submit(function (e) {
    if ($("#transferRequestRegion").val() == null) {
        e.preventDefault();
        $("#transferValidationforRegions").css("display", "block");
    }
    else {
        $("#transferValidationforRegions").css("display", "none");
    }
    if ($("#transferRequestPhysician").val() == null) {
        e.preventDefault();
        $("#transferValidationPhysician").css("display", "block");
    }
    else {
        $("#transferValidationPhysician").css("display", "none");
    }
})


////

function navigatToViewCase(requestId) {
    navigation("ViewCase", requestId);
}

function navigatToViewNotes(requestId) {
    navigation("ViewNotes", requestId);
}

function navigatToViewDocuments(requestId) {
    navigation("ViewDocument", requestId);
}

function navigatToConcludeCare(requestId) {
    navigation("ConcludeCare", requestId);
}

function navigatToSendOrder(requestId) {
    navigation("SendOrder", requestId);
}

function navigatToEncounterForm(requestId, isFinaliz) {
    if (!isFinaliz) {
        navigation("EncounterForm", requestId);
    }
    else { 
        $(".patientRequestId").val(requestId);
        $("#downloadReport").modal('show');
    }
}

function navigatToCloseCase(requestId) {
    navigation("CloseCase", requestId);
}


function navigation(actionName, requestId) {
    $.ajax({
        url: "/Admin/SetRequestId",
        type: "Get",
        contentType: "application/json",
        data: {
            requestId: requestId,
            actionName: actionName,
        },
        success: function (response) {
            window.location.href = response.redirect;
        }
    })
}


//// provider popup - contact provider

var email = false, sms = false;

$(document).on("change", "#email", function () {
    if ($(this).is(":checked")) {
        email = true;
    }
    else {
        email = false;
    }
    isBothRadionbuttonslected();
})

$(document).on("change", "#sms", function () {
    if ($(this).is(":checked")) {
        sms = true;
    }
    else {
        sms = false;
    }
    isBothRadionbuttonslected();
})

function isBothRadionbuttonslected() {
    if (email && sms) {
        $("#both").prop("checked", true);
    }
    else {
        $("#both").prop("checked", false);
    }
}

$(document).on("change", "#both", function () {
    if ($(this).is(":checked")) {
        sms = true;
        email = true;
        $("#email").prop("checked", true);
        $("#sms").prop("checked", true);
    }
    else {
        sms = false;
        email = false;
        $("#email").prop("checked", false);
        $("#sms").prop("checked", false);
    }
})

$(document).on("submit", "#contactForm", function (e) {
    if (!email && !sms) {
        e.preventDefault();
        $("#medium").css("display", "block");
    }
    else {
        $("#medium").css("display", "none");
    }
    if ($("#message").val().length == 0) {
        e.preventDefault();
        $("#messageValidation").css("display", "block");
    }
    else {
        $("#messageValidation").css("display", "none");
    }
})


////   Request Support

function messageValidation() {
    if ($("#message").val().length == 0) {
        $("#messageValidation").text("Message is required");
    }
    else {
        $("#messageValidation").text("");
    }
}

$(document).on("change", "#message", function () {
    messageValidation();
})

$("#requestSupportForm").submit(function (e) {
    if ($("#message").val().length == 0) {
        e.preventDefault();
        messageValidation();
    }
})




///  physician Encounter

function checkEncounter(isEncounter, requestId, isFinaliz) {
    if (isEncounter == 1) {
        navigatToEncounterForm(requestId, isFinaliz);
    }
    else {
        $(".patientRequestId").val(requestId);
        $("#encounterPopUp").modal('show');
    }
}

var isvideoCall = true;
function careButtonChnage(temp) {
    if (temp) {
        $("#videoCall").addClass("activeButton");
        $("#houseVisit").removeClass("activeButton");
    }
    else {
        $("#houseVisit").addClass("activeButton");
        $("#videoCall").removeClass("activeButton");
    }
    isvideoCall = !isvideoCall;
}

function encounter() {
    $.ajax({
        url: "/Physician/SetEncounter",
        type: "Get",
        contentType: "application/json",
        data: {
            isVideoCall: isvideoCall,
            requestId: $("#requestId").val(),
        },
        success: function (response) {
            window.location.href = response.redirect;
        }
    })
}

//// download pdf pop-up

$(document).on("click", ".downloadPdf", function () {
    $("#downloadReport").modal('hide');
})

