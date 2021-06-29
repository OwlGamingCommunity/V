var g_TaxiState = 0;
var isPlayerCalled = false;
/* Clear the phone UI screen (Only where content is displayed on.) */
function hidePhoneScreens() {
    $("#phoneUI").empty();
    isPlayerCalled = false;
}

/* Begin of home button */
function setHomeBtnStatus(inApp) {
    if ($("#home-btn").hasClass("in-app")) {
        $("#home-btn").removeClass();
        $("#background").removeClass();

        $("#home-btn").addClass("unlocked");
        $("#background").addClass("wallpaper");

        createHomeScreen();
        TriggerEvent("GetTotalUnviewedMessages");
    }

    if (inApp) {
        $("#home-btn").removeClass();
        $("#home-btn").addClass("in-app");
    }
}

$("#home-btn").click(function () {
    setHomeBtnStatus();
});
/* End of home button */

/* Begin of turn off/on button */
$("#iphone-turn").click(function () {
    try {
        var $alert = document.querySelector('.alert');
        $alert.parentElement.removeChild($alert);
    } catch ($error) { }

    hidePhoneScreens();
    TriggerEvent("EndCall");
    TriggerEvent("ClosePhone");
});
/* End of turn off/on button */

/* Begin of phone alerts */
function createPhoneAlert(title, description) {
    try {
        var $alert = document.querySelector('.alert');
        $alert.parentElement.removeChild($alert);
    } catch ($error) { }

    if (title != null & description != null) {
        var $alert = document.createElement('span');
        $alert.innerHTML = '<div class="alert"><div class="inner"><div class="title">' + title + '</div><div class="text">' + description + '</div></div><div class="button">OK</div></div>';
        document.getElementById("phoneScreen").appendChild($alert);
        setTimeout(function () {
            document.querySelector('.alert .button:last-child').addEventListener("click", function () {

                $alert.parentElement.removeChild($alert);
            });
        });
        return false;
    }
}
/* End of phone alerts */

/* Begin of notifications */
function createPhoneNotification(appName, notificationTitle, notificationMsg) {
    try {
        var $notification = document.querySelector('.notification');
        $notification.parentElement.removeChild($notification);
    } catch ($error) { }

    if (appName != null && notificationTitle != null && notificationMsg != null) {
        var $notification = document.createElement('span');

        $notification.innerHTML = `    
        <div class="box notification">
            <div class="app-title">
                <img src="package://owl_chat.client/notification-icon.png" class="notification-img" alt="">
                ` + appName + `
            </div>
            <div class="person">` + notificationTitle + `</div>
            <div class="msg">` + notificationMsg + `</div>
        </div>
        `;

        document.getElementById("phoneScreen").appendChild($notification);
        setTimeout(function () {
            $notification.parentElement.removeChild($notification);
        }, 2000);
        return false;
    }
}
/* End of notifications */

function Initialize(taxi_state, isCalled, callingNumber) {
    g_TaxiState = taxi_state;
    isPlayerCalled = isCalled;

    if (isCalled) {
        setHomeBtnStatus(true);
        createPickupScreen(callingNumber);
    } else {
        createHomeScreen();
    }
}

function SetTime(hour, minute, date) {
    hour_val = hour;
    minute_val = minute;

    // add trailing zeros
    if (hour < 10) {
        hour_val = "0" + hour_val;
    }

    if (minute < 10) {
        minute_val = "0" + minute_val;
    }

    $('.hour').text(hour_val);
    $('.minute').text(minute_val);

    updateTimes(date);
}

function displayUnreadMessages(unreadMessages) {
    if (unreadMessages > 0) {
        $("#app-messages").addClass("notification-badge");
        $('#app-messages').attr('data-badge', unreadMessages);
    }
}