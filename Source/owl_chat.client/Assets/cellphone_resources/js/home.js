/* Begin of phone time and date */
function updateTimes(date) {
    var months = [
        'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'
    ];

    var days = [
        'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
    ];

    var date = new Date(date);

    var dayOfWeek = days[date.getDay()];
    var month = months[date.getMonth()];
    var dater = date.getDate();

    var dateString = dayOfWeek + ', ' + month + ' ' + dater;

    $('.date').text(dateString);
    $('.day-n').text(dater);
    $('.day').text(dayOfWeek);
}
/* End of phone time and date */

/* Begin of home screen with apps */
function createHomeScreen() {
    setHomeBtnStatus();
    hidePhoneScreens();

    $("#phoneUI").load("package://owl_chat.client/homescreen.html", function (_,
        status) {
        if (status == "success") {
            $("#background").removeClass();
            $("#background").addClass("wallpaper");

            $("#home-btn").removeClass();
            $("#home-btn").addClass("unlocked");
        }
    });
    
}

$(document).on("click", "#app-phone", function () {
    setHomeBtnStatus(true);
    createCallScreen();
});

$(document).on("click", "#app-taxi", function () {
    setHomeBtnStatus(true);
    createTaxiScreen();
});

$(document).on("click", "#app-contacts", function () {
    setHomeBtnStatus(true);
    createContactsScreen();
});

$(document).on("click", "#app-banking", function () {
    TriggerEvent("OpenMobileBankingUI");
    hidePhoneScreens();
    TriggerEvent("EndCall");
    TriggerEvent("ClosePhone");
});

$(document).on("click", "#app-messages", function () {
    setHomeBtnStatus(true);
    createSMSScreen();
});

$(document).on("click", "#app-safari", function () {
    createPhoneAlert("App unavailable", "This app is currently in maintenance.");
});

$(document).on("click", "#app-settings", function () {
    createPhoneAlert("App unavailable", "This app is currently in maintenance.");
});
/* End of home screen with apps */