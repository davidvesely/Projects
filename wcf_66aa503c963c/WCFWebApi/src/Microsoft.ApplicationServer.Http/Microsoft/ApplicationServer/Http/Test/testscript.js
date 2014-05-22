var contentTypePatterns = { 'xml': /(text|application)\s*\/\s*xml/i, 'json': /(text|application)\s*\/\s*json/i, 'raw': /(text)\s*\/\s*plain/i };
var defaultContentTypes = { 'xml': 'text/xml', 'json': 'application/json', 'raw': 'text/plain' };
var httpMethodsNoBody = ['GET', 'HEAD', 'MOVE', 'TRACE', 'DELETE', 'CONNECT', 'MKCOL', 'COPY', 'UNLOCK', 'OPTIONS'];
var formatList = ['Xml', 'Json', 'Raw'];
var server = {};
var currentResource = null;
var invokeHistory = {'records':[], 'maxWidth':0};
var autoCompletesResetList = [];
var currentFormat = 0;
var formatTabs = null;
var contentLengthRegex = /^\s*Content\-Length\s*(\:|$)/i;
var contentTypeRegex = /^\s*Content\-Type\s*(\:[^\;]*|$)/i;
var defaultHttpMethod = 'GET';

(function ($) {
    $.tree = function (uri) {
        this.uri = uri;
        this.indexInServer = -1;
        this.subtree = new Array();
    };

    $.extend($.tree.prototype, {
        show: function (level) {
            var content = ["<div>"];

            for (var i = 0; i < level * 4; i++) {
                content.push("&nbsp;");
            }

            if (this.indexInServer >= 0) {
                content.push("<a class='resourceTreeNode' href='javascript:void(0);' indexInServer='");
                content.push(this.indexInServer);
                content.push("' title='");
                content.push(server.Resources[this.indexInServer].Uri);
                content.push("'>");
                content.push(this.uri);
                content.push("</a>");
            }
            else {
                content.push(this.uri);
            }

            content.push("</div>");            

            for (var i = 0; i < this.subtree.length; i++) {
                content.push(this.subtree[i].show(level + 1));
            }

            return content.join('');
        },

        add: function (uri, indexInServer) {
            if (uri.length == 0) {
                this.indexInServer = indexInServer;
                return;
            }
            else {
                var j = uri.indexOf("/");
                if (j < 0) {
                    j = uri.length;
                }
                var sub_uri = uri.substring(0, j);
                var sub_tree_id = -1;
                for (var i = 0; i < this.subtree.length; i++) {
                    if (this.subtree[i].uri == sub_uri) {
                        sub_tree_id = i;
                        break;
                    }
                }

                if (sub_tree_id < 0) {
                    this.subtree.push(new $.tree(sub_uri));
                    sub_tree_id = this.subtree.length - 1;
                }

                this.subtree[sub_tree_id].add(uri.substring(j + 1), indexInServer);
            }
        }

    });

    $.resourceTree = function () {
    };

    $.extend($.resourceTree.prototype, {
        stretchItemsWidthToSame: function(controls){
            var maxWidth = 0;
            var width = 0;
            for(var i=0; i<controls.length; i++)
            {
                width = $(controls[i]).width();
                maxWidth = (maxWidth > width)? maxWidth: width;
            }
            
            controls.width(maxWidth);
        },
		
		getHostNormalizedUri: function(uri){
			var i = uri.indexOf('//');
			if(i < 0){
				return uri;
			}
			
			i += 2;
			var j = uri.indexOf('/', i);
			if(j < 0){
				return uri;
			}
			
			return uri.slice(0, i) + window.location.host + uri.slice(j);		
		},
        
        build: function (placeHolder, callback) {
            var THIS = this;
            proxy.getResource(function (data) {
                    server = data;
                    if(server.Resources.length > 0){
                        var baseAddress = THIS.getHostNormalizedUri(server.Resources[0].BaseAddress);
                        var resourcesTree = new $.tree(baseAddress);
                        var length = (baseAddress.charAt(baseAddress.length-1) == '/')? baseAddress.length : baseAddress.length + 1;
                        
                        for (i = 0; i < server.Resources.length; i++) {
                            resourcesTree.add(THIS.getHostNormalizedUri(server.Resources[i].Uri).substring(length), i);
                            
                            server.Resources[i].operationMap = {};
                            for(j=0; j<server.Resources[i].Operations.length; j++){
                                server.Resources[i].operationMap[server.Resources[i].Operations[j].HttpMethod] = server.Resources[i].Operations[j].Name;
                            }
                        }
                        
                        var content = resourcesTree.show(0);
                        placeHolder.html(content);
                        THIS.stretchItemsWidthToSame(placeHolder.contents());
                        $(".resourceTreeNode").click(callback);
                        $("#resources").show();
                        
                        if(server.HelpEnabled){
                            $("#sampleButton").show();
                        }
                        else{
                            $("#sampleButton").hide();
                        }
                    }
                },
                function(){
                    $("#resources").hide();
                });
        }
    });

    })(jQuery);

