/*
Telerik Kendo Grid have only limitted support of MVVM and not support some importent features.
For more details see:
http://www.telerik.com/forums/grid-mvvm-data-columns-can-we-bind-this-to-a-source
This module is wrapper for kendo grid, and allow to manage Grid from ViewModel without manipulation with HTML & JQuery and keep ViewModel clear.
*/

/* Dependencies
 <script src="//da7xgjtj801h2.cloudfront.net/2015.1.429/js/kendo.all.min.js"></script>
*/

var app = app || {};

app.KendoGrid = (function (global, $, kendo) {
    function Grid(gridId) {
        this.$reportContent = $("#" + gridId);
    }

    Grid.prototype = {
        ShowGrid: function (gridConiguration, gridData) {
            var
              fields = {};

            gridConiguration.columns.forEach(function (element) {
                fields[element.field] = { type: "string" };
            });

            this.$reportContent.empty();
            this.$reportContent.kendoGrid();

            this.$reportContent.kendoGrid({
                scrollable: true,
                sortable: true,
                pageable: true,
                columnMenu: true,
                filterable: true,
                resizable: true,
                selectable: "row",
                height: 728,
                dataSource: {
                    data: gridData,
                    pageSize: 10,
                    schema: {
                        model: {
                            fields: fields
                        }
                    }
                },
                columns: gridConiguration.columns
            });
        },

        ClearGrid: function () {
            this.$reportContent.kendoGrid();
        }
    }

    return Grid;
})(window, jQuery, kendo);
