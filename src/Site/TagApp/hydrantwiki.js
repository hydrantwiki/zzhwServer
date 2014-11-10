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

//Load the security info from local storage
function LoadFromLocalStorage() {
    authToken = localStorage.authToken;
    displayName = localStorage.displayName;
    userName = localStorage.userName;
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

//Write security info to local storage
function WriteToLocalStorage() {
    localStorage.authToken = authToken;
    localStorage.displayName = displayName;
    localStorage.userName = userName;
}

function SetupApp() {
    LoadFromLocalStorage();
}

function Reauthorize() {
    if (userName == null
        || authToken == null) {
        
        //We don't have an auth token or a user so we need to setup login
        AddHomeLoginButton();
    }
    else {
        //So we have a user and auth already so lets try to use it.
        $.ajax({
            type: "POST",
            url: "/rest/reauthorize",
            headers: { "UserName": userName, "AuthorizationToken": authToken },
            success: function (response) {
                if (response.Result == "Success") {
                    AddHomeButtons();
                }
                else {
                    AddHomeLoginButton();
                }
            },
            error: ReauthorizeFailure,
            cache: false,
            contentType: false,
            processData: false
        });
    }
}

function ReauthorizeFailure(jqxhr, status, error) {
    alert(error);
}

function StatisticsFailure(jqxhr, status, error) {
    alert(error);
}

function Login(userNameText, password) {
    userName = userNameText.toLowerCase();

    $.ajax({
        type: "POST",
        url: "/rest/authorize",
        headers: { "Username": userName, "Password": password },
        success: function (response) {
            if (response.Result == "Success") {
                //Cache user info
                authToken = response.AuthorizationToken;
                userName = response.UserName;
                displayName = response.DisplayName;
                WriteToLocalStorage();

                //Go Home
                AddHomeButtons();
                $.mobile.changePage($("#home"), { transition: "slide", reverse: "true" });
               
            }
            else if (response.Result == "NotActive") {
                $("#lblStatus").text("User is has been deactivated.");
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

function LoginFailure(jqxhr, status, error) {
    alert(error);
}

function LoginButtonClicked() {
    var tempUserName = $("#txtUserName").val();
    var tempPassword = $("#txtPassword").val();

    Login(tempUserName, tempPassword);
}

//Add the QuickTag Button to the home scren
function AddHomeLoginButton() {
    $("#home_logoutbutton_wrapper").remove();

    //Register the button click event
    $("#btnLogin").click(LoginButtonClicked);

    ClearWatch();

    $('#home_content').empty();

    $('#home_content').append($('<p><input type="submit" id="home_loginbutton" value="Login" data-role="button"/></p>'));
    $('#home_loginbutton').button();

    $("#home_loginbutton").bind("click", function (event, ui) {
        $.mobile.changePage($("#login"), { transition: "slide" });
    });
};

function AddHomeButtons() {
    ClearWatch();

    $('#home_content').empty();
    AddQuickTagButton();

    AddLogoutButton();
}

function AddLogoutButton() {
    $('#home_footer').append($('<p id="home_logoutbutton_wrapper"><input type="submit" id="home_logoutbutton" value="Logout" data-role="button"/></p>'));
    $('#home_logoutbutton').button();

    $("#home_logoutbutton").bind("click", function (event, ui) {
        LogoutAndClear();

        AddHomeLoginButton();
    });
}

function AddQuickTagButton() {

    $('#home_content').append($('<p><input type="submit" id="home_quicktagbutton" value="Quick Tag Hydrant" data-role="button"/></p>'));
    $('#home_quicktagbutton').button();

    $("#home_quicktagbutton").bind("click", function (event, ui) {
        ClearWatch();

        ResetQuickTagForm();
        SetLocationEmpty();

        StartWatch();

        $.mobile.changePage($("#quicktag"), { transition: "slide" });
    });
};


//Add the QuickTag Button to the home scren
function AddQuickTagButton() {

    $('#home_content').empty();

    $('#home_content').append($('<p><input type="submit" id="home_quicktagbutton" value="Quick Tag Hydrant" data-role="button"/></p>'));
    $('#home_quicktagbutton').button();

    $("#home_quicktagbutton").bind("click", function (event, ui) {
        ClearWatch();

        ResetQuickTagForm();
        SetLocationEmpty();

        StartWatch();

        $.mobile.changePage($("#quicktag"), { transition: "slide" });
    });
};

//Start a position watcher
function StartWatch() {
    if (watchID == null) {
        hasFix = false;

        m_PositionAverager = new PositionAverager();

        watchID = navigator.geolocation.watchPosition(UpdateLocationData, HandleLocationError);
    }
}

//Reset the QuickTag Form
function ResetQuickTagForm() {
    $('#quicktag_content').empty();

    //Add the form
    $('#quicktag_content').append($('<form id="quicktag_form" method="post" enctype="multipart/form-data"></form>'));

    //Post back the authorization token and userid.
    $('#quicktag_form').append($('<input type="hidden" id="latitudeInput" name="latitudeInput" value="" />'));
    $('#quicktag_form').append($('<input type="hidden" id="longitudeInput" name="longitudeInput" value="" />'));
    $('#quicktag_form').append($('<input type="hidden" id="accuracyInput" name="accuracyInput" value="" />'));
    $('#quicktag_form').append($('<input type="hidden" id="positionDateTimeInput" name="positionDateTimeInput" value="" />'));

    //Add the position label
    $('#quicktag_form').append($('<p id="locationLabel"></p>'));

    //Add the File input method
    $('#quicktag_form').append($('<p><input type="file" name="imageInput" id="quicktag_image" accept="image/*" onchange="EnableButtonIfReady()" /></p>'));

    //Add the Submit button.
    $('#quicktag_form').append($('<p><button id="quicktag_save" value="Save Tag" type="submit" disabled/></p>'));
    $('#quicktag_save').button();

    $("#quicktag_save").bind("click", PostFormStart);
}

//Let the user know that the Hydrant was successfully sent to the server.
function PostReturn(response) {
    if (response.Result == "Success") {
        $.mobile.changePage($("#home"), { transition: "slide", reverse: "true" });

        alert('Thanks - Tag posted to server.'); //
    } else if (response.Result == "Failure - No position") {
        alert('A position is required for a hydrant');
    } else {
        alert('An unknown error occurred.');
    }
    
}

//Start the ajax post to the server of the tag and photo
function PostFormStart() {
    $('#quicktag_save').button('disable');

    try {
        ClearWatch();

        var formData = new FormData();
        formData.append('file', $('#quicktag_image')[0].files[0]);
        formData.append('latitudeInput', $('#latitudeInput').val());
        formData.append('longitudeInput', $('#longitudeInput').val());
        formData.append('accuracyInput', $('#accuracyInput').val());
        formData.append('positionDateTimeInput', $('#positionDateTimeInput').val());

        $.ajax({
            type: "POST",
            url: "/rest/tag",
            headers: { "UserName": userName, "AuthorizationToken": authToken },
            data: formData,
            success: PostReturn,
            cache: false,
            contentType: false,
            processData: false
        });
    }
    catch (err) {
        alert(err);
        $('#quicktag_save').button('enable');
    }
}

//Handle a location error
function HandleLocationError(error) {
    //Todo - handle no permissions
    $('#locationLabel').empty();
    $('#locationLabel').append("Location Error: " + error);
}

//Clear out the location info
function SetLocationEmpty() {
    $('#locationLabel').empty();
    $('#locationLabel').append("Latitude: Acquiring</br>Longitude: Acquiring</br>Accuracy: Acquiring</br>Count: 0");
}

//New location data has arrived.
function UpdateLocationData(location) {
    var latitude = location.coords.latitude;
    var longitude = location.coords.longitude;
    var accuracy = location.coords.accuracy;

    if (m_PositionAverager) {
        var hwcoord = new HWCoordinate(latitude, longitude, accuracy, new Date());

        m_PositionAverager.AddPosition(hwcoord);
        var hwOutput = m_PositionAverager.GetAverage();

        if (hwOutput) {
            var posCount = m_PositionAverager.PositionCount();
            var tempString = GetLocationString(hwOutput.Latitude, hwOutput.Longitude, hwOutput.Accuracy, posCount);

            $('#locationLabel').empty();
            $('#locationLabel').append(tempString);

            $('#latitudeInput').val(hwOutput.Latitude);
            $('#longitudeInput').val(hwOutput.Longitude);
            $('#accuracyInput').val(hwOutput.Accuracy);
            $('#positionDateTimeInput').val(hwOutput.Time);

            if (posCount >= 10) {
                ClearWatch();
            }
        }
    }

    hasFix = true;

    EnableButtonIfReady();
}

function EnableButtonIfReady() {
    var imageNull = false;
    var temp = $('#quicktag_image');

    if (temp.val() == null
        || temp.val()=="") {
        imageNull = true;
    }

    if (!imageNull && hasFix) {
        $('#quicktag_save').button('enable');
    }
}

function ClearWatch() {
    if (watchID != null) {
        navigator.geolocation.clearWatch(watchID);
        watchID = null;
    }

    m_PositionAverager = null;
}

function GetLocationString(lat, lon, accuracy, positionCount) {
    return '<span class="hw_position">Latitude: ' + lat.toFixed(6) + '<br/>Longitude: ' + lon.toFixed(6) + '<br/>Accuracy: ' + accuracy.toFixed(1) + '<br/>Count: ' + positionCount + '<span/>';
}

function PositionAverager() {
    this.Positions = new Array();
    this.TotalCount = 0;

    this.AddPosition = function(hwcoordinate) {
        if (hwcoordinate.Accuracy <= 50
            && hwcoordinate.Latitude != 0.0
            && hwcoordinate.Longitude != 0.0) {
            this.Positions[this.Positions.length] = hwcoordinate;
        }

        this.TotalCount++;
    };

    this.PositionCount = function() {
        return this.Positions.length;
    };

    this.GetAverage = function() {
        var tLat = 0;
        var tLon = 0;
        var tAcc = 0;
        var posCount = 0;

        for (var i = 0; i < this.Positions.length; i++) {
            var coord = this.Positions[i];

            if (coord.Latitude != 0.0
                && coord.Longitude != 0.0) {

                tLat = tLat + coord.Latitude;
                tLon = tLon + coord.Longitude;
                tAcc = tAcc + coord.Accuracy;
                posCount++;
            }
        }

        if (posCount > 0) {
            var output = new HWCoordinate(tLat / posCount, tLon / posCount, tAcc / posCount, new Date());

            return output;
        }

        return null;
    };


}

function HWCoordinate(lat, lon, acc, time) {
    this.Latitude = lat;
    this.Longitude = lon;
    this.Accuracy = acc;
    this.TimeStamp = time;
}