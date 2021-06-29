var selectedText = "Select action";
var wheel = null;
var PLAYER_IMAGES = [
    "handcuff.png",
    "frisk.png"
];

var VEHICLE_IMAGES = [
    "trunk.png",
    "hood.png",
    "vehlock.png"
];

function transformImagePaths(images) {
    var basepath = IsRunningInRAGECEF() ? "package://owl_entityinteractions.client/" : "./icons/";
    return images.map(filename => "imgsrc:" + basepath + filename);
}

function Show(playerMenu, vehicleMenu, pedMenu) {
    setupRadialMenu();
    if (playerMenu) {
        wheel.createWheel(transformImagePaths(PLAYER_IMAGES));

        wheel.navItems[0].navItem.mouseover(function () { setItemSelection("Handcuff", 0); });
        wheel.navItems[0].navItem.mouseout(function () { resetItemSelection(0); });

        wheel.navItems[1].navItem.mouseover(function () { setItemSelection("Frisk", 1); });
        wheel.navItems[1].navItem.mouseout(function () { resetItemSelection(1); });

    } else if (vehicleMenu) {
        wheel.createWheel(transformImagePaths(VEHICLE_IMAGES));

        wheel.navItems[0].navItem.mouseover(function () { setItemSelection("Vehicle Trunk", 2); });
        wheel.navItems[0].navItem.mouseout(function () { resetItemSelection(2); });

        wheel.navItems[1].navItem.mouseover(function () { setItemSelection("Vehicle Hood", 3); });
        wheel.navItems[1].navItem.mouseout(function () { resetItemSelection(3); });

        wheel.navItems[2].navItem.mouseover(function () { setItemSelection("Vehicle Lock", 4); });
        wheel.navItems[2].navItem.mouseout(function () { resetItemSelection(4); });

    } else if (pedMenu) {
        // TODO: Remove when Ped menu is going to get used to activate wheel.
        //wheel.createWheel(['Placeholder', 'Placeholder', 'Placeholder']);
        //wheel.navItems[0].navSlice.mouseover(function () { setItemSelection("Placeholder", 5); });
        //wheel.navItems[0].navSlice.mouseout(function () { resetItemSelection(5); });
    }
}

function reloadRadialMenu() {
    wheel.spreader.inTitle.title = selectedText;
    wheel.spreader.outTitle.title = selectedText;
    wheel.refreshWheel();
}

function resetItemSelection(index) {
    selectedText = "Select Action";
    wheel.refreshWheel();
    TriggerEvent("PlayerRadial_OnExitItem", index);
}

function setItemSelection(selection, index) {
    selectedText = selection;
    reloadRadialMenu();
    TriggerEvent("PlayerRadial_OnEnterItem", index);
}

function setupRadialMenu() {
    var spinnerSize = 800;
    document.getElementById("wheelDiv").style.width = spinnerSize + "px";
    document.getElementById("wheelDiv").style.height = spinnerSize + "px";

    // Set top margin to center wheel
    var marginTopSize = ($(window).height() - spinnerSize) / 2;
    document.getElementById("container").style.marginTop = marginTopSize + "px";

    /**
     * @see https://github.com/softwaretailoring/wheelnav
     * @type {wheelnav}
     */
    wheel = new wheelnav("wheelDiv");
    wheel.navAngle = 90;
    wheel.clickModeRotate = true;
    wheel.slicePathFunction = slicePath().DonutSlice;
    wheel.slicePathCustom = slicePath().DonutSliceCustomization();
    wheel.slicePathCustom.minRadiusPercent = 0.3;
    wheel.slicePathCustom.maxRadiusPercent = 0.6;
    wheel.sliceInitPathCustom = wheel.slicePathCustom;
    wheel.sliceHoverPathCustom = wheel.slicePathCustom;
    wheel.sliceSelectedPathCustom = wheel.slicePathCustom;
    wheel.slicePathAttr = {fill: '#000000', stroke: '#000000', 'stroke-width': 2, opacity: 0.50};
    wheel.sliceHoverAttr = { fill: '#ff8000', stroke: '#000000', 'stroke- width': 2, opacity: 0.80};
    wheel.sliceSelectedAttr = { fill: '#ff8000', stroke: '#000000', 'stroke- width': 2, opacity: 0.80};
    wheel.titleWidth = 64;
    wheel.titleHeight = 64;
    wheel.titleCurved = true;
    wheel.titleCurvedClockwise = false;
    wheel.titleCurvedByRotateAngle = false;
    wheel.titleAttr = { fill: '#ffffff', stroke: 'none', font: 'Helvetica', 'font-size': 16, 'font-weight': '200'};
    wheel.titleHoverAttr = { fill: '#ffffff', stroke: 'none', font: 'Helvetica', 'font- size': 16, 'font-weight': '200'};
    wheel.titleSelectedAttr = { fill: '#ffffff', stroke: 'none', font: 'Helvetica', 'font- size': 16, 'font-weight': '200' };
    wheel.spreaderEnable = true;
    wheel.spreaderRadius = 125;
    wheel.spreaderPathInAttr = { fill: '#000000', stroke: '#000000', 'stroke-width': 2, opacity: 0.50 };
    wheel.spreaderPathOutAttr = { fill: '#000000', stroke: '#000000', 'stroke-width': 2, opacity: 0.50 };
    wheel.spreaderTitleInAttr = { fill: '#fff' };
    wheel.spreaderTitleOutAttr = { fill: '#fff' };
    wheel.spreaderInTitle = selectedText;
    wheel.spreaderOutTitle = selectedText;
    wheel.spreaderTitleFont = '400 24px Helvetica';
    wheel.selectedNavItemIndex = null;
}
function Hide() {
    wheel.removeWheel();
    selectedText = "Select action";
}

function OnLoad() {
    if (!IsRunningInRAGECEF()) {
        Show(null, 1, null);
    }
}