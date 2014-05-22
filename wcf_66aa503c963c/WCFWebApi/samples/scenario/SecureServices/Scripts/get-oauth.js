$(function () {
    $("#getGreeting").click(GetComments);
    $("#getPersonalizedGreeting").click(GetPersonalizedGreeting);
});

function GetComments(){
    $.ajax({
		url: "greeting",
		cache: false,
		statusCode: {
			200: function(data) {
				BuildList(data);
			}
		}
    });
}

function GetPersonalizedGreeting(){
	var personalized = $("#txtGreetingPersonalizer").val();
    $.ajax({ url: "greeting/" + personalized,
        accepts: "application/json",
        cache: false,
        statusCode: {
            200: function(data){
                BuildList(data);
            },
            
            401: function (jqXHR, textStatus, errorThrown) {
                var wwwAuthHeaderValue = jqXHR.getResponseHeader("WWW-Authenticate");
                    
                //get the location paramater and call it in an ajax modal
                var re = /\w+\slocation="([/\.\?\=:&\w]+)"/i;
                var results = re.exec(wwwAuthHeaderValue);
                var authZServerUrl = results[1];
                    
                alert("navigating to: " + authZServerUrl);
                var w = window.open(authZServerUrl, null, 'width=900,height=600,toolbar=no,menubar=no');
            },
            
            403: function() {
                alert('Sorry, you are not authorized to make this request.');
            }
        }
    });
}

function BuildList(data){
	$("#txtGreeting").val(data);
}

function OAuthDlgClosed(){
	GetPersonalizedGreeting();
}