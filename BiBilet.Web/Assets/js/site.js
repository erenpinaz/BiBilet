/* ======================================================
    Filename    : site.js
    Description : Site scripts
    Author      : Nejdet Eren Pinaz
   ====================================================== */

if (!window.jQuery) {
    throw "Site scripts requires jQuery.";
}

$(function () {

    // Bootstrap table resize fix
    var $table = $("table[data-toggle='table']");
    if ($table.length > 0) {
        $(window).resize(function () {
            $('table[data-toggle="table"]').bootstrapTable("resetView");
        });
    }

    // Organizer description tinymce
    var $organizerDescription = $("#organizerForm").find("[data-tinymce='organizer']");
    if ($organizerDescription.length > 0) {
        tinymce.init({
            selector: "textarea[data-tinymce='organizer']",
            language: "tr_TR",
            autosave_interval: "20s",
            relative_urls: false,
            entity_encoding: "raw",
            min_height: 300,
            plugins: [
                "advlist autolink lists link image charmap print preview anchor",
                "searchreplace visualblocks fullscreen autosave",
                "insertdatetime media table contextmenu paste"
            ],
            toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
        });
    }

    // Organizer profile select list
    var $profileSelect = $("#organizerForm").find("#profile-select");
    if ($profileSelect.length > 0) {
        $($profileSelect).on("change", function () {
            window.location = $profileSelect.find("option:selected").data("url");
        });
    }
});