function lengthInUtf8Bytes(str) {  
    var m = encodeURIComponent(str).match(/\%[89ABab]/g);  
    return str.length + (m ? m.length : 0);
    }

function setContentLength(str) {
    var requestHeaderTextArea = $("#requestHeader");
    var headers = requestHeaderTextArea.val().split('\n');
    var contentLength = "Content-Length:" + lengthInUtf8Bytes(str);
    var contentLengthExists = false;
    for (var i = 0; i < headers.length; i++) {
        if (headers[i].match(contentLengthRegex)) {
            headers[i] = contentLength;
            contentLengthExists = true;
        }
    }

    headers = headers.join('\n');

    if (!contentLengthExists) {
        headers = headers.replace(/\s*$/, "");
        headers = headers + "\n" + contentLength;
    }

    requestHeaderTextArea.val(headers);
}

function setContentType(format) {
    if (format in defaultContentTypes) {
        var contentType = "Content-Type:" + defaultContentTypes[format];
        var requestHeaderTextArea = $("#requestHeader");
        var headers = requestHeaderTextArea.val().split('\n');
        var contentTypeExists = false;
        for (var i = 0; i < headers.length; i++) {
            var match = headers[i].match(contentTypeRegex)
            if (match) {
                contentTypeExists = true;
                if(match[0].match(contentTypePatterns[format]))
                {
                    return;
                }

                headers[i] = headers[i].replace(contentTypeRegex, contentType);
            }
        }

        headers = headers.join('\n');

        if(!contentTypeExists)
        {
            headers = headers.replace(/\s*$/, "");
            headers = headers + "\n" + contentType;
        }

        requestHeaderTextArea.val(headers);
    }
}

function selectVariableInUriTextbox(textbox) {
    var val = textbox.val();
    var selectionEnd = textbox.getSelection().end;
    var s, e;
    s = val.lastIndexOf("{", selectionEnd);
    if (s < 0) {
        return;
    }

    e = val.indexOf("}", s);
    e = (e < 0) ? val.length : e + 1;

    textbox.setSelection({
        start: s,
        end: e
    });
}

function enableIntellisense(control, postTextCallback) {
    var autoComplete = control.enableAutoComplete();

    control.keydown(function (event) {
        return autoComplete.onKeyDown(event);
    });

    control.keyup(function (event) {
        return autoComplete.onKeyUp(event, function (fullText, curPos, callback) {

            return postTextCallback(fullText, curPos, callback);
        });
    });

    control.click(function (event) {
        return autoComplete.onMouseClick(event);
    });

    control.focus(function (event) {
        return autoComplete.onFocus(event);
    });

    control.blur(function (event) {
        return autoComplete.onBlur(event);
    });

    return autoComplete;
}

