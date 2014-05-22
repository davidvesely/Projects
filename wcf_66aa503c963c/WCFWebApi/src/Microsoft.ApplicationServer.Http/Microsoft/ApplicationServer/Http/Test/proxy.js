var proxy = {
    baseAddress: ""
};

proxy.postText = function (url, fullText, callback) {
    window.status = "sending request to server ...";
    var jqxhr = $.post(url, fullText, function (data) {
        window.status = "";
        callback(data);
    });
    jqxhr.error(function () {
        window.status = jqxhr.statusText; // +":" + jqxhr.responseText;
    });
}

proxy.postUriText = function (uriTemplate, httpMethod, curPos, fullText, callback) {
    var url = "request/uri?uri=" + encodeURIComponent(uriTemplate) + "&method=" + encodeURIComponent(httpMethod) + "&op=autocomplete" + "&cursorpos=" + curPos;
    return this.postText(url, fullText, callback);
};

proxy.postHeaderText = function (curPos, fullText, callback) {
    var url = "request/headers?cursorpos=" + curPos;
    return this.postText(url, fullText, callback);
};

proxy.postBodyText = function (uriTemplate, httpMethod, format, curPos, fullText, callback) {
    var url = "request/content?uri=" + encodeURIComponent(uriTemplate) + "&method=" + encodeURIComponent(httpMethod) + "&op=autocomplete" + "&cursorpos=" + curPos + "&format0=" + format;
    return this.postText(url, fullText, callback);
};

proxy.validateText = function (type, uriTemplate, httpMethod, format, fullText, callbackSuc, callbackErr) {
    var url = "";
    
    if(type == 'uri'){
        url = "request/uri?uri=" + encodeURIComponent(uriTemplate) + "&method=" + encodeURIComponent(httpMethod) + "&op=validate";
    }
    else{
        url = "request/content?uri=" + encodeURIComponent(uriTemplate) + "&method=" + encodeURIComponent(httpMethod) + "&op=validate&format0="+format;
    }
    
    var jqxhr = $.post(url, fullText, function (isValid) {
        callbackSuc(isValid);
    });
    
    jqxhr.error(function () {
        callbackErr();
    });
};

proxy.formatRequestBody = function(uriTemplate, httpMethod, rawRequestBody, format, callbackSuc, callbackErr){
    var url = "request/content?uri=" + encodeURIComponent(uriTemplate) + "&method=" + encodeURIComponent(httpMethod) + "&op=format" + "&format0=" + format;
    $.ajaxSetup({async: false, timeout:10000});
    var jqxhr =  $.post(url, rawRequestBody, function (formattedText) {
        callbackSuc(formattedText);
    });
    
    jqxhr.error(function () {
        callbackErr();
    });
    $.ajaxSetup({async: true});
};

proxy.postResContText = function (rawResponse, format, callbackSuc, callbackErr) {
    var resContFormaterURL = "response/content?format="+format;
    $.ajaxSetup({async: false, timeout:10000});
    var jqxhr =  $.post(resContFormaterURL, rawResponse, function (formattedText) {
        callbackSuc(formattedText);
    });
    
    jqxhr.error(function () {
        callbackErr();
    });
    $.ajaxSetup({async: true});
};

proxy.convertRequestBodyFormat = function (uriTemplate, httpMethod, currentFormat, targetFormat, requestBodyText, callbackSuc, callbackErr) {
    var url = "request/content?uri=" + encodeURIComponent(uriTemplate) + "&method="+ encodeURIComponent(httpMethod) + "&op=convert&format0=" + currentFormat +"&format1=" + targetFormat;
    $.ajaxSetup({async: false, timeout:10000});
    var jqxhr =  $.post(url, requestBodyText, function (convertResult) {
        callbackSuc(convertResult);
    });
    
    jqxhr.error(function () {
        callbackErr();
    });
    $.ajaxSetup({async: true});
};

proxy.getResource = function (callbackSuc, callbackErr) {
    $.ajaxSetup({cache: false});
    var jqxhr = $.getJSON("resources", function (data) {
        callbackSuc(data);
    });
    
    jqxhr.error(function () {
        callbackErr();
    });
}
