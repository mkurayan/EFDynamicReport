<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SchoolReport.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report Page</title>
    
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

    <link href="Styles/default.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="configPanel">
        <div class="configPanel_header">Add Report Columns</div>
        <div class="configPanel_selectedConfiguration">
            <input  
                data-role="combobox"
                data-placeholder="---Columns---"
                data-text-field="title"
                data-value-field="title"
                data-bind=" value: columnSelectedField,
                            source: reportColumnsAvailableForReportFocus,
                            events: {
                                change: addReportColumn
                            }"
                />
            <br/>
            <p><strong>Fields</strong></p>
            <ul data-bind="source: reportModel.columns" data-template="reportColumn"></ul>

            <ul data-bind="visible: noColumnsSelectedForReport">
                <li class="msg">Add Fields above</li>   
            </ul>
        </div>
    </div>
        
    <div class="configPanel">
        <div class="configPanel_header">Add Report Filters</div>
        <div class="configPanel_selectedConfiguration">
            <input  
                data-role="combobox"
                data-placeholder="---Filter By---"
                data-value-primitive="true"
                data-text-field="title"
                data-value-field="title"
                data-bind=" value: filterSelectedField,
                            source: reportModel.columns"
            />

            <input  
                data-role="combobox"
                data-placeholder="---Select---"
                data-value-primitive="true"
                data-text-field="filterTitle"
                data-value-field="filterType"
                data-bind=" value: filterSelectedOperator,
                    source: reportFilterOperators"
            />
                                    
            <input  
                type="text" 
                class="k-textbox"
                placeholder="---Value---" 
                data-bind="value: filterValue"/>
            
            <a   
                data-role="button"
                data-sprite-css-class="k-icon k-i-funnel"
                data-bind="events: { click: addReportFilter }"
            >Apply Filter</a>
            
            <p><strong>Filters</strong></p>
            <ul data-bind="source: reportModel.filters" data-template="reportFilters"></ul> 
                
            <ul data-bind="visible: noFiltersSelectedForReport">
                <li class="msg">Add Filters above</li>   
            </ul>
        </div>
    </div>
    <div class="configPanel">
         <a data-role="button" 
            data-bind="events: { click: buildReport }"
            class="k-primary">Build Report</a>
    </div>
    <div class="report-grid" id="reportContent"></div> 
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
            var content = $("#form1");
            kendo.bind(content, app.reportsViewModel);
            app.reportsViewModel.LoadViewModel("#reportContent");
        });
    </script>
</body>
</html>
