var watchID = null;
var positionDateTime;
var hasFix;
var m_PositionAverager;
var authToken;
var displayName;
var userName;
var siteTagCount;
var siteUserCount;
var siteHydrantCount;
var Tags;

function menuselect(event, ui) {
    alert("Hello");
}

function login() {
    var username = $("#txtUsername").val();
    var password = $("#txtPassword").val();

    $.ajax({
        type: "POST",
        url: "/login",
        headers: { "Username": username, "Password": password },
        success: function (response) {
            if (response.Result == "Success") {
                //Cache user info
                authToken = response.AuthorizationToken;
                userName = response.UserName;
                displayName = response.DisplayName;
                WriteToLocalStorage();

                window.location.replace("/home");
            }
            else if (response.Result == "NotActive") {
                $("#lblStatus").text("User has been deactivated.");
            }
            else if (response.Result == "NotVerified") {
                $("#lblStatus").text("Please verify your email.");
            }
            else {
                $("#lblStatus").text("Bad user or password combination.");
            }
        },
        error: LoginFailure,
        cache: false,
        contentType: false,
        processData: false
    });
}


function GetTags() {
    var username = localStorage.userName;
    var authToken = localStorage.authToken;

    $.ajax({
        type: "GET",
        url: "/rest/tags",
        headers: { "Username": username, "AuthorizationToken": authToken },
        success: GetTagsReceived,
        error: GetTagsFailure,
        cache: false,
        contentType: false,
        processData: false
    });
}

function GetTagsReceived(response) {
    $('#tags_table').dataTable({
        data: response.data,
        columns: [ { data: 'DeviceDateTime' } ]

    });
}

function GetTagsFailure() {
    
}

function LoginFailure(jqxhr, status, error) {
    alert(error);
}

//Load the security info from local storage
function LoadFromLocalStorage() {
    authToken = localStorage.authToken;
    displayName = localStorage.displayName;
    userName = localStorage.userName;
}

//Write security info to local storage
function WriteToLocalStorage() {
    localStorage.authToken = authToken;
    localStorage.displayName = displayName;
    localStorage.userName = userName;
}

//Clear security info from storage to log the user out
function LogoutAndClear() {
    authToken = null;
    displayName = null;
    userName = null;

    localStorage.removeItem("authToken");
    localStorage.removeItem("displayName");
    localStorage.removeItem("userName");
}