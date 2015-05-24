<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SchoolReport.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <link rel="stylesheet" href="//cdn.kendostatic.com/2015.1.429/styles/kendo.common-material.min.css" />
    <link rel="stylesheet" href="//cdn.kendostatic.com/2015.1.429/styles/kendo.material.min.css" />
    <link rel="stylesheet" href="//cdn.kendostatic.com/2015.1.429/styles/kendo.dataviz.min.css" />
    <link rel="stylesheet" href="//cdn.kendostatic.com/2015.1.429/styles/kendo.dataviz.material.min.css" />

    <script src="//cdn.kendostatic.com/2015.1.429/js/jquery.min.js"></script>
    <script src="//cdn.kendostatic.com/2015.1.429/js/kendo.all.min.js"></script>
    
    <script src="Scripts/kendoMvvmHelper.js"></script>
    <script src="Scripts/apiRequestHelper.js"></script>
    <script src="Scripts/kendoGrid.js"></script>
    <script src="Scripts/report.viewmodel.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div id="kBindDiv">
        <div class="configPanel">
            <input 
                    data-role="combobox"
                    data-placeholder="Select Report Columns"
                    data-text-field="title"
                    data-value-field="title"
                    data-bind=" value: columnSelectedField,
                                source: reportColumnsAvailableForReportFocus,
                                events: {
                                    change: addReportColumn
                                }"
                />
            <div class="selected-configuration">
                <strong>Fields</strong>
                <ul data-bind="source: reportModel.columns" data-template="reportColumn"></ul>

                <ul data-bind="visible: noColumnsSelectedForReport">
                    <li class="msg">Add Fields above</li>   
                </ul>
            </div>
        </div>
        
        <div class="configPanel">
            <input style="width: 205px;"
                    data-role="combobox"
                    data-placeholder="---Filter By---"
                    data-value-primitive="true"
                    data-text-field="title"
                    data-value-field="title"
                    data-bind=" value: filterSelectedField,
                                source: reportModel.columns"
                />

            <input style="width: 205px;"
                data-role="combobox"
                data-placeholder="---Select---"
                data-value-primitive="true"
                data-text-field="filterTitle"
                data-value-field="filterType"
                data-bind=" value: filterSelectedOperator,
                            source: reportFilterOperators"
                />
                                    
            <input type="text" data-bind="value: filterValue"/>
            <input value="Apply Filter" data-bind="click: addReportFilter" />

            <div class="selected-configuration">
                <strong>Filters</strong>
                <ul data-bind="source: reportModel.filters" data-template="reportFilters"></ul> 
                
                <ul data-bind="visible: noFiltersSelectedForReport">
                    <li class="msg">Add Filters above</li>   
                </ul>
            </div>
        </div>
        
         <a href="#" data-bind="click: buildReport"><span>Build Report</span></a>
    </div>
     
    <div id="reportContent">
    </div>

    </form>
    
    <script type="text/x-kendo-template" id="reportColumn">
        <li>
            #= title #
            <a href="\\#" class="remove" title="Remove"  data-bind="click: removeReportColumn" data-report-field="#= title #">x</a>
        </li>
    </script>
    
    <script type="text/x-kendo-template" id="reportFilters">
        <li>
            #= reportFieldTitle # #= filterTitle # #= filterValue # 
            <a href="\\#" class="remove" title="Remove" data-bind="click: removeReportFilter" data-report-hashCode="#= hashCode #">x</a>
        </li>
    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            var content = $("#kBindDiv");
            kendo.bind(content, app.reportsViewModel);
            app.reportsViewModel.LoadViewModel("#reportContent");
        });
    </script>
</body>
</html>
