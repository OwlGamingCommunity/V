/// <reference path="../typings/PlayFab/PlayFabClientApi.d.ts" />

var PlayFab = typeof PlayFab != "undefined" ? PlayFab : {};

if(!PlayFab.settings) {
    PlayFab.settings = {
        titleId: null, // You must set this value for PlayFabSdk to work properly (Found in the Game Manager for your title, at the PlayFab Website)
        developerSecretKey: null, // For security reasons you must never expose this value to the client or players - You must set this value for Server-APIs to work properly (Found in the Game Manager for your title, at the PlayFab Website)
        advertisingIdType: null,
        advertisingIdValue: null,
        GlobalHeaderInjection: null,

        // disableAdvertising is provided for completeness, but changing it is not suggested
        // Disabling this may prevent your advertising-related PlayFab marketplace partners from working correctly
        disableAdvertising: false,
        AD_TYPE_IDFA: "Idfa",
        AD_TYPE_ANDROID_ID: "Adid"
    }
}

if(!PlayFab._internalSettings) {
    PlayFab._internalSettings = {
        entityToken: null,
        sdkVersion: "1.18.180122",
        sessionTicket: null,
        productionServerUrl: ".playfabapi.com",
        errorTitleId: "Must be have PlayFab.settings.titleId set to call this method",
        errorLoggedIn: "Must be logged in to call this method",
        errorEntityToken: "You must successfully call GetEntityToken before calling this",
        errorSecretKey: "Must have PlayFab.settings.developerSecretKey set to call this method",

        GetServerUrl: function () {
            return "https://" + PlayFab.settings.titleId + PlayFab._internalSettings.productionServerUrl;
        },

        InjectHeaders: function (xhr, headersObj) {
            if (!headersObj)
                return;

            for (var headerKey in headersObj)
            {
                try {
                    xhr.setRequestHeader(gHeaderKey, headersObj[headerKey]);
                } catch (e) {
                    console.log("Failed to append header: " + headerKey + " = " + headersObj[headerKey] + "Error: " + e);
                }
            }
        },

        ExecuteRequest: function (completeUrl, request, authkey, authValue, callback, customData, extraHeaders) {
            if (callback != null && typeof (callback) != "function")
                throw "Callback must be null of a function";

            if (request == null)
                request = {};

            var startTime = new Date();
            var requestBody = JSON.stringify(request);

            var xhr = new XMLHttpRequest();
            // window.console.log("URL: " + completeUrl);
            xhr.open("POST", completeUrl, true);

            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.setRequestHeader("X-PlayFabSDK", "JavaScriptSDK-" + PlayFab._internalSettings.sdkVersion);
            if (authkey != null)
                xhr.setRequestHeader(authkey, authValue);
            PlayFab._internalSettings.InjectHeaders(xhr, PlayFab.settings.GlobalHeaderInjection);
            PlayFab._internalSettings.InjectHeaders(xhr, extraHeaders);

            xhr.onloadend = function () {
                if (callback == null)
                    return;

                var result;
                try {
                    // window.console.log("parsing json result: " + xhr.responseText);
                    result = JSON.parse(xhr.responseText);
                } catch (e) {
                    result = {
                        code: 503, // Service Unavailable
                        status: "Service Unavailable",
                        error: "Connection error",
                        errorCode: 2, // PlayFabErrorCode.ConnectionError
                        errorMessage: xhr.responseText
                    };
                }

                result.CallBackTimeMS = new Date() - startTime;
                result.Request = request;
                result.CustomData = customData;

                if (result.code === 200)
                    callback(result, null);
                else
                    callback(null, result);
            }

            xhr.onerror = function () {
                if (callback == null)
                    return;

                var result;
                try {
                    result = JSON.parse(xhr.responseText);
                } catch (e) {
                    result = {
                        code: 503, // Service Unavailable
                        status: "Service Unavailable",
                        error: "Connection error",
                        errorCode: 2, // PlayFabErrorCode.ConnectionError
                        errorMessage: xhr.responseText
                    };
                }

                result.CallBackTimeMS = new Date() - startTime;
                result.Request = request;
                result.CustomData = customData;

                callback(null, result);
            }

            xhr.send(requestBody);
        }
    }
}

