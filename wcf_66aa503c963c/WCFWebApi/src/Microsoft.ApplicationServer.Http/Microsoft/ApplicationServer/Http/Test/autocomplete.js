if (jQuery) {
    $.extend($.fn, {
        enableAutoComplete: function (debug) {

            // #region private fields and methods of "autoComplete" object

            var _textbox = $(this);

            var _div = !debug
                ? $("<div style='overflow: scroll; position: absolute; visibility: hidden;' />")
                : $("<div style='overflow: scroll;' />");
            _textbox.after(_div);
            _div.width(_textbox.width());
            _div.height(_textbox.height());

            if (!debug) {
                _div.offset({ top: _textbox.offset().top, left: _textbox.offset().left });
            }

            var _span1 = $("<span style='position: relative;' ></span>");
            _span1.css("font", _textbox.css("font"));
            _span1.appendTo(_div);

            var _span2 = $("<span style='position: relative;' >.</span>");
            _span2.appendTo(_div);

            var _select = $("<select style='display: none; position: absolute; z-index: 100;' />");
            _select.attr("id", _textbox.attr("id") + "Select");
            _select.appendTo(_textbox.parent().parent());

            var _context = null;
            var _items = [];
            var _shownItems = [];

            var _regexContextId = /^(.*\W)?/;

            var _filteringOn1stChar = false;
            var _comboboxMode = false;  // if true, focus triggers dropdown, and replacement is for whole textbox

            var _suppressKeyUp = false;
            var _hideTimeoutId = null;

            var _encodeToHtml = function (text) {
                if (!text || text == "") {
                    return "";
                }

                return text.replace(/\&/g, "&amp;")
                    .replace(/\</g, "&lt;")
                    .replace(/\>/g, "&gt;")
                    .replace(/\n/g, "<br/>")
                    .replace(/\s/g, "&nbsp;");
            };

            var _getValueFromItem = function (item) {
                var buf = item.split("\n");
                return buf.length >= 2 ? buf[1] : buf[0];
            };

            var _getDisplayNameFromItem = function (item) {
                var buf = item.split("\n");
                return buf[0];
            };

            var _getReplaceStartPosFromItem = function (item) {
                var buf = item.split("\n");
                return buf.length >= 3 && buf[2] != "" ? buf[2] - 0 : undefined;
            };

            var _getFinalCaretPosFromItem = function (item) {
                var buf = item.split("\n");
                return buf.length >= 4 && buf[3] != "" ? buf[3] - 0 : undefined;
            };

            var _getApplicableFromItem = function (item) {
                var buf = item.split("\n");
                return buf.length >= 5 ? (buf[4] == "true" || buf[4] == "") : true;
            };

            // return most matching item index
            var _rebuildOptions = function (word) {
                var mostMatchingIndex = -1;
                var mostMatchingLevel = -1;

                if (!word) {
                    // show a full dropdown list
                    _shownItems = _items;
                }
                else {
                    // filter the dropdown list by the word
                    word = word.toLowerCase();

                    var newShownItems = [];
                    for (var i = 0; i < _items.length; i++) {
                        var displayName = _getDisplayNameFromItem(_items[i]);
                        var pos = displayName.toLowerCase().indexOf(word);

                        if (pos < 0) {
                            // if no match in display-name, use value to match
                            // this is useful for json enum, where display-name is string and value is number
                            // so that the user can type string or number to find autcomplete matches
                            displayName = _getValueFromItem(_items[i]);
                            pos = displayName.toLowerCase().indexOf(word);
                        }

                        if (pos < 0 && (_filteringOn1stChar || word.length > 1)) {
                            continue;
                        }

                        // item at least contains the word, or word has only 1 char
                        newShownItems.push(_items[i]);

                        var matchingLevel = -1;
                        if (pos >= 0) {
                            var matchingLevel = 0; // contains
                            if (pos == 0) {
                                matchingLevel = 1; // begins with

                                if (displayName.length == word.length) {
                                    matchingLevel = 2; // case-insensitive equals

                                    if (displayName == word) {
                                        matchingLevel = 3; // equals
                                    }
                                }
                            }
                        }

                        if (matchingLevel > mostMatchingLevel) {
                            mostMatchingIndex = newShownItems.length - 1;
                            mostMatchingLevel = matchingLevel;
                        }
                    }

                    if (newShownItems.length < 1) {
                        // no matching item
                        // keep the dropdown list as is, in case the user mistyped 
                        return -1;
                    }

                    _shownItems = newShownItems;
                }

                // rebuild the dropdown list
                _select.html("");
                for (var i = 0; i < _shownItems.length; i++) {
                    var option = $("<option />");
                    option.text(_getDisplayNameFromItem(_shownItems[i]));
                    option.attr("title", _getValueFromItem(_shownItems[i]));
                    option.appendTo(_select);

                    if (!_getApplicableFromItem(_shownItems[i])) {
                        option.addClass("notApplicableItem");
                    }
                }

                // set the dropdown list height
                var count = _shownItems.length;
                if (count < 2) {
                    _select.attr("size", 2);
                }
                else if (count < 10) {
                    _select.attr("size", count);
                }
                else {
                    _select.attr("size", 10);
                }

                return mostMatchingIndex;
            };

            // #endregion private fields and methods of "autoComplete" object

            // #region public fields and methods of "autoComplete" object

            var autoComplete = {

                // filter + select
                filterItemsByWord: function (word) {
                    var mostMatchingIndex = _rebuildOptions(word);
                    _select.attr("selectedIndex", mostMatchingIndex);
                },

                getContext: function () {
                    var context = {
                        curPos: -1,
                        fullText: _textbox.val(),
                        id: "",
                        lastWord: "",
                        selection: _textbox.getSelection()
                    };

                    context.curPos = context.selection.start;

                    var text = context.fullText.substr(0, context.curPos);
                    if (_comboboxMode) {
                        context.id = "";
                        context.lastWord = text;
                    }
                    else {
                        // e.g. <abc|def
                        // context.id = "<"
                        // context.lastWord = "abc"
                        var results = text.match(_regexContextId);
                        if (results) {
                            context.id = results[0];
                            context.lastWord = text.substr(context.id.length);
                        }
                    }

                    return context;
                },

                clearContext: function () {
                    _context = null;
                },

                comboboxMode: function (value) {
                    if (value || value == false) {
                        // setter
                        _comboboxMode = value;
                    }
                    else {
                        // getter
                        return _comboboxMode;
                    }
                },

                filteringOn1stChar: function (value) {
                    if (value || value == false) {
                        // setter
                        _filteringOn1stChar = value;
                    }
                    else {
                        // getter
                        return _filteringOn1stChar;
                    }
                },

                hide: function () {
                    _select.hide();
                },

                onBlur: function (event) {
                    // note: do not immediately hide, otherwise fail to double click it
                    var THIS = this;
                    _hideTimeoutId = setTimeout(function () {
                        THIS.hide();
                    }, 100);
                },

                onFocus: function (event) {
                    if (!_comboboxMode) {
                        return;
                    }

                    _textbox.setSelection({
                        start: 0,
                        end: _textbox.val().length
                    });

                    return _textbox.keyup();
                },

                onKeyDown: function (event) {
                    if (event.keyCode == 74 && event.ctrlKey) {
                        // ctrl-j
                        event.preventDefault();
                        return;
                    }

                    if (_select.css("display") == "none") {
                        return;
                    }

                    // handle special chars

                    var context = this.getContext();
                    var selectedIndex = _select.attr("selectedIndex");

                    switch (event.keyCode) {
                        case 9:     // tab            
                        case 13:    // return
                            if (selectedIndex >= 0) {
                                // replace the word with the selected item
                                var item = _shownItems[selectedIndex];
                                this.replaceLastWord(context, item);

                                event.preventDefault();
                                this.hide();
                                _suppressKeyUp = true;
                            }
                            break;

                        case 27:    // escape
                            event.preventDefault();
                            this.hide();
                            _suppressKeyUp = true;
                            break;

                        case 32:    // space
                            if (selectedIndex >= 0) {
                                // replace the word with the selected item
                                var item = _shownItems[selectedIndex];
                                this.replaceLastWord(context, item);

                                this.hide();
                                _suppressKeyUp = true;
                            }
                            break;

                        case 33:    // pageup
                            selectedIndex -= 10;
                            if (selectedIndex < 0) {
                                selectedIndex = 0;
                            }
                            _select.attr("selectedIndex", selectedIndex);

                            event.preventDefault();
                            _suppressKeyUp = true;
                            break;

                        case 34:    // pagedown
                            selectedIndex += 10;
                            if (selectedIndex > _shownItems.length - 1) {
                                selectedIndex = _shownItems.length - 1;
                            }
                            _select.attr("selectedIndex", selectedIndex);

                            event.preventDefault();
                            _suppressKeyUp = true;
                            break;

                        case 38:    // up
                            selectedIndex--;
                            if (selectedIndex < 0) {
                                selectedIndex = _shownItems.length - 1;
                            }
                            _select.attr("selectedIndex", selectedIndex);

                            event.preventDefault();
                            _suppressKeyUp = true;
                            break;

                        case 40:    // down
                            selectedIndex++;
                            if (selectedIndex > _shownItems.length - 1) {
                                selectedIndex = 0;
                            }
                            _select.attr("selectedIndex", selectedIndex);

                            event.preventDefault();
                            _suppressKeyUp = true;
                            break;
                    }
                },

                onKeyUp: function (event, asyncGetDropDownList) {
                    if (_suppressKeyUp) {
                        _suppressKeyUp = false;
                        return;
                    }

                    // handle special chars
                    switch (event.keyCode) {
                        case 13:    // return
                            this.hide();
                            return;

                        case 35:    // end
                        case 36:    // home
                            this.hide();
                            return;

                        case 37:    // left
                        case 39:    // right
                            if (_select.css("display") == "none") {
                                return;
                            }
                            break;

                        case 33:    // pageup
                        case 34:    // pagedown
                        case 38:    // up
                        case 40:    // down
                            // handled at onKeyDown
                            return;

                        case 9:     //tab
                        case 16:    //shift
                        case 17:    //ctrl
                        case 18:    //alt
                        case 19:    //Pause Break
                        case 20:    //caps lock
                        case 44:    //PrScrn SysRq
                        case 45:    //insert
                        case 91:    //left win
                        case 92:    //right win
                        case 93:    //popup menu
                        case 112:   //F1
                        case 113:   //F2
                        case 114:   //F3
                        case 115:   //F4
                        case 116:   //F5
                        case 117:   //F6
                        case 118:   //F7
                        case 119:   //F8
                        case 120:   //F9
                        case 121:   //F10
                        case 122:   //F11
                        case 123:   //F12
                        case 144:   //num lock
                        case 154:   //scroll lock
                            // be filtered out.
                            return;

                        case 74:    // j
                            if (event.ctrlKey) {
                                // ctrl-j
                                _context = null;    // force autocomplete without cache
                            }
                            break;
                    }

                    // handle normal chars

                    var context = this.getContext();

                    if (_context && context.id == _context.id) {
                        if (_items.length > 0) {
                            // reuse existing dropdown list
                            // just do local filtering
                            this.filterItemsByWord(context.lastWord);
                            this.show(context);
                        }
                        // otherwise nop, expecting another response
                    }
                    else {
                        // need to async retrieve dropdown items
                        _context = context;
                        _items = [];

                        var THIS = this;
                        var callback = function (data) {
                            if (!data || !data.autoCompleteList || data.autoCompleteList.length < 1) {
                                return;
                            }

                            if (_context == null || context.id != _context.id) {
                                // this response is out of date
                                return;
                            }

                            THIS._debugContext = context; // for debugging

                            THIS.setItems(data.autoCompleteList);
                            THIS.filterItemsByWord(_context.lastWord);
                            THIS.show(_context);
                        };

                        this.hide();
                        asyncGetDropDownList(context.fullText, context.curPos, callback);
                    }
                },

                onMouseClick: function (event) {
                    if (!_comboboxMode) {
                        this.hide();
                        return;
                    }

                    return _textbox.keyup();
                },

                regexContextId: function (value) {
                    if (value) {
                        // setter
                        _regexContextId = value;
                    }
                    else {
                        // getter
                        return _regexContextId;
                    }
                },

                replaceLastWord: function (context, item) {
                    if (!_comboboxMode) {
                        var replaceStartPos = _getReplaceStartPosFromItem(item);
                        if (replaceStartPos == undefined) {
                            replaceStartPos = context.curPos - context.lastWord.length;
                        }

                        _textbox.setSelection({
                            start: replaceStartPos,
                            end: context.curPos
                        });
                    }
                    else {
                        _textbox.setSelection({
                            start: 0,
                            end: context.fullText.length
                        });
                    }

                    _textbox.replaceSelection(_getValueFromItem(item));

                    if (_comboboxMode) {
                        _textbox.change();
                    }
                    else {
                        var pos = _getFinalCaretPosFromItem(item);
                        if (pos != undefined) {
                            pos = _textbox.getSelection().end + pos;    // relative to absolute
                            _textbox.setSelection({ start: pos, end: pos });

                            // force another autocomplete
                            _context = null;
                            _textbox.keyup();
                        }
                    }
                },

                // set + sort
                setItems: function (items) {
                    _items = items.slice(0);
                    _items.sort();

                    _rebuildOptions();
                },

                show: function (context) {
                    if (_select.text() == '') {
                        return;
                    }

                    var text = context.fullText.substr(0, context.curPos);
                    var html = _encodeToHtml(text);

                    _div.width(_textbox.width());
                    _div.height(_textbox.height());

                    _span1.html(html);
                    if (_textbox.css("font")) {
                        _span1.css("font", _textbox.css("font"));
                    }
                    else {
                        if (_textbox.css("font-size")) {
                            _span1.css("font-size", _textbox.css("font-size"));
                        }

                        if (_textbox.css("font-family")) {
                            _span1.css("font-family", _textbox.css("font-family"));
                        }

                        if (_textbox.css("font-style")) {
                            _span1.css("font-style", _textbox.css("font-style"));
                        }

                        if (_textbox.css("font-weight")) {
                            _span1.css("font-weight", _textbox.css("font-weight"));
                        }
                    }

                    if (!debug) {
                        _div.offset({ top: _textbox.offset().top, left: _textbox.offset().left });
                    }

                    var left = _span2.offset().left - _textbox.scrollLeft();   // handle when the string is too long
                    var top = _span2.offset().top + 20 - _textbox.scrollTop();

                    _select.show();
                    _select.offset({ top: top, left: left });
                }
            };

            // #endregion public fields and methods of "autoComplete" object

            _select.blur(function (event) {
                autoComplete.onBlur(event);
            });

            _select.dblclick(function (event) {
                var context = autoComplete.getContext();
                var selectedIndex = _select.attr("selectedIndex");

                if (selectedIndex >= 0) {
                    // replace the word with the selected item
                    var item = _shownItems[selectedIndex];
                    autoComplete.replaceLastWord(context, item);

                    autoComplete.hide();
                }
            });

            _select.focus(function (event) {
                if (_hideTimeoutId) {
                    clearTimeout(_hideTimeoutId);
                    _hideTimeoutId = null;
                }
            });

            _select.keydown(function (event) {
                switch (event.keyCode) {
                    case 9:     // tab
                    case 13:    // return
                        _select.dblclick();

                        event.preventDefault();
                        break;
                }

            });

            return autoComplete;
        }
    });
}