function enableUriIntellisense(uri, uriTemplate, httpMethod) {
    var autoComplete = enableIntellisense(uri, function (fullText, curPos, callback) {

        return proxy.postUriText(uriTemplate.val(), httpMethod.val(), curPos, fullText, callback);
    });

    autoComplete.regexContextId(/^.*[\/\?\=\&\#]/);
    return autoComplete;
}

function enableHeaderIntellisense(header) {
    var autoComplete = enableIntellisense(header, function (fullText, curPos, callback) {

        return proxy.postHeaderText(curPos, fullText, callback);
    });

    autoComplete.regexContextId(/^(.*\n)*([^\:]+\:)?/);
    return autoComplete;
}

function enableRequestBodyXmlIntellisense(body, uriTemplate, httpMethod) {
    var autoComplete = enableIntellisense(body, function (fullText, curPos, callback) {

        return proxy.postBodyText(uriTemplate.val(), httpMethod.val(), "xml", curPos, fullText, callback);
    });

    // (\s|\S) is stronger than . because the former also matches \n
    autoComplete.regexContextId(/^(\s|\S)*(\<\/|\<([^\>]*(\s|\"))?|\>)/);         
    return autoComplete;
}

function enableRequestBodyJsonIntellisense(body, uriTemplate, httpMethod) {
    var autoComplete = enableIntellisense(body, function (fullText, curPos, callback) {

        return proxy.postBodyText(uriTemplate.val(), httpMethod.val(), "json", curPos, fullText, callback);
    });

    autoComplete.regexContextId(/^(\s|\S)*[\{\}\"\:\[\]\,]/);
    return autoComplete;
}

function enableHttpMethodIntellisense(httpMethod) {
    var autoComplete = enableIntellisense(httpMethod, function (fullText, curPos, callback) {
        return getHttpMethodAutoCompleteList(curPos, fullText, callback);
    });

    autoComplete.filteringOn1stChar(true);
    autoComplete.comboboxMode(true);
    return autoComplete;
}

function getHttpMethodAutoCompleteList(curPos, fullText, callback) {
    var data = {
        autoCompleteList: ["GET", "POST", "PUT", "DELETE", "HEAD", "PATCH", "TRACE", "OPTIONS"]
    };
    if (!currentResource) {
        // no resource is selected yet
        // assume every method is applicable
        return callback(data);
    }

    var map = {};
    for (var j = 0; j < currentResource.Operations.length; j++) {
        map[currentResource.Operations[j].HttpMethod] = true;
    }

    // need to mark not applicable methods
    for (var i = 0; i < data.autoCompleteList.length; i++) {
        var method = data.autoCompleteList[i];
        var applicable = map[method];
        if (!applicable) {
            data.autoCompleteList[i] = method + "\n" + method + "\n\n\nfalse";
        }
        else{
            delete map[method];
        }
    }
    
    for (var key in map){
        data.autoCompleteList.push(key);
    }

    return callback(data);
}

function clearLastRequest() {
    $("#httpMethodTextBox").val("");
    $("#uri").val("");
    $("#requestHeader").val("Accept:*/*");
    
    for(var i=0; i<formatList.length; i++){
        $("#requestBody"+formatList[i]).val("");
    }
    clearAllValidationResult();   
    clearAutoCompleteResetListContext(0);   // also reset http-method textbox	
}

function clearLastResponse() {
    $("#statusBar").text("");
    $("#status").text("");
    $("#responseHeader").val("");
    $("#responseBody").val("");
}

function clearLastSession() {
    clearLastResponse();
    clearLastRequest();
}

function initResourceTree() {
    var rt = new $.resourceTree();
    rt.build(
        $("#resourceTree"),
        function (event) {
            clearLastSession();

            var target = $(event.target);
            var i = target.attr("indexInServer") - 0;
            currentResource = server.Resources[i];

            // host name in the url could be wrong, e.g. in Azure case, service returns urls with service's internal host name
            // fixed here with the real host name in order to invoke the service, e.g. in Azure case, must use the external host name of the service
            $("#uri").val(rt.getHostNormalizedUri(currentResource.Uri));

            // the url template here is used to match web api operation on service side
            // so keep the host name got from service side
            $("#uriTemplate").val(currentResource.Uri);

            // make 'GET' as the default httpmethod as long as the selected service supports 'GET'.
            if (defaultHttpMethod in currentResource.operationMap) {
                $("#httpMethodTextBox").val(defaultHttpMethod);
            }
            else {
                $("#httpMethodTextBox").val((currentResource.Operations[0].HttpMethod));
            }

            $("#httpMethodTextBox").change();

            enableFormattingButton(currentFormat);

            $(".resourceSelected").removeClass("resourceSelected");
            target.parent().addClass("resourceSelected");

            $("#uri").focus();

            return false;
        });
}

function dateToString(date) {
    var h = date.getHours();
    var m = date.getMinutes();
    var s = date.getSeconds();
    var ms = date.getMilliseconds();
    return "time:  " + Math.floor(h / 10) + h % 10 + "." + Math.floor(m / 10) + m % 10 + "." + Math.floor(s / 10) + s % 10+ "." + Math.floor(ms / 100) + Math.floor(ms / 10) + ms % 10;
}

(function ($) {
    $.invokeRecord = function () {
        this._resource = null,
        this._httpMethod = "";
        this._uri = "";
        this._format = "";
        this._uriTemplate = "";
        this._requestHeader = "";
        this._requestBody = "";
        this._responseHeader = "";
        this._responseBody = "";
        this._duration = 0;
        this._statusCode = 0;
        this._statusText = "";
        this._requestTimeStr = "";
        this._responseTimeStr = "";
    };

    $.extend($.invokeRecord.prototype,
        {
            resource: function (value) {
                if (value) {
                    // setter
                    this._resource = value;
                }
                else {
                    // getter
                    return this._resource;
                }
            },

            httpMethod: function (value) {
                if (value) {
                    // setter
                    this._httpMethod = value;
                }
                else {
                    // getter
                    return this._httpMethod;
                }
            },

            uri: function (value) {
                if (value) {
                    // setter
                    this._uri = value;
                }
                else {
                    // getter
                    return this._uri;
                }
            },
            
            format: function (value) {
                if (value != null) {
                    // setter
                    this._format = value;
                }
                else {
                    // getter
                    return this._format;
                }
            },


            uriTemplate: function (value) {
                if (value) {
                    // setter
                    this._uriTemplate = value;
                }
                else {
                    // getter
                    return this._uriTemplate;
                }
            },

            requestHeader: function (value) {
                if (value) {
                    // setter
                    this._requestHeader = value;
                }
                else {
                    // getter
                    return this._requestHeader;
                }
            },

            requestBody: function (value) {
                if (value) {
                    // setter
                    this._requestBody = value;
                }
                else {
                    // getter
                    return this._requestBody;
                }
            },
            
            
            responseHeader: function (value) {
                if (value) {
                    // setter
                    this._responseHeader = value;
                }
                else {
                    // getter
                    return this._responseHeader;
                }
            },

            responseBody: function (value) {
                if (value) {
                    // setter
                    this._responseBody = value;
                }
                else {
                    // getter
                    return this._responseBody;
                }
            },

            duration: function (value) {
                if (value || value == 0) {
                    // setter
                    this._duration = value;
                }
                else {
                    // getter
                    return this._duration;
                }
            },
            
            requestTimeStr: function (value) {
                if (value) {
                    // setter
                    this._requestTimeStr = value;
                }
                else {
                    // getter
                    return this._requestTimeStr;
                }
            },
            
            responseTimeStr: function (value) {
                if (value) {
                    // setter
                    this._responseTimeStr = value;
                }
                else {
                    // getter
                    return this._responseTimeStr;
                }
            },
            
            statusText: function (value) {
                if (value) {
                    // setter
                    this._statusText = value;
                }
                else {
                    // getter
                    return this._statusText;
                }
            },

            statusCode: function (value) {
                if (value || value == 0) {
                    // setter
                    this._statusCode = value;
                }
                else {
                    // getter
                    return this._statusCode;
                }
            },

            show: function (index) {
                var html = [];
                var i = 0;
                html.push("<a href='javascript:void(0);' class='historyRecord'");
                html.push("' queueIndex='");
                html.push(index);
                html.push("'>");
                
                html.push("<span class='historyIndex'>");
                html.push(index+1);
                html.push("</span>");
                
                html.push("<span class='historyDuration'>");
                html.push(this._duration);
                html.push("s</span>");

                html.push("<span class='historyStatus ");
                if (this._statusCode < 400) {
                    html.push("requestStatusOK'>");
                }
                else {
                    html.push("requestStatusError'>");
                }
                html.push(this._statusCode);
                html.push("</span>");

                html.push("<span class='historyHttpMethod'>");
                html.push(this._httpMethod);
                html.push("</span>");

                html.push("<span>");
                
                if(this._resource && this._uri.indexOf(this._resource.BaseAddress)==0){
                    html.push((this._resource.BaseAddress.length < this._uri.length) ? this._uri.substring(this._resource.BaseAddress.length) : this._uri);
                }
                else{
                    html.push(this._uri);
                }
                
                html.push("</span>");
                html.push("</a>");
                
                var _a = $(html.join(""));
                _a.click(function (event) {
                    var target = $(this);
                    var queueIndex = target.attr("queueIndex") - 0;
                    var ihr = invokeHistory.records[queueIndex];

                    clearLastSession();

                    currentResource = ihr.resource();

                    $("#uri").val(ihr.uri());
                    $("#uriTemplate").val(ihr.uriTemplate());
                    $("#requestHeader").val(ihr.requestHeader());
                    currentFormat = ihr.format();
                    enableFormattingButton(currentFormat);
                    $("#requestBody"+formatList[currentFormat]).val(ihr.requestBody());
                    formatTabs.tabs('select', currentFormat);                    
                    $("#httpMethodTextBox").val(ihr.httpMethod());
                    $("#httpMethodTextBox").change();
                    
                    $("#responseHeader").val(ihr.responseHeader());
                    $("#responseBody").val(ihr.responseBody());
                    $("#statusBar").text("Request " + ihr.requestTimeStr()+"   Response " + ihr.responseTimeStr() + "  Duration: " + ihr.duration()+"s");
                    
                    $("#status").text(ihr.statusCode() + "/" + ihr.statusText());
                    if (ihr.statusCode() < 400) {
                        $("#status").switchClass("requestStatusError","requestStatusOK", 0);
                    }
                    else {
                        $("#status").switchClass("requestStatusOK","requestStatusError", 0);
                    }
                });

                return _a;
            }
        }
    );
})(jQuery);

function enableAllIntellisense() {
    autoCompletesResetList.push(enableHttpMethodIntellisense($("#httpMethodTextBox")));
    enableHeaderIntellisense($("#requestHeader"));
    autoCompletesResetList.push(enableUriIntellisense($("#uri"), $("#uriTemplate"), $("#httpMethodTextBox")));
    autoCompletesResetList.push(enableRequestBodyXmlIntellisense($("#requestBodyXml"), $("#uriTemplate"), $("#httpMethodTextBox")));
    autoCompletesResetList.push(enableRequestBodyJsonIntellisense($("#requestBodyJson"), $("#uriTemplate"), $("#httpMethodTextBox")));
}

function clearAutoCompleteResetListContext(index)
{
    for (var i = index; i < autoCompletesResetList.length; i++) {
        autoCompletesResetList[i].clearContext();
    }
}

function clearValidationWarning(control){
    control.css('visibility', 'hidden');
    control.removeAttr('title');
}

function setValidationWarning(control, warningMessage){
    control.css('visibility', 'visible');
    control.attr('title', warningMessage);
}

function clearAllValidationResult(){
    clearValidationWarning($("#httpMethodValidationIndicator"));
    clearValidationWarning($("#uriValidationIndicator"));
    clearValidationWarning($("#requestBodyValidationIndicator"));   
}

function validateHttpMethod(httpMethod){
    var indicator = $("#httpMethodValidationIndicator");
    clearValidationWarning(indicator);
    if(currentResource && !(httpMethod in currentResource.operationMap)) {
        setValidationWarning(indicator, "The selected method may not be available, and autocomplete of uri and request body will be affected.");
    }
}

function validateUriOrRequestBody(control){
    var type = null;
    var indicator = null;
    if(control.attr('id') == 'uri'){
        type = 'uri';
        indicator = $("#uriValidationIndicator");
    }
    else{
        type = 'content';
        indicator = $("#requestBodyValidationIndicator");
    }
    
    clearValidationWarning(indicator);
    
    if(!currentResource){
        return;
    }
    
    var uriTemplate = $("#uriTemplate").val();
    var httpMethod  = $("#httpMethodTextBox").val();
    var fullText = control.val();
    if(uriTemplate.length==0 || httpMethod.length==0 || fullText.length==0){
        return;
    }
    
    var context = httpMethod + uriTemplate + currentFormat + fullText;
        
    proxy.validateText(
        type, 
        uriTemplate, 
        httpMethod, 
        formatList[currentFormat], 
        fullText, 
        function(isValid){
            var _context = $("#httpMethodTextBox").val() + $("#uriTemplate").val() + currentFormat + control.val();
            if(context != _context){
                return;  //the validation result is out of date.
            }


            if (!isValid) {
                if(type == 'uri') {
                    setValidationWarning(indicator, "The uri does not match currently selected resource.");
                }
                else {
                    setValidationWarning(indicator, "The request body may not be expected given currently selected resource.");
                }
            }
            else {
                clearValidationWarning(indicator);
            }
        },
        function(){
            clearValidationWarning(indicator);
    });
}

function enableFormattingButton(format){
    if((format==0 || format==1)){  // xml or json
        if($("#formatButton").is('.unclickableLink')){
            $("#formatButton").attr('href',"javascript:void(0);");
            $("#formatButton").switchClass('unclickableLink', 'clickableLink', 0);
        }
    }
    else {
        if($("#formatButton").is('.clickableLink')){
            $('#formatButton').removeAttr('href');
            $("#formatButton").switchClass('clickableLink', 'unclickableLink', 0);
        }
    }
}

function updateSampleLink(httpMethod){
    if(currentResource && currentFormat!=2 && currentResource.operationMap[httpMethod]){
        var href = currentResource.BaseAddress+"/help/operations/" + currentResource.operationMap[httpMethod] + "#request-" + formatList[currentFormat];
        $("#sampleButton").attr('href',href);
        $("#sampleButton").attr('title',href);
        $("#sampleButton").switchClass('unclickableLink', 'clickableLink', 0);
    }
    else{
        $('#sampleButton').removeAttr('href');
        $('#sampleButton').removeAttr('title');
        $("#sampleButton").switchClass('clickableLink', 'unclickableLink', 0);
    }
}

function handleHttpResponse(httpRequest, ir){
    $("#status").text(httpRequest.status + "/" + httpRequest.statusText);
    if (httpRequest.status < 400) {
        $("#status").switchClass("requestStatusError", "requestStatusOK", 0);
    }
    else {
        $("#status").switchClass("requestStatusOK", "requestStatusError", 0);
    }
    ir.statusCode(httpRequest.status);
    ir.statusText(httpRequest.statusText);

    $("#responseHeader").val(httpRequest.getAllResponseHeaders());
    ir.responseHeader($("#responseHeader").val());

    var resTime = new Date();
    ir.responseTimeStr(dateToString(resTime));

    ir.duration(Math.ceil((resTime - reqTime) / 10) / 100);

    var format = null;
    var contentType = httpRequest.getResponseHeader("Content-Type");
    for (var key in contentTypePatterns) {
        if (contentType.match(contentTypePatterns[key])) {
            format = key;
            break;
        }
    }

    var rawResponse = httpRequest.responseText;

    if (format != null) {
        proxy.postResContText(
            rawResponse,
            format,
            function (formattedText) {
                $("#responseBody").val(formattedText);
            },
            function () {
                $("#responseBody").val(rawResponse);
            });
    }
    else {
        $("#responseBody").val(rawResponse);
    }

    ir.responseBody($("#responseBody").val());
    $("#statusBar").text(" Request " + ir.requestTimeStr() + "  Response " + ir.responseTimeStr() + "  Duration: " + ir.duration() + "s");
    invokeHistory.records.push(ir);

    var recordShow = ir.show(invokeHistory.records.length - 1);
    recordShow.prependTo($("#invokeList"));
    if (recordShow.width() > invokeHistory.maxWidth) {
        invokeHistory.maxWidth = recordShow.width();
        $("#invokeList").contents().width(invokeHistory.maxWidth);
    }
    else {
        recordShow.width(invokeHistory.maxWidth);
    }
}

$(document).ready(function () {
    initResourceTree();
    formatTabs = $("#requestBodyTabs").tabs();
    enableAllIntellisense();

    invokeHistory = { 'records': [], 'maxWidth': $('#history').width() };
    $("#clearHistory").click(function () {
        invokeHistory = { 'records': [], 'maxWidth': $('#history').width() };
        $("#invokeList").html("");
    });

    $("#uri").dblclick(function () {
        selectVariableInUriTextbox($("#uri"));
    });

    $("#uri").change(function () {
        validateUriOrRequestBody($("#uri"));
    });

    $("#requestBodyXml").change(function () {
        validateUriOrRequestBody($("#requestBodyXml"));
    });

    $("#requestBodyJson").change(function () {
        validateUriOrRequestBody($("#requestBodyJson"));
    });

    for (var i = 0; i < formatList.length; i++) {
        var requestBodyTextArea = $("#requestBody" + formatList[i]);
        requestBodyTextArea.change(function () {
            setContentLength($("#requestBody" + formatList[currentFormat]).val());
        });
    }

    $("#httpMethodTextBox").change(function () {
        var httpMethod = $("#httpMethodTextBox").val();
        var needHideRequestBody = false;
        for (var i = 0; i < httpMethodsNoBody.length; i++) {
            if (httpMethodsNoBody[i] == httpMethod) {
                needHideRequestBody = true;
                break;
            }
        }

        validateHttpMethod(httpMethod);
        validateUriOrRequestBody($("#uri"));
        
        $("#requestHeader").val("Accept:*/*");
        if (needHideRequestBody) {
            $("#requestBodyDiv").hide();
            $("#requestBodyTabs").hide();
            $("#responseBody").height(490);
            $("#requestBody" + formatList[currentFormat]).val("");
        }
        else {
            $("#requestBodyDiv").show();
            $("#requestBodyTabs").show();
            $("#responseBody").height(284);

            setContentType(formatList[currentFormat].toLowerCase());
            var requestBodyTextArea = $("#requestBody" + formatList[currentFormat]);
            setContentLength(requestBodyTextArea.val());
            
            validateUriOrRequestBody(requestBodyTextArea);
            updateSampleLink(httpMethod);
        }

        clearAutoCompleteResetListContext(1);   // no need to reset http-method textbox
    });

    $(".formatTabs").click(function (event) {
        var targetFormat = formatTabs.tabs('option', 'selected');

        if (targetFormat != currentFormat) {
            var currentTextbox = $("#requestBody" + formatList[currentFormat]);
            var targetTextBox = $("#requestBody" + formatList[targetFormat]);
            var httpMethod = $("#httpMethodTextBox").val();

            if (currentTextbox.val().length < 1) {
                currentFormat = targetFormat;
                updateSampleLink(httpMethod);
                enableFormattingButton(currentFormat);
                setContentType(formatList[currentFormat].toLowerCase());
                return;
            }

            if (targetFormat == 2 || currentFormat == 2) {        //raw format.            
                targetTextBox.val(currentTextbox.val());
                currentFormat = targetFormat;
            }
            else {
                var uriTemplate = $("#uriTemplate").val();
                var content = currentTextbox.val();

                var errorMessage = null;
                proxy.convertRequestBodyFormat(
                    uriTemplate,
                    httpMethod,
                    formatList[currentFormat].toLowerCase(),
                    formatList[targetFormat].toLowerCase(),
                    content,
                    function (convertResult) {
                        targetTextBox.val(convertResult);
                        currentTextbox.val("");
                        currentFormat = targetFormat;
                        setContentLength(targetTextBox.val());
                    },
                    function () {
                        errorMessage = ["The request body cannot be converted from "];
                        errorMessage.push(formatList[currentFormat]);
                        errorMessage.push(" to ");
                        errorMessage.push(formatList[targetFormat]);
                        errorMessage.push(". Do you want to paste it to ");
                        errorMessage.push(formatList[targetFormat] + " tab?");
                    });

                if (errorMessage != null) {
                    var answer = window.confirm(errorMessage.join(""));
                    if (answer) {
                        targetTextBox.val(content);
                        currentTextbox.val("");
                        currentFormat = targetFormat;
                        setContentLength(targetTextBox.val());
                    }
                    else {
                        formatTabs.tabs('select', currentFormat);
                    }
                }
            }

            enableFormattingButton(currentFormat);
            updateSampleLink(httpMethod);
            setContentType(formatList[currentFormat].toLowerCase());
        }
    });

    $("#formatButton").click(function () {
        if (currentFormat == 0 || currentFormat == 1) {
            //TODO: prevent the user from clicking other tabs.
            var currentTextBox = $("#requestBody" + formatList[currentFormat]);
            var rawRequestBody = currentTextBox.val();
            if (rawRequestBody.length > 0) {
                proxy.formatRequestBody(
                    $("#uriTemplate").val(),
                    $("#httpMethodTextBox").val(),
                    rawRequestBody,
                    formatList[currentFormat].toLowerCase(),
                    function (formattedText) {
                        currentTextBox.val(formattedText);
                    },
                    function () {
                        //TODO: show Formatting failed message.
                    });
            }
        }
    });

    $("#invokeButton").click(function () {

        clearLastResponse();

        var httpMethod = $("#httpMethodTextBox").val();
        var url = $("#uri").val();
        var requestHeader = $("#requestHeader").val();

        if (httpMethod.length == 0) {
            alert("HTTP Method should not be empty");
            $("#httpMethodTextBox").focus();
            return false;
        }

        if (url.length == 0) {
            alert("Url should not be empty");
            $("#uri").focus();
            return false;
        }

        var httpRequest = new XMLHttpRequest();
        try {
            httpRequest.open(httpMethod, encodeURI(url), false);
        }
        catch (e) {
            alert("Cannot send request. Check the security setting of your browser if you are sending request to a different domain.");
            return false;
        }
        var requestHeaders = requestHeader.split("\n");
        httpRequest.setRequestHeader("If-Modified-Since", new Date(0));
        for (var i = 0; i < requestHeaders.length; i++) {
            var headerPair = requestHeaders[i].split(":", 2);
            if (headerPair.length == 2) {
                httpRequest.setRequestHeader(headerPair[0], headerPair[1]);
            }
        }

        var ir = new $.invokeRecord();

        ir.resource(currentResource);
        ir.httpMethod(httpMethod);
        ir.uri(url);
        ir.uriTemplate($("#uriTemplate").val());
        ir.requestHeader(requestHeader);
        ir.requestBody($("#requestBody" + formatList[currentFormat]).val());
        ir.format(currentFormat);

        if ($.browser.mozilla) {  //since firefox 3.5/3.6 will not trigger XMLHttpRequest.onreadystatechange().
            httpRequest.onload = httpRequest.onerror = httpRequest.onabort = function(){
            handleHttpResponse(httpRequest, ir);
           };
        }
        else{
            httpRequest.onreadystatechange = function () {
                switch (this.readyState) {
                    case 4:
                        handleHttpResponse(httpRequest, ir);
                        break;
                    default:
                        break;
                }
            }
        }

        httpRequest.ontimeout = function () {
            $("#status").text("Request timed out.");
        }

        reqTime = new Date();
        ir.requestTimeStr(dateToString(reqTime));
        httpRequest.send($("#requestBody" + formatList[currentFormat]).val());

        return false;
    });

    $("#container").show();
});