PlayFab.buildIdentifier = "jbuild_javascriptsdk_0";
PlayFab.sdkVersion = "1.18.180122";
PlayFab.GenerateErrorReport = function (error) {
    if (error == null)
        return "";
    var fullErrors = error.errorMessage;
    for (var paramName in error.errorDetails)
        for (var msgIdx in error.errorDetails[paramName])
            fullErrors += "\n" + paramName + ": " + error.errorDetails[paramName][msgIdx];
    return fullErrors;
};

PlayFab.ClientApi = {

    IsClientLoggedIn: function () {
        return PlayFab._internalSettings.sessionTicket != null && PlayFab._internalSettings.sessionTicket.length > 0;
    },
    ForgetAllCredentials: function () {
        PlayFab._internalSettings.sessionTicket = null;
        PlayFab._internalSettings.entityToken = null;
    },

    AcceptTrade: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AcceptTrade", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddFriend: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddFriend", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddGenericID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddGenericID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddOrUpdateContactEmail: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddOrUpdateContactEmail", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddSharedGroupMembers: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddSharedGroupMembers", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddUsernamePassword: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddUsernamePassword", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AddUserVirtualCurrency: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AddUserVirtualCurrency", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AndroidDevicePushNotificationRegistration: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AndroidDevicePushNotificationRegistration", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    AttributeInstall: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        var overloadCallback = function (result, error) {
            // Modify advertisingIdType:  Prevents us from sending the id multiple times, and allows automated tests to determine id was sent successfully
            PlayFab.settings.advertisingIdType += "_Successful";

            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/AttributeInstall", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, overloadCallback);
    },

    CancelTrade: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/CancelTrade", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ConfirmPurchase: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ConfirmPurchase", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ConsumeItem: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ConsumeItem", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    CreateSharedGroup: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/CreateSharedGroup", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ExecuteCloudScript: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ExecuteCloudScript", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetAccountInfo: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetAccountInfo", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetAllUsersCharacters: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetAllUsersCharacters", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCatalogItems: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCatalogItems", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCharacterData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCharacterData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCharacterInventory: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCharacterInventory", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCharacterLeaderboard: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCharacterLeaderboard", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCharacterReadOnlyData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCharacterReadOnlyData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCharacterStatistics: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCharacterStatistics", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetContentDownloadUrl: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetContentDownloadUrl", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetCurrentGames: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetCurrentGames", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetFriendLeaderboard: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetFriendLeaderboard", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetFriendLeaderboardAroundPlayer: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetFriendLeaderboardAroundPlayer", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetFriendsList: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetFriendsList", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetGameServerRegions: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetGameServerRegions", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetLeaderboard: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetLeaderboard", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetLeaderboardAroundCharacter: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetLeaderboardAroundCharacter", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetLeaderboardAroundPlayer: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetLeaderboardAroundPlayer", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetLeaderboardForUserCharacters: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetLeaderboardForUserCharacters", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPaymentToken: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPaymentToken", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPhotonAuthenticationToken: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPhotonAuthenticationToken", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerCombinedInfo: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerCombinedInfo", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerProfile: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerProfile", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerSegments: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerSegments", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerStatistics: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerStatistics", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerStatisticVersions: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerStatisticVersions", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerTags: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerTags", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayerTrades: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayerTrades", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromFacebookIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromFacebookIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromGameCenterIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromGameCenterIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromGenericIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromGenericIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromGoogleIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromGoogleIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromKongregateIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromKongregateIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromSteamIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromSteamIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPlayFabIDsFromTwitchIDs: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPlayFabIDsFromTwitchIDs", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPublisherData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPublisherData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetPurchase: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetPurchase", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetSharedGroupData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetSharedGroupData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetStoreItems: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetStoreItems", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetTime: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetTime", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetTitleData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetTitleData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetTitleNews: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetTitleNews", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetTitlePublicKey: function (request, callback, customData, extraHeaders) {
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetTitlePublicKey", request, null, null, callback, customData, extraHeaders);
    },

    GetTradeStatus: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetTradeStatus", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetUserData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetUserData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetUserInventory: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetUserInventory", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetUserPublisherData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetUserPublisherData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetUserPublisherReadOnlyData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetUserPublisherReadOnlyData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetUserReadOnlyData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetUserReadOnlyData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    GetWindowsHelloChallenge: function (request, callback, customData, extraHeaders) {
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GetWindowsHelloChallenge", request, null, null, callback, customData, extraHeaders);
    },

    GrantCharacterToUser: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/GrantCharacterToUser", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkAndroidDeviceID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkAndroidDeviceID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkCustomID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkCustomID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkFacebookAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkFacebookAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkGameCenterAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkGameCenterAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkGoogleAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkGoogleAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkIOSDeviceID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkIOSDeviceID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkKongregate: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkKongregate", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkSteamAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkSteamAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkTwitch: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkTwitch", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LinkWindowsHello: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LinkWindowsHello", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    LoginWithAndroidDeviceID: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithAndroidDeviceID", request, null, null, overloadCallback);
    },

    LoginWithCustomID: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithCustomID", request, null, null, overloadCallback);
    },

    LoginWithEmailAddress: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithEmailAddress", request, null, null, overloadCallback);
    },

    LoginWithFacebook: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithFacebook", request, null, null, overloadCallback);
    },

    LoginWithGameCenter: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithGameCenter", request, null, null, overloadCallback);
    },

    LoginWithGoogleAccount: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithGoogleAccount", request, null, null, overloadCallback);
    },

    LoginWithIOSDeviceID: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithIOSDeviceID", request, null, null, overloadCallback);
    },

    LoginWithKongregate: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithKongregate", request, null, null, overloadCallback);
    },

    LoginWithPlayFab: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithPlayFab", request, null, null, overloadCallback);
    },

    LoginWithSteam: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithSteam", request, null, null, overloadCallback);
    },

    LoginWithTwitch: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithTwitch", request, null, null, overloadCallback);
    },

    LoginWithWindowsHello: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/LoginWithWindowsHello", request, null, null, overloadCallback);
    },

    Matchmake: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/Matchmake", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    OpenTrade: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/OpenTrade", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    PayForPurchase: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/PayForPurchase", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    PurchaseItem: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/PurchaseItem", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RedeemCoupon: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RedeemCoupon", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RegisterForIOSPushNotification: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RegisterForIOSPushNotification", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RegisterPlayFabUser: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RegisterPlayFabUser", request, null, null, overloadCallback);
    },

    RegisterWithWindowsHello: function (request, callback, customData, extraHeaders) {
        request.TitleId = PlayFab.settings.titleId ? PlayFab.settings.titleId : request.TitleId; if (!request.TitleId) throw PlayFab._internalSettings.errorTitleId;
        var overloadCallback = function (result, error) {
            if (result != null && result.data.SessionTicket != null) {
                PlayFab._internalSettings.sessionTicket = result.data.SessionTicket;
                PlayFab.ClientApi._MultiStepClientLogin(result.data.SettingsForUser.NeedsAttribution);
            }
            if (callback != null && typeof (callback) == "function")
                callback(result, error);
        };
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RegisterWithWindowsHello", request, null, null, overloadCallback);
    },

    RemoveContactEmail: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RemoveContactEmail", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RemoveFriend: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RemoveFriend", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RemoveGenericID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RemoveGenericID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RemoveSharedGroupMembers: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RemoveSharedGroupMembers", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ReportDeviceInfo: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ReportDeviceInfo", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ReportPlayer: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ReportPlayer", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    RestoreIOSPurchases: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/RestoreIOSPurchases", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    SendAccountRecoveryEmail: function (request, callback, customData, extraHeaders) {
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/SendAccountRecoveryEmail", request, null, null, callback, customData, extraHeaders);
    },

    SetFriendTags: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/SetFriendTags", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    SetPlayerSecret: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/SetPlayerSecret", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    StartGame: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/StartGame", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    StartPurchase: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/StartPurchase", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    SubtractUserVirtualCurrency: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/SubtractUserVirtualCurrency", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkAndroidDeviceID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkAndroidDeviceID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkCustomID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkCustomID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkFacebookAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkFacebookAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkGameCenterAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkGameCenterAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkGoogleAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkGoogleAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkIOSDeviceID: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkIOSDeviceID", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkKongregate: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkKongregate", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkSteamAccount: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkSteamAccount", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkTwitch: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkTwitch", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlinkWindowsHello: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlinkWindowsHello", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlockContainerInstance: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlockContainerInstance", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UnlockContainerItem: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UnlockContainerItem", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateAvatarUrl: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateAvatarUrl", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateCharacterData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateCharacterData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateCharacterStatistics: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateCharacterStatistics", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdatePlayerStatistics: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdatePlayerStatistics", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateSharedGroupData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateSharedGroupData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateUserData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateUserData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateUserPublisherData: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateUserPublisherData", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    UpdateUserTitleDisplayName: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/UpdateUserTitleDisplayName", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ValidateAmazonIAPReceipt: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ValidateAmazonIAPReceipt", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ValidateGooglePlayPurchase: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ValidateGooglePlayPurchase", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ValidateIOSReceipt: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ValidateIOSReceipt", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    ValidateWindowsStoreReceipt: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/ValidateWindowsStoreReceipt", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    WriteCharacterEvent: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/WriteCharacterEvent", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    WritePlayerEvent: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/WritePlayerEvent", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    WriteTitleEvent: function (request, callback, customData, extraHeaders) {
        if (!PlayFab._internalSettings.sessionTicket) throw PlayFab._internalSettings.errorLoggedIn;
        PlayFab._internalSettings.ExecuteRequest(PlayFab._internalSettings.GetServerUrl() + "/Client/WriteTitleEvent", request, "X-Authorization", PlayFab._internalSettings.sessionTicket, callback, customData, extraHeaders);
    },

    _MultiStepClientLogin: function (needsAttribution) {
        if (needsAttribution && !PlayFab.settings.disableAdvertising && PlayFab.settings.advertisingIdType !== null && PlayFab.settings.advertisingIdValue !== null) {
            var request = {};
            if (PlayFab.settings.advertisingIdType === PlayFab.settings.AD_TYPE_IDFA)
                request.Idfa = PlayFab.settings.advertisingIdValue;
            else if (PlayFab.settings.advertisingIdType === PlayFab.settings.AD_TYPE_ANDROID_ID)
                request.Adid = PlayFab.settings.advertisingIdValue;
            else
                return;
            PlayFab.ClientApi.AttributeInstall(request, null);
        }
    }
};

var PlayFabClientSDK = PlayFab.ClientApi;

PlayFab.RegisterWithPhaser = function() {
    if ( typeof Phaser === "undefined" )
        return;

    Phaser.Plugin.PlayFab = function (game, parent) {
        Phaser.Plugin.call(this, game, parent);
    };
    Phaser.Plugin.PlayFab.prototype = Object.create(Phaser.Plugin.prototype);
    Phaser.Plugin.PlayFab.prototype.constructor = Phaser.Plugin.PlayFab;
    Phaser.Plugin.PlayFab.prototype.PlayFab = PlayFab;
    Phaser.Plugin.PlayFab.prototype.settings = PlayFab.settings;
    Phaser.Plugin.PlayFab.prototype.ClientApi = PlayFab.ClientApi;
};
PlayFab.RegisterWithPhaser();

