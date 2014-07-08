function doAdd(url, collection, data) {
    if (data) {
        $.post(url, data,
            function (response, status) {
                if (status = "success") {
                    if (response.error) {
                        alert(response.error);
                    }
                    else {
                        collection.push(response.data);
                    }
                }
            }
        )
    }
}

function doDelete(url, fieldName, collection, data) {
    var arg = {}
    arg[fieldName] = data.Id;
    $.post(url, arg,
        function (response, status) {
            if (status = "success") {
                if (response.error) {
                    alert(response.error);
                }
                else {
                    collection.remove(data);
                }
            }
        }
    )
}

function initializeAutocomplete(inputId, suggestionUrl, propertyName, buttonId, f) {
    var input = $(inputId);
    input.autocomplete({
        source: suggestionUrl,
        select: function (event, ui) {
            input[propertyName] = ui.item.data
        }
    });
    var button = $(buttonId)
    button.click(
        function () {
            f(input[propertyName])

            input.val('');
            input[propertyName] = null;
        }
    );
}