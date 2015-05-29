
/* Examples: 
        Get all students:
            GET /api/students
            [URL] /api/students

        Get a students's profile: 
            GET /api/students/1234
            [URL] /api/students/{id}

        Create a new dependent record:
            POST /api/students/1234/dependents
            [URL] /api/students/{id}/dependents
*/

var app = app || {};

app.ApiRequest = (function () {
    var defaults = {
        usedDefaultErrorHandler: true
    };

    function ajax(url, method, data) {
        return $.ajax({
            type: method,
            url: url,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        });
    }

    function addDefaultErrorHandler(request, info) {
        request.fail(function (xhr, status, error) {
            //Here we can implement any default error handler.
            var error = "Error: " + error + ". Info: " + info;
            alert(error);
        });
    }

    // constructor
    function ApiModule(options) {
        this.options = $.extend({}, defaults, options);
    };


    // prototype
    ApiModule.prototype = {
        get: function (url, info) {
            return this.apiRequest(url, "GET", null, info);
        },

        post: function (url, data, info) {
            return this.apiRequest(url, "POST", data, info);
        },

        put: function() {
            return this.apiRequest(url, "PUT", data, info);
        },

        delete: function (url, data, info) {
            return this.apiRequest(url, "DELETE", data, info);
        },

        apiRequest: function (url, method, data, info) {
            var request = ajax(url, method, data);

            if (this.options.usedDefaultErrorHandler) {
                addDefaultErrorHandler(request, info);
            }

            return request;
        }
    };

    return ApiModule;
})();