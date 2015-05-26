(function (global, kendo, app) {

    var
        util = new app.ApiRequest(),
        reportGrid = new app.KendoGrid("reportContent"),
        kendoMvvmHelper = app.kendoMvvmHelper;

    var ReportModel = new kendo.data.ObservableObject({
        reportType: "StudentReport",
        filters: [],
        columns: [],

        addColumnToTeport: function (fieldDefenition) {
            if (kendoMvvmHelper.findIndex(this.columns, 'title', fieldDefenition.title) > -1) {
                alert("Field is already added");
            } else {
                this.columns.push(fieldDefenition);
                this.trigger("change", { field: "columns" });
            }
        },

        removeColumnFromReport: function (fieldDefenition) {
            var index = kendoMvvmHelper.findIndex(this.columns, 'title', fieldDefenition.title);
            if (index > -1) {
                var reportfield = this.columns[index];
                this.columns.splice(index, 1);
                this.trigger("change", { field: "columns" });

                //if columns do not contains current field defenition we will remove all filters releated to this field.
                if (kendoMvvmHelper.findIndex(columns, 'title', fieldDefenition.title) !== -1) {
                    return;
                }

                var filterIndex = kendoMvvmHelper.findIndex(this.filters, 'reportFieldTitle', reportfield.title);
                while (filterIndex > -1) {
                    this.filters.splice(filterIndex, 1);
                    filterIndex = kendoMvvmHelper.findIndex(this.filters, 'reportFieldTitle', reportfield.title);
                }
            }
        },

        addFilterToReport: function (filterDefenition) {
            var validationResult = this._validateProposedFilter(filterDefenition);
            if (validationResult) {
                alert(validationResult);
                return;
            }

            filterDefenition.hashCode = this._hashCode(filterDefenition.reportFieldTitle + filterDefenition.filterTitle + filterDefenition.filterValue);

            if (kendoMvvmHelper.findIndex(this.filters, 'hashCode', filterDefenition.hashCode) > -1) {
                alert("Filter is already added");
            } else {
                this.filters.push(filterDefenition);
            }
        },

        removeFilterFromReport: function (hashCode) {
            var index = kendoMvvmHelper.findIndex(this.filters, 'hashCode', hashCode);
            if (index > -1) {
                this.filters.splice(index, 1);
            }
        },

        _validateProposedFilter: function (filterDefenition) {
            if (filterDefenition.reportFieldTitle === "")
                return "Please Select Filter field";

            if (!filterDefenition.filterType)
                return "Please Select Filter operator";

            if (filterDefenition.filterValue === '')
                return "Please Enter Filter text";

            var columnDefenition = kendoMvvmHelper.findElement(this.columns, 'title', filterDefenition.reportFieldTitle);

            if (columnDefenition == null)
                return "You can not add a filter to a column that is not in the report";

            var dateFormat1 = /^(0[1-9]|1[012])[\/](0[1-9]|[12][0-9]|3[01])[\/]\d{4}$/;
            var dateFormat2 = /^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[ ](0[1-9]|[12][0-9]|3[01])[,][ ](19|20)\d\d$/;

            if (columnDefenition.type === 'Date' &&
                (filterDefenition.filterType !== 'Include' && filterDefenition.filterType !== 'NotInclude') &&
                !(dateFormat1.test(filterDefenition.filterValue) || dateFormat2.test(filterDefenition.filterValue))) {
                return "Please enter valid date (Mmm dd, yyyy) or (mm/dd/yyyy). Example: Jul 02, 2013 ";
            }

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
            this._addFieldToReport(this.columnSelectedField);
        },

        removeReportColumn: function (e) {
            this._removeFieldFromReport(e.currentTarget.getAttribute("data-report-field"));
        },

        addReportFilter: function () {
            var filterDefenition = {
                reportFieldTitle: this.filterSelectedField,
                filterType: this.filterSelectedOperator,
                filterValue: this.filterValue
            };

            this._addFilterToReport(filterDefenition);
        },

        removeReportFilter: function (e) {
            this.reportModel.removeFilterFromReport(e.currentTarget.getAttribute('data-report-hashCode'));
            this._trigerChangeReportModelEvents();
        },

        buildReport: function () {
            this._buildReport();
        },

        _newReport: function () {
            this._clearWizardData();

            this._buildReport();
            //var that = this;
            //util.get("/api/reports/fields", "Get report default fields").done(function (reportColumns) {
            //    reportColumns.forEach(function (column) {
            //        that._addFieldToReport(column.title);
            //    });

            //    that._buildReport();
            //});
        },

        _addFieldToReport: function (fieldTitle) {
            var fieldDefenition = kendoMvvmHelper.findElement(this.reportColumnsAvailableForReportFocus, 'title', fieldTitle);

            if (fieldDefenition) {
                this.reportModel.addColumnToTeport(fieldDefenition);
            } else {
                alert("Column defenition was not found. Title: " + fieldTitle);
            }

            this._trigerChangeReportModelEvents();
        },

        _removeFieldFromReport: function (fieldTitle) {
            var columnDefenition = kendoMvvmHelper.findElement(this.reportColumnsAvailableForReportFocus, 'title', fieldTitle);

            if (columnDefenition) {
                this.reportModel.removeColumnFromReport(columnDefenition);
            }

            this._trigerChangeReportModelEvents();

            if (this.filterSelectedField === columnDefenition.title) {
                this.set("filterSelectedField", '');
            }
        },

        _addFilterToReport: function (filterDefenition) {
            var filterOperatorDefenition = kendoMvvmHelper.findElement(this.reportFilterOperators, 'filterType', filterDefenition.filterType);

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
                    reportFieldTitle: element.reportFieldTitle,
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
                    gridConiguration.columns.push(that._getKendoGridField(element));
                });

                reportGrid.ShowGrid(gridConiguration, data);
            });
        },

        _getKendoGridField: function (element) {
            var field = {
                field: element.field,
                title: element.title
            };

            return field;
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
            
            this.set("columnSelectedField", '');
            this.set("filterSelectedField", '');
            this.set("filterSelectedOperator", '');
            this.set("filterValue", '');

            //Refresh UI
            this._trigerChangeReportModelEvents();
        },

        LoadViewModel: function () {
            var that = this;
            util.get('/api/reports/filters', "Get report filters").done(function (data) {
                data.forEach(function (element) {
                    that.reportFilterOperators.push(element);
                });

                that.trigger("change", { field: "reportFilterOperators" });
            });
            

            util.get('/api/reports/' + this.reportModel.reportType + '/columns', "").done(function (data) {
                data.forEach(function (element) {
                    that.reportColumnsAvailableForReportFocus.push(element);
                });

                that.trigger("change", { field: "reportColumnsAvailableForReportFocus" });
            });
        }
    });

    app.reportsViewModel = reportsViewModel;
})(window, kendo, app);