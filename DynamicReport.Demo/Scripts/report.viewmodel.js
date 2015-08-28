(function (global, kendo, app) {

    var
        util = new app.ApiRequest(),
        kendoMvvmHelper = app.kendoMvvmHelper,
        reportGrid;

    var ReportModel = new kendo.data.ObservableObject({
        reportType: "StudentReport",
        filters: [],
        columns: [],

        addColumnToTeport: function (columnDefenition) {
            if (kendoMvvmHelper.findIndex(this.columns, "title", columnDefenition.title) > -1) {
                alert("Column is already added");
            } else {
                this.columns.push(columnDefenition);
                this.trigger("change", { field: "columns" });
            }
        },

        removeColumnFromReport: function (columnDefenition) {
            var index = kendoMvvmHelper.findIndex(this.columns, "title", columnDefenition.title);
            if (index > -1) {
                var reportColumn = this.columns[index];
                this.columns.splice(index, 1);
                this.trigger("change", { field: "columns" });

                //if columns do not contains current column defenition we will remove all filters releated to this column.
                if (kendoMvvmHelper.findIndex(this.columns, "title", columnDefenition.title) !== -1) {
                    return;
                }

                while (kendoMvvmHelper.findIndex(this.filters, "reportColumnTitle", reportColumn.title) > -1) {
                    this.filters.splice(kendoMvvmHelper.findIndex(this.filters, "reportColumnTitle", reportColumn.title), 1);
                }
            }
        },

        addFilterToReport: function (filterDefenition) {
            var validationResult = this._validateProposedFilter(filterDefenition);
            if (validationResult) {
                alert(validationResult);
                return;
            }

            filterDefenition.hashCode = this._hashCode(filterDefenition.reportColumnTitle + filterDefenition.filterTitle + filterDefenition.filterValue);

            if (kendoMvvmHelper.findIndex(this.filters, "hashCode", filterDefenition.hashCode) > -1) {
                alert("Filter is already added");
            } else {
                this.filters.push(filterDefenition);
            }
        },

        removeFilterFromReport: function (hashCode) {
            var index = kendoMvvmHelper.findIndex(this.filters, "hashCode", hashCode);
            if (index > -1) {
                this.filters.splice(index, 1);
            }
        },

        _validateProposedFilter: function (filterDefenition) {
            if (filterDefenition.reportColumnTitle === "")
                return "Please Select Filter column";

            if (!filterDefenition.filterType)
                return "Please Select Filter operator";

            if (filterDefenition.filterValue === "")
                return "Please Enter Filter text";

            var columnDefenition = kendoMvvmHelper.findElement(this.columns, "title", filterDefenition.reportColumnTitle);

            if (columnDefenition == null)
                return "You can not add a filter to a column that is not in the report";

            return null;
        },

        _hashCode: function (str) {
            var hash = 0;
            if (str.length == 0) return hash;
            for (var i = 0; i < str.length; i++) {
                var charCode = str.charCodeAt(i);
                hash = ((hash << 5) - hash) + charCode;
                hash = hash & hash; // Convert to 32bit integer
            }
            return hash.toString();
        }
    });

    var reportsViewModel = kendo.observable({
        reportModel: ReportModel,
        reportFilterOperators: [],
        reportColumnsAvailableForReportFocus: [],
        columnSelectedField: "",
        filterSelectedField: "",
        filterSelectedOperator: null,
        filterValue: "",

        noColumnsSelectedForReport: function () {
            return this.reportModel.columns.length === 0;
        },
        noFiltersSelectedForReport: function () {
            return this.reportModel.filters.length === 0;
        },
        
        addReportColumn: function (e) {
            this._addColumnToReport(this.columnSelectedField);
        },

        removeReportColumn: function (e) {
            this._removeColumnFromReport(e.currentTarget.getAttribute("data-report-column"));
        },

        addReportFilter: function () {
            var filterDefenition = {
                reportColumnTitle: this.filterSelectedField,
                filterType: this.filterSelectedOperator,
                filterValue: this.filterValue
            };

            this._addFilterToReport(filterDefenition);
        },

        removeReportFilter: function (e) {
            this.reportModel.removeFilterFromReport(e.currentTarget.getAttribute("data-report-hashCode"));
            this._trigerChangeReportModelEvents();
        },

        buildReport: function () {
            this._buildReport();
        },

        _newReport: function () {
            this._clearWizardData();

            this._buildReport();
        },

        _addColumnToReport: function (columnTitle) {
            var columnDefenition = kendoMvvmHelper.findElement(this.reportColumnsAvailableForReportFocus, "title", columnTitle);

            if (columnDefenition) {
                this.reportModel.addColumnToTeport(columnDefenition);
            } else {
                alert("Column defenition was not found. Title: " + columnTitle);
            }

            this._trigerChangeReportModelEvents();
        },

        _removeColumnFromReport: function (columnTitle) {
            var columnDefenition = kendoMvvmHelper.findElement(this.reportColumnsAvailableForReportFocus, "title", columnTitle);

            if (columnDefenition) {
                this.reportModel.removeColumnFromReport(columnDefenition);
            }

            this._trigerChangeReportModelEvents();

            if (this.filterSelectedField === columnDefenition.title) {
                this.set("filterSelectedField", "");
            }
        },

        _addFilterToReport: function (filterDefenition) {
            var filterOperatorDefenition = kendoMvvmHelper.findElement(this.reportFilterOperators, "filterType", filterDefenition.filterType);

            if (filterOperatorDefenition) {
                filterDefenition.filterTitle = filterOperatorDefenition.filterTitle;
                this.reportModel.addFilterToReport(filterDefenition);
                this._trigerChangeReportModelEvents();
            } else {
                alert("Filter defenition was not found");
            }
        },

        _getReportModelForWebApi: function () {
            var reportFilters = this.reportModel.filters.map(function (element) {
                return {
                    reportColumnTitle: element.reportColumnTitle,
                    filterValue: element.filterValue,
                    filterType: element.filterType
                }
            });

            return {
                columns: this.reportModel.columns.map(function (element) { return element.title; }),
                filters: reportFilters
            };
        },

        _buildReport: function () {
            var postData = this._getReportModelForWebApi();

            if (postData.columns.length === 0) {
                alert("Please select at least one column for report.");
                return;
            }

            reportGrid.ClearGrid();

            var that = this;
            util.post('/api/reports/' + that.reportModel.reportType, postData, "Build report").done(function (data) {
                var gridConiguration = {
                    columns: []
                };

                that.reportModel.columns.forEach(function (element) {
                    gridConiguration.columns.push(that._getKendoGridColumn(element));
                });

                reportGrid.ShowGrid(gridConiguration, data);
            });
        },

        _getKendoGridColumn: function (element) {
            var column = {
                field: element.alias,
                title: element.title
            };

            return column;
        },

        _trigerChangeReportModelEvents: function () {
            this.trigger("change", { field: "noColumnsSelectedForReport" });
            this.trigger("change", { field: "noFiltersSelectedForReport" });
            this.trigger("change", { field: "reportModel.columns" });
            this.trigger("change", { field: "reportModel.filters" });
        },

        _clearData: function () {
            //Clear Report.. 
            this.set("reportModel.reportType", "");
            this.set("reportModel.columns", []);
            this.set("reportModel.filters", []);
            reportGrid.ClearGrid();
            
            this.set("columnSelectedField", "");
            this.set("filterSelectedField", "");
            this.set("filterSelectedOperator", "");
            this.set("filterValue", "");

            //Refresh UI
            this._trigerChangeReportModelEvents();
        },

        LoadViewModel: function () {
            reportGrid = new app.KendoGrid("reportContent");

            var that = this;
            util.get('/api/reports/filters', "Get report filters").done(function (data) {
                data.forEach(function (element) {
                    that.reportFilterOperators.push(element);
                });

                that.trigger("change", { field: "reportFilterOperators" });
            });
            

            util.get('/api/reports/' + this.reportModel.reportType + "/columns", "").done(function (data) {
                data.forEach(function (element) {
                    that.reportColumnsAvailableForReportFocus.push(element);
                });

                that.trigger("change", { field: "reportColumnsAvailableForReportFocus" });
            });
        }
    });

    app.reportsViewModel = reportsViewModel;
})(window, kendo, app);