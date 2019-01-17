/*
* selectAllRows.js - http://beckelman.net
*
* Copyright (c) 2008 Bill Beckelman
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* 2008-11-18
* Version .01
*/

(function($) {
    $.fn.selectAllRows = function(callerSettings) {
        var settings;
        var headerCheckbox;
        var columnCheckboxes;

        settings = $.extend({
                column: 'first',
                selectTip: 'Click to Select All',
                unselectTip: 'Click to Un-Select All'
            }, callerSettings || { });

        if (isNaN(settings.column)) {
            headerCheckbox = $("thead tr th:" + settings.column + "-child input:checkbox", this);
            columnCheckboxes = $("tbody tr td:" + settings.column + "-child input:checkbox", this);
        } else {
            headerCheckbox = $("thead tr th:nth-child(" + settings.column + ") input:checkbox", this);
            columnCheckboxes = $("tbody tr td:nth-child(" + settings.column + ") input:checkbox", this);
        }

        headerCheckbox.attr("title", settings.selectTip);

        headerCheckbox.click(function() {
            var checkedStatus = this.checked;

            columnCheckboxes.each(function() {
                this.checked = checkedStatus;
            });

            if (checkedStatus == true) {
                $(this).attr("title", settings.unselectTip);
            } else {
                $(this).attr("title", settings.selectTip);
            }
        });

        return $(this);
    };
})(jQuery);