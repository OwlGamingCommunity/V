var isPlayerCalling = false

/* Begin of call screen */
function createCallScreen() {
    hidePhoneScreens();

    $("#background").removeClass();
    $("#background").addClass("callBg");

    $("#phoneUI").load("package://owl_chat.client/dialpadscreen.html");
}

$(document).on("click", ".key", function () {
    let clickedNumber = $(this).text().charAt(0);
    if ($(this).attr('id') == "removeEntry") {
        $('#numberInput').val(
            function (index, value) {
                return value.substr(0, value.length - 1);
            }
        )
    } else {
        $('#numberInput').val($('#numberInput').val() + clickedNumber);
    }
});

$(document).on("click", "#callBtn", function () {
    let numberToDial = $('#numberInput').val();
    if (numberToDial != '' || numberToDial.length != 0) {
        createDialScreen(numberToDial, null, false);
    } else {
        createPhoneAlert("Invalid format", "You have to enter a phone number in order to call someone.");
    }
});
/* End of call screen */

/* Begin of dial screen */
function createDialScreen(phoneNumber, contactName, isCalled) {
    hidePhoneScreens();

        $("#phoneUI").load("package://owl_chat.client/callingscreen.html", function (_,
            status) {
            if (status == "success") {
                $("#background").removeClass();
                $("#background").addClass("dialBg");
                if (!isCalled) {
                    TriggerEvent("CallNumber", phoneNumber);
                }

                if (contactName == "" || contactName == null) {
                    $('#callingNumber').val(phoneNumber);
                } else {
                    $('#callingNumber').val(contactName);
                }
            }
        });
}

function cancelPhoneCall() {
    createHomeScreen();
}

$(document).on("click", "#cancelDialBtn", function () {
    if (isPlayerCalling) {
        TriggerEvent("EndCall");
        isPlayerCalling = false
    } else {
        TriggerEvent("CancelCall");
        isPlayerCalling = false
    }
    cancelPhoneCall();
});
/* End of dial screen */

function createPickupScreen(phoneNumber) {
    hidePhoneScreens();

    TriggerEvent("GetPhoneContactByNumber", phoneNumber);


    $("#phoneUI").load("package://owl_chat.client/pickupscreen.html", function (_,
        status) {
        if (status == "success") {
            $("#background").removeClass();
            $("#background").addClass("dialBg");

            $('#calledByNumber').attr("callingNumber", phoneNumber);
            $('#calledByNumber').val(phoneNumber);
        }
    });
}

$(document).on("click", "#acceptCallBtn", function () {
    TriggerEvent("AnswerCall");
    createDialScreen($('#calledByNumber').attr("callingNumber"), $('#calledByNumber').val(), true);
});

$(document).on("click", "#declineCallBtn", function () {
    TriggerEvent("CancelCall");
    cancelPhoneCall();
    isPlayerCalled = false;
    isPlayerCalling = false;
});

function OnCallReceived(number, is_connected) {
    if (is_connected) {
        isPlayerCalling = true
        $('#callingStatus').html("Connected!");
    }
}

function UpdateCallStatus(reason) {
    $('#callingStatus').html(reason);
        setTimeout(function () {
            createHomeScreen();
        }, 3000);
        isPlayerCalled = false;
        isPlayerCalling = false;
}

function loadContactNameByNumber(contactName) {
    if (contactName.length > 0) {
        $('#calledByNumber').val(contactName);
    } 
}