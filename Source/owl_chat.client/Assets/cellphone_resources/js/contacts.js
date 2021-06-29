var contacts = null;

/* Begin of contacts screen */
function createContactsScreen() {
    hidePhoneScreens();

    TriggerEvent("GetPhoneContacts");

    $("#phoneUI").load("package://owl_chat.client/contactsscreen.html", function (_,
        status) {
        if (status == "success") {
            $("#contactsList").ioslist();
        }
    });
}

$(document).on("click", "#contactsBackBtn", function () {
    $("#home-btn").removeClass();
    $("#home-btn").addClass("unlocked");

    createHomeScreen();
});

$(document).on("click", "#addContactBtn", function () {
    createPhoneContact();
});

function callContact(phoneNumber, contactName) {
    createDialScreen(phoneNumber, contactName, false);
}

$(document).on("click", "#addedContacts li.contact", function (event) {
    if (event.target.id == "removeContactBtn") {
        var phoneNumber = event.target.parentElement.attributes.phonenumber.value;
        var contactName = event.target.parentElement.attributes.contactName.value;
        TriggerEvent("RemovePhoneContact", phoneNumber, contactName);
        TriggerEvent("GetPhoneContacts");
    } else {
        var phoneNumber = event.target.attributes.phonenumber.value;
        var contactName = event.target.attributes.contactName.value;
        createContactOptions(phoneNumber, contactName);
    }
});
/* End of contacts screen */

/* Begin of add phone contact */
function createPhoneContact() {
    try {
        var $alert = document.querySelector('.alert');
        $alert.parentElement.removeChild($alert);
    } catch ($error) { }

    var $alert = document.createElement('span');
    $alert.innerHTML = `
    <div class="alert">
        <div class="inner">
            <div class="title">New Contact</div>
            <div class="text">
                <input type="text" maxlength="30" id="entryName" class="form-control" placeholder="Name" style="margin-bottom: 1em;">
                <input type="text" id="entryNumber" class="form-control" placeholder="Phone number">
            </div>
        </div>
        <div class="button" id="saveContactBtn">Save</div>
        <div class="button" id="cancelContactBtn">Cancel</div>
    </div>`;
    document.getElementById("phoneScreen").appendChild($alert);
    setTimeout(function () {
        document.querySelector('.alert #saveContactBtn').addEventListener("click", function () {
            var entryName = $('#entryName').val();
            var entryNumber = $('#entryNumber').val();
            if (/^\d+$/.test(entryNumber)) {
                TriggerEvent("SavePhoneContact", entryNumber, entryName);
                TriggerEvent("GetPhoneContacts");
            }
            $alert.parentElement.removeChild($alert);
        });
        document.querySelector('.alert #cancelContactBtn').addEventListener("click", function () {

            $alert.parentElement.removeChild($alert);
        });
    });
    return false;
}
/* End of add phone contact */

function loadPhoneContacts(contactsList) {
    contacts = JSON.parse(contactsList);
    var items = '';
    if (Object.keys(contacts).length >= 1) {
        contacts.forEach(contact => 
        {
            contactName = contact.Key;
            contactNumber = contact.Value;
            items += '<li class="contact" phoneNumber="' + contactNumber + '" contactName="' + contactName + '">' + contactName + '<i class="fas fa-trash remove-contact-icon" id="removeContactBtn"></i></li>';
        });
    } else {
        items += '<li>No contacts found.</li>';
    }


    $('#addedContacts').empty();
    $('#addedContacts').append(items);

    $('#addedContacts li.contact').hover(
        function () {
            var phoneNumber = $(this).attr("phoneNumber")
            $($(this)).contents().filter(function () {
                return this.nodeType == 3;
            })[0].nodeValue = phoneNumber;
        },
        function () {
            var contactName = $(this).attr("contactName");
            $($(this)).contents().filter(function () {
                return this.nodeType == 3;
            })[0].nodeValue = contactName;
        }
    );
}

/* Begin of contact options */
function createContactOptions(phoneNumber, contactName) {
    try {
        var $alert = document.querySelector('.alert');
        $alert.parentElement.removeChild($alert);
    } catch ($error) { }

    var $alert = document.createElement('span');
    $alert.innerHTML = `
    <div class="alert">
        <div class="inner">
            <div class="title">Contact options</div>
            <div class="text">What would you like to do?</div>
        </div>
        <div class="button" id="callContactBtn">Call</div>
        <div class="button" id="textContactBtn">SMS</div>
        <div class="button" id="cancelContactBtn">Cancel</div>
    </div>`;
    document.getElementById("phoneScreen").appendChild($alert);
    setTimeout(function () {
        document.querySelector('.alert #callContactBtn').addEventListener("click", function () {
            callContact(phoneNumber, contactName);
            $alert.parentElement.removeChild($alert);
        });
        document.querySelector('.alert #textContactBtn').addEventListener("click", function () {
            createChatScreen(phoneNumber, contactName);
            $alert.parentElement.removeChild($alert);
        });
        document.querySelector('.alert #cancelContactBtn').addEventListener("click", function () {
            $alert.parentElement.removeChild($alert);
        });
    });
    return false;
}
/* End of contact options */