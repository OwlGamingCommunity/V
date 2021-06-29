$(function () {
    $(document).on("change", ":file", function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input
                .val()
                .replace(/\\/g, "/")
                .replace(/.*\//, "");
        input.trigger("fileselect", [numFiles, label]);
    });

    // We can watch for our custom `fileselect` event like this
    $(document).ready(function () {
        $(":file").on("fileselect", function (event, numFiles, label) {
            var input = $(this)
                .parents(".input-group")
                .find(":text"),
                log = numFiles > 1 ? numFiles + " files selected" : label;

            if (input.length) {
                input.val(log);
            }
        });
    });
});

function getBase64(file, onLoadCallback) {
    var reader = new FileReader();
    reader.onload = onLoadCallback;
    reader.readAsDataURL(file);
}

function Reset() {
    $("#fileNameField").val('');
    $("#exitX").val('');
    $("#exitY").val('');
    $("#exitZ").val('');

    $("#propertyID").text('Unknown');
    $("#propertyName").text('Unknown');
}

function Close() {
    TriggerEvent("CustomInterior_CloseWindow");
}

function validateInput() {
    var positionX = parseFloat($("#exitX").val());
    var positionY = parseFloat($("#exitY").val());
    var positionZ = parseFloat($("#exitZ").val());

    if (isNaN(positionX)) {
        $("#errorText").text('The position entered on value X is not a valid position.');
    } else if (isNaN(positionY)) {
        $("#errorText").text('The position entered on value Y is not a valid position.');
    } else if (isNaN(positionZ)) {
        $("#errorText").text('The position entered on value Z is not a valid position.');
    }

    if (!isNaN(positionX) && !isNaN(positionY) && !isNaN(positionZ)) {
        var files = document.getElementById('mapFile').files;
        if (files.length > 0) {
            var fileName = files[0].name;
            var fileExtension = fileName.substring(fileName.lastIndexOf('.') + 1);

            if (fileExtension == "ymap" || fileExtension == "xml") {
                $("#errorText").text('');
                getBase64(files[0], function (e) {
                    var base64string = e.target.result.replace('data:', '').replace(/^.+,/, '');
                    TriggerEvent("CustomInterior_ProcessCustomInterior", base64string, fileExtension, positionX, positionY, positionZ);
                });
            }
        }
    }
}

function SetUIData(propertyID, propertyName) {
    // Clear all fields first
    $("#mappingFileTxt").val('');
    $("#exitX").val('');
    $("#exitY").val('');
    $("#exitZ").val('');

    // Then load data from the server
    $("#propertyID").text(propertyID);
    $("#propertyName").text(propertyName);
}


// DRAGGING LOGIC
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
        if (e.target.nodeName == "TEXTAREA" || e.target.nodeName == "BUTTON" || e.target.nodeName == "INPUT") {
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