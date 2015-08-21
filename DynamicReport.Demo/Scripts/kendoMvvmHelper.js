// Ajax and logging utilities. Please keep things object-oriented.

var app = app || {};

app.kendoMvvmHelper =
{
    findIndex: function (array, keyPropertyName, key) {
        var index = -1;
        for (var i = 0; i < array.length; i++) {
            if (array[i][keyPropertyName] === key) {
                index = i;
                break;
            }
        }

        return index;
    },

    findElement: function (array, keyPropertyName, key) {
        var result = null;
        var index = this.findIndex(array, keyPropertyName, key);
        if (index > -1) {
            result = array[index];
        }

        return result;
    }
}