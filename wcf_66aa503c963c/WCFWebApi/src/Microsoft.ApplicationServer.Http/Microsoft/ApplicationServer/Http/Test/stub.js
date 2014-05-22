// only for testing home.htm locally, without wcf service running
// must be loaded after proxy.js in home.htm
// must not be added to SR.resx in order to fail loading with wcf service running

// override all methods in proxy.js

proxy.postText = function (fullText, curPos, callback) {
    var data;

    window.status = "sim json call at " + new Date() + "; curPos=" + curPos;

    var lastChar = fullText.substr(curPos - 1, 1);

    if ("0" <= lastChar && lastChar <= "9") {
        data = {
            autoCompleteList: ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119"]
        };
    }
    else {
        data = {
            autoCompleteList: ["Red", "Green", "Blue", "White", "Black"]
        };
    }

    callback(data);
};

proxy.postUriText = function (uriTemplate, httpMethod, curPos, fullText, callback) {

    return this.postText(fullText, curPos, callback);
};

proxy.postHeaderText = function (curPos, fullText, callback) {

    return this.postText(fullText, curPos, callback);
};

proxy.postHeaderText = function (curPos, fullText, callback) {

    return this.postText(fullText, curPos, callback);
};

proxy.postHttpVersionText = function (curPos, fullText, callback) {

    return this.postText(fullText, curPos, callback);
};