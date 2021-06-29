var toMessageNumber = null
var toMessageName = null
var messages = [];
var messengerContacts = null

/* Begin of messages screen */
function createSMSScreen() {
    hidePhoneScreens();

    $("#phoneUI").load("package://owl_chat.client/messagescreen.html");

    TriggerEvent("GetPhoneMessagesContacts");
}

$(document).on("click", "#openMessages #messengerChat", function (event) {
    createChatScreen($(this).attr('phoneNumber'), event.target.innerText);
});

$(document).on("click", "#addMessagesBtn", function () {
    createPhoneMessage();
});
/* End of messages screen */

/* Begin of chat screen */
function createChatScreen(phoneNumber, phoneName) {
    hidePhoneScreens();

    TriggerEvent("GetPhoneMessagesFromNumber", phoneNumber);
    $("#phoneUI").load("package://owl_chat.client/chatscreen.html", function (_,
        status) {
        if (status == "success") {
            responsiveChat('.message-chat', phoneNumber, phoneName);
            $('#contactName').text(phoneName);
        }
    });
}

$(document).on("click", "#messagesBackBtn", function () {
    createSMSScreen();
});
/* End of chat screen */

/* Begin of chat */
function responsiveChat(element, toNumber, toName) {
    toMessageNumber = toNumber;
    toMessageName = toName;
    $(element).html('<form class="chat"><span></span><div class="messages"></div><input type="text" maxlength="200" placeholder="Your message"><input type="submit" value="Send"></form>');

    function showLatestMessage() {
        $(element).find('.messages').scrollTop($(element).find('.messages').prop('scrollHeight'));
        TriggerEvent("UpdateMessageViewed", toMessageNumber);
    }

    $(element + ' input[type="text"]').keypress(function (event) {
        if (event.which == 13) {
            event.preventDefault();
            $(element + ' input[type="submit"]').click();
        }
    });
    $(element + ' input[type="submit"]').click(function (event) {
        event.preventDefault();
        var message = $(element + ' input[type="text"]').val();
        if ($(element + ' input[type="text"]').val()) {
            var d = new Date();
            var clock = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
            var month = d.getMonth() + 1;
            var day = d.getDate();
            var currentDate =
                (("" + day).length < 2 ? "0" : "") +
                day +
                "." +
                (("" + month).length < 2 ? "0" : "") +
                month +
                "." +
                d.getFullYear() +
                "&nbsp;&nbsp;" +
                clock;
            $(element + ' div.messages').append(
                '<div class="message"><div class="myMessage"><p>' +
                message +
                "</p><date>" +
                currentDate +
                "</date></div></div>"
            );
            setTimeout(function () {
                $(element + ' > span').addClass("spinner");
            }, 100);
            setTimeout(function () {
                $(element + ' > span').removeClass("spinner");
            }, 2000);
        }

        if (message.length > 0) {
            TriggerEvent("CreatePhoneMessage", toMessageNumber, message);
        }

        $(element + ' input[type="text"]').val("");
        showLatestMessage();
    });
    $(element).find('.messages').empty();
}
/* End of chat */

/* Begin of add chat message to chat screen */
function responsiveChatPush(element, origin, date, message) {
    var originClass;
    if (origin == 'me') {
        originClass = 'myMessage';
    } else {
        originClass = 'fromThem';
    }
    $(element + ' .messages').append('<div class="message"><div class="' + originClass + '"><p>' + message + '</p><date><b>' + date + '</date></div></div>');

	$(element).find('.messages').scrollTop($(element).find('.messages').prop('scrollHeight'));
}
/* End of add chat message to chat screen */

/* Begin of loading all the contacts that sent a message to the phone */
function loadPhoneMessagesContacts(messagesContactList) {
    messengerContacts = JSON.parse(messagesContactList);
    var items = '';
    if (Object.keys(messengerContacts).length >= 1) {
        $.each(messengerContacts, function (key, value) {
            if (value.viewed > 0) {
                items += '<li class="list-group-item notification-badge" data-badge="' + value.viewed + '" id="messengerChat" phoneNumber="' + value.number + '"><div class="avatar"><img src="https://i.imgur.com/iuDcGlE.jpg"></div><div id="messagePhoneNumber">' + value.entryName + '</div></li>';
            } else {
                items += '<li class="list-group-item" id="messengerChat" phoneNumber="' + value.number + '"><div class="avatar"><img src="https://i.imgur.com/iuDcGlE.jpg"></div><div id="messagePhoneNumber">' + value.entryName + '</div></li>';
            }
        });
    } else {
        items += '<li>No messages found.</li>';
    }


    $('#openMessages').empty();
    $('#openMessages').append(items);
}
/* End of loading all the contacts that sent a message to the phone */

/* Begin of loading all the messages that the user selected he wanted to see by phone number */
function loadPhoneMessagesFromNumber(messagesList) {
    messages = JSON.parse(messagesList);
    if (Object.keys(messages).length >= 1) {
        $.each(messages, function (_, message) {
            if (message.to == toMessageNumber) {
                responsiveChatPush('.chat', 'me', message.date, message.content);
            } else {
                responsiveChatPush('.chat', 'you', message.date, message.content);
            }
        });
        TriggerEvent("UpdateMessageViewed", toMessageNumber);
    }
}
/* End of loading all the messages that the user selected he wanted to see by phone number */

/* Begin of open messenger */
function createPhoneMessage() {
    try {
        var $alert = document.querySelector('.alert');
        $alert.parentElement.removeChild($alert);
    } catch ($error) { }

    var $alert = document.createElement('span');
    $alert.innerHTML = `
    <div class="alert">
        <div class="inner">
            <div class="title">Send new message</div>
            <div class="text">
                <input type="text" id="phoneNumber" class="form-control" placeholder="Phone number">
            </div>
        </div>
        <div class="button" id="openChatBtn">Start chat</div>
        <div class="button" id="cancelChatBtn">Cancel</div>
    </div>`;
    document.getElementById("phoneScreen").appendChild($alert);
    setTimeout(function () {
        document.querySelector('.alert #openChatBtn').addEventListener("click", function () {
            var phoneNumber = $('#phoneNumber').val();
            if (/^\d+$/.test(phoneNumber) && phoneNumber != null) {
                createChatScreen(phoneNumber, phoneNumber);
                $alert.parentElement.removeChild($alert);
            } else {
                $alert.parentElement.removeChild($alert);
            }
        });
        document.querySelector('.alert #cancelChatBtn').addEventListener("click", function () {

            $alert.parentElement.removeChild($alert);
        });
    });
    return false;
}
/* End of open messenger */