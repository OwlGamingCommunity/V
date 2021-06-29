var cachedDetails;

window.onload = function (e) {
    dragElement(document.getElementById("box-middle"));
}

function dragElement(elmnt) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "header")) {
        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
    }
    else {
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        if (e.target.nodeName == "TEXTAREA" || e.target.nodeName == "BUTTON") {
            return;
        }

        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }
}

function closeDragElement() {
    // stop moving when mouse button is released:
    document.onmouseup = null;
    document.onmousemove = null;
}

function AddNote() {
    var note = document.getElementById('adminnotes').value;
    TriggerEvent("SaveVehicleNote", note);
    TriggerEvent("ReloadCheckVehData");
    $('#adminnotes').val('').blur();
    $(".nav-tabs li a").removeClass("active");
    $('#actions').addClass('active');
    $("#noteHeaders").addClass("hiddenHeader");
    $('#actionHeaders').removeClass('hiddenHeader');
}

function Close() {
    TriggerEvent("CloseCheckVeh");
}

function SetAllData(vehicleDetails) {
    cachedDetails = JSON.parse(vehicleDetails);

    $('#username').text(cachedDetails.Username);
    $('#characterName').text(cachedDetails.CharacterName);
    $('#storePrice').text("$" + cachedDetails.StorePrice);
    $('#vehicleModel').text(cachedDetails.Model);
    $('#paymentMethod').text(cachedDetails.PaymentMethod);
    $('#paymentMade').text(cachedDetails.PaymentsMade);
    $('#paymentRemaining').text(cachedDetails.PaymentsRemaining);
    $('#paymentMissed').text(cachedDetails.PaymentsMissed);
    $('#creditAmount').text(cachedDetails.CreditAmount);
    $('#faction').text(cachedDetails.Faction);
    $('#stolen').prop('checked', cachedDetails.Stolen);

    LoadActionsData();
    $(".nav-tabs li a").removeClass("active");
    $('#actions').addClass('active');
    $("#noteHeaders").addClass("hiddenHeader");
    $('#actionHeaders').removeClass('hiddenHeader');
}

function LoadActionsData() {
    var Actions = '';
    if (Object.keys(cachedDetails.Actions).length >= 1) {
        cachedDetails.Actions.reverse();
        $.each(cachedDetails.Actions, function (key, value) {
            Actions += '<tr><td>' + value.action + '</td><td>' + value.actor + '</td><td>' + value.date + '</td></tr>';
        });
    } else {
        Actions += '<p>No results found.</p>';
    }

    $('#tableBody').empty();
    $('#tableBody').append(Actions);
}

function LoadNotesData() {
    var Notes = '';
    if (Object.keys(cachedDetails.Notes).length >= 1) {
        cachedDetails.Notes.reverse();
        $.each(cachedDetails.Notes, function (key, value) {
            Notes += '<tr><td>' + value.note + '</td><td>' + value.creator + '</td><td>' + value.date + '</td></tr>';
        });
    } else {
        Notes += '<p>No results found.</p>';
    }

    $('#tableBody').empty();
    $('#tableBody').append(Notes);
}

$('.nav-tabs li').on('click', 'a', function (e) {
    if (this.text == "Admin Notes") {
        LoadNotesData();
        $(".nav-tabs li a").removeClass("active");
        $('#adminNotes').addClass('active');
        $("#noteHeaders").removeClass("hiddenHeader");
        $('#actionHeaders').addClass('hiddenHeader');
    } else if (this.text == "Actions") {
        LoadActionsData();
        $(".nav-tabs li a").removeClass("active");
        $('#actions').addClass('active');
        $("#noteHeaders").addClass("hiddenHeader");
        $('#actionHeaders').removeClass('hiddenHeader');
    }
});

function UpdateStolenState(cb) {
    TriggerEvent("UpdateStolenState", cb.checked);
}