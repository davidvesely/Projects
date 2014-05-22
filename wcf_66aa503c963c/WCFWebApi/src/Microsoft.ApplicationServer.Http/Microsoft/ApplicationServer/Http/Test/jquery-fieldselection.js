/* Copyright (c) Microsoft 
* Note:  This file is a modified version of the jQuery FieldSelection plug-in from
* http://plugins.jquery.com/files/jquery-fieldselection.js_0.txt under the license that is referred to below. 
* That license and the other notices below are provided for informational purposes only and are not the license 
* terms under which Microsoft distributes the files.  Microsoft grants you the right to use this file for the sole 
* purpose of either: (i) interacting through your browser with the Microsoft web site hosting the files, subject to 
* that web site’s terms of use; or (ii) using the file in conjunction with the Microsoft product with which it was 
* distributed subject to that product’s End User License Agreement. Unless applicable law gives you more rights, 
* Microsoft reserves all other rights to the files not expressly granted by Microsoft, whether by implication, 
* estoppel or otherwise.
 
* Copyright (c) 2006, Alex Brem
* All rights reserved.
 
* Redistribution and use of this software in source and binary forms, with or without modification, are
* permitted provided that the following conditions are met:
 
* * Redistributions of source code must retain the above copyright notice, this list of conditions and the
* following disclaimer.
 
* * Redistributions in binary form must reproduce the above
* copyright notice, this list of conditions and the
* following disclaimer in the documentation and/or other
* materials provided with the distribution.
 
* * Neither my name nor the names of its contributors may
* be used to endorse or promote products derived from
* this software without specific prior written permission.
 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
* WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
* PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
* ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
* LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
* TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
* ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. */

(function () {

    var fieldSelection = {

        getSelection: function () {

            var e = this.jquery ? this[0] : this;

            return (

            /* mozilla / dom 3.0 */
				('selectionStart' in e && function () {
				    var l = e.selectionEnd - e.selectionStart;
				    return { start: e.selectionStart, end: e.selectionEnd, length: l, text: e.value.substr(e.selectionStart, l) };
				}) ||

            /* explorer */
				(document.selection && function () {
				    e.focus();

				    var r = document.selection.createRange();
				    if (r == null) {
				        return { start: 0, end: e.value.length, length: 0 }
				    }

				    var r1 = e.createTextRange();
				    var r2 = r1.duplicate();
				    r2.moveToBookmark(r.getBookmark()); // r2 is the selection in textbox
				    r1.setEndPoint('EndToStart', r2);   // r1 is from 0 to selection start

				    // return { start: r1.text.length, end: r1.text.length + r.text.length, length: r.text.length, text: r.text };
				    // the issues with the above result:
				    // IE8 bug -- when the selection is right after \r\n, r1.text does not count ending \r\n

				    var r1len = r1.text.length;
				    if (r1len < e.value.length) {
				        var r3 = r1.duplicate();
				        r3.collapse(true);
				        // note: move counts \r\n as 1 char
				        r3.moveEnd("character", r1.text.replace(/\r/g, "").length); // r3 is from 0 to r1.text.length

				        // end of r3 should be the same as end of r1, otherwise there are \r\n in between
				        while (r1len < e.value.length && r3.compareEndPoints("EndToEnd", r1) < 0) {
				            r1len += 2;
				            r3.moveEnd("character", 1);
				        }
				    }

				    // return { start: r1len, end: r1len + r.text.length, length: r.text.length, text: r.text };
				    // the issue with the above result:
				    // jQuery .val() replaces \r\n with \n, so start and end do not work with jQuery .val()
				    var r1text = e.value.substr(0, r1len).replace(/\r\n/g, "\n");
				    var rtext = e.value.substr(r1len, r.text.length).replace(/\r\n/g, "\n");
				    return { start: r1text.length, end: r1text.length + rtext.length, length: rtext.length, text: rtext };
				}) ||

            /* browser not supported */
				function () {
				    return { start: 0, end: e.value.length, length: 0 };
				}

			)();

        },

        replaceSelection: function (text) {

            var e = this.jquery ? this[0] : this;
            var THIS = this;

            if (!text) {
                text = "";
            }

            return (

            /* mozilla / dom 3.0 */
				('selectionStart' in e && function () {
				    var pos = e.selectionStart + text.length;
				    e.value = e.value.substr(0, e.selectionStart) + text + e.value.substr(e.selectionEnd, e.value.length);
				    THIS.setSelection({ start: pos, end: pos });
				    return this;
				}) ||

            /* explorer */
				(document.selection && function () {
				    e.focus();

				    var r = document.selection.createRange();
				    if (r == null) {
				        return;
				    }

				    var selection = THIS.getSelection();
				    var pos = selection.start + text.length;
				    r.text = text;
				    THIS.setSelection({ start: pos, end: pos });
				    return this;
				}) ||

            /* browser not supported */
				function () {
				    e.value += text;
				    return this;
				}

			)();

        },

        setSelection: function (selection) {
			if (this.get(0).setSelectionRange) {       
				this.get(0).setSelectionRange(selection.start, selection.end);    
			} else if (this.get(0).createTextRange) {
				var range = this.get(0).createTextRange();      
				range.collapse(true);       
				range.moveEnd('character', selection.end);       
				range.moveStart('character', selection.start);      
				range.select();     
			}
        }
    };

    jQuery.each(fieldSelection, function (i) { jQuery.fn[i] = this; });

})();
