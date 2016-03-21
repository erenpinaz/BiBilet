/* ======================================================
    Filename    : site.js
    Description : Site scripts
    Author      : Nejdet Eren Pinaz
   ====================================================== */

if (!window.jQuery) {
    throw "Site scripts requires jQuery.";
}

$(function () {

    // Update selected item of a select menu when browser back button pressed
    // Credits: http://stackoverflow.com/questions/4370819/select-menu-not-being-restored-when-back-button-used/28302447#28302447
    $("select").each(function () {
        var select = $(this);
        var selectedValue = select.find("option[selected]").val();

        if (selectedValue) {
            select.val(selectedValue);
        } else {
            select.prop("selectedIndex", 0);
        }
    });

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

    // Organizer profile image uploader
    var $organizerImageUploader = $("#organizer-image-uploader");
    if ($organizerImageUploader.length > 0) {
        var uploader = new plupload.Uploader({
            runtimes: "html5,flash",
            browse_button: "image-picker",
            container: "organizer-image-uploader",
            url: $organizerImageUploader.data("url"),
            max_file_size: "5mb",
            dragdrop: false,
            multiple_queues: false,
            multi_selection: false,
            max_file_count: 1,
            resize: {
                width: 240,
                height: 240,
                crop: true
            },
            filters: [
                { title: "Image files", extensions: "jpg,jpeg,gif,png" }
            ],
            flash_swf_url: "/assets/js/plupload/Moxie.swf",
            silverlight_xap_url: "/assets/js/plupload/Moxie.xap"
        });

        uploader.bind("Init", function (up, params) {
            $("#runtime").html("Current runtime: " + params.runtime);
        });

        uploader.bind("Browse", function () {
            uploader.splice();
            uploader.refresh();
        });

        uploader.bind("FilesAdded", function (up, file) {
            up.start();
        });

        uploader.bind("FileUploaded", function (up, file, data) {
            var response = jQuery.parseJSON(data.response);
            var $preview = $organizerImageUploader.find("img").get(0);
            var $image = $organizerImageUploader.find("#Image").get(0);

            if (response && $preview && $image) {
                $($preview).attr("src", response.path);
                $($image).attr("value", response.path);
            }
        });

        uploader.init();
    }
});