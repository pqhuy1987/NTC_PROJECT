﻿function AddLoader() {
    if ($('#loader-wrapper').length == 0) {
        var loader = $("<div id='loader-wrapper'></div>");
        loader.append("<div id='loader'></div>");
        $(".RICONS-body").append(loader);
    }
}

function RemoveLoader() {
    $('#loader-wrapper').remove();
}