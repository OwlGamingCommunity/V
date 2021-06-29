/* Begin of taxi screen */
function createTaxiScreen() {
    hidePhoneScreens();

    $("#background").removeClass();
    $("#background").addClass("taxiBg");

    $("#phoneUI").load("package://owl_chat.client/taxiscreen.html", function (_,
        status) {
        if (status == "success") {
            if (g_TaxiState) {
                $("#requestTaxiBtn").html("Cancel pickup");
            }
        }
    });
}

$(document).on("click", "#requestTaxiBtn", function () {
    if (!g_TaxiState) {
        TriggerEvent("CallTaxi");
        createHomeScreen();
        createPhoneNotification("YC Taxi", "Order received!", "Your order was received, the driver is on it's way!");
        g_taxiState = true;
    } else {
        TriggerEvent("CancelTaxi");
        createHomeScreen();
        createPhoneNotification("YC Taxi", "Order canceled", "Your order was canceled, hit us up anytime if you need a ride!");
        g_TaxiState = false;
    }
});
/* End of taxi screen */