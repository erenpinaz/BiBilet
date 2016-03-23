/* ======================================================
    Filename    : site.js
    Description : Site scripts
    Author      : Nejdet Eren Pinaz
   ====================================================== */

if (!window.jQuery) {
    throw "Site scripts requires jQuery.";
}

$(function () {
    // Initialize moment
    moment.locale("tr");

    // Fix bootstrap table resize
    var $table = $("table[data-toggle='table']");
    if ($table.length > 0) {
        $(window).resize(function () {
            $('table[data-toggle="table"]').bootstrapTable("resetView");
        });
    }

    // Localize (tr) jQuery validation for decimal and date
    jQuery.extend(jQuery.validator.methods, {
        number: function (value, element) {
            return this.optional(element)
             || /^-?(?:\d+|\d{1,3}(?:[\s\,]\d{3})+)(?:[\,]\d+)?$/.test(value);
        },
        date: function (value, element) {
            return this.optional(element)
                || moment(new Date(value)).format();
        }
    });

    // Update select lists when browser back button pressed
    // Credits: http://stackoverflow.com/questions/4370819/select-menu-not-being-restored-when-back-button-used/28302447#28302447
    $("select").each(function () {
        var $select = $(this);
        var selectedValue = $select.find("option[selected]").val();

        if (selectedValue) {
            $select.val(selectedValue);
        } else {
            $select.prop("selectedIndex", 0);
        }
    });

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
        var $preview = $organizerImageUploader.find("#preview").get(0);
        var $image = $organizerImageUploader.find("#Image").get(0);

        var uploader = new plupload.Uploader({
            runtimes: "html5,flash",
            browse_button: "picker",
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

        uploader.bind("UploadProgress", function (up, file) {
            var $progress = $organizerImageUploader.find("#progress").find(".progress-bar").get(0);
            if ($progress) {
                $($progress)
                    .text(file.percent)
                    .css("width", file.percent + "%")
                    .attr("aria-valuenow", file.percent);
            }
        });

        uploader.bind("FileUploaded", function (up, file, data) {
            var response = jQuery.parseJSON(data.response);

            if (response.path && $preview && $image) {
                $($preview).attr("src", response.path);
                $($image).attr("value", response.path);
            }
        });

        uploader.init();
    }

    // Event linked datetime pickers
    var $eventStartDate = $("#eventForm").find("#StartDate");
    var $eventEndDate = $("#eventForm").find("#EndDate");
    if ($eventStartDate.length > 0 && $eventEndDate.length > 0) {
        $eventStartDate.datetimepicker({
            locale: "tr"
        });
        $($eventEndDate).datetimepicker({
            locale: "tr",
            useCurrent: false
        });
        $($eventStartDate).on("dp.change", function (e) {
            $($eventEndDate).data("DateTimePicker").minDate(e.date);
        });
        $($eventEndDate).on("dp.change", function (e) {
            $($eventStartDate).data("DateTimePicker").maxDate(e.date);
        });
    }

    // Event description tinymce
    var $eventDescription = $("#eventForm").find("[data-tinymce='event']");
    if ($eventDescription.length > 0) {
        tinymce.init({
            selector: "textarea[data-tinymce='event']",
            language: "tr_TR",
            autosave_interval: "10s",
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

    // Event ticket container configuration
    var $form = $("#eventForm").find("form");
    var $ticketContainer = $form.find("#ticket-container");
    if ($ticketContainer.length > 0) {

        // Parses the delete ticket buttons
        var parseDeleteTicketButtons = function (selector) {
            $(selector).on("click", function (e) {
                e.preventDefault();

                var ticketId = $(selector).data("id");
                if (ticketId) {
                    if (confirm("Bu bileti silmek istiyor musunuz?")) {
                        $.post("/ticket/deleteticket?id=" + ticketId, function () { })
                            .done(function (data) {
                                if (data.result === "success") {
                                    $(this).parents(".ticket-item:first").remove();
                                    parseForValidations($form);
                                } else {
                                    alert("Bilet silinirken bir hata oluştu.");
                                }
                            })
                            .fail(function (jqXhr, textStatus, error) {
                                alert(error);
                            });
                    }
                }
            });
        }
        parseDeleteTicketButtons($(".delete-ticket"));

        var $createTicketButtons = $ticketContainer.find(".create-ticket");
        $createTicketButtons.on("click", function (e) {
            e.preventDefault();

            $.get("/event/addticketitem?type=" + $(this).data("type"), function () { })
                .done(function (data) {
                    $ticketContainer.find(".panel-body").append(data);
                    parseDeleteTicketButtons($(".delete-ticket:last"));
                    parseForValidations($form);
                })
                .fail(function (jqXhr, textStatus, error) {
                    alert(error);
                });
        });
    }

    // Event populate sub topics
    var $topicList = $("#eventForm").find("#TopicId");
    var $subTopicList = $("#eventForm").find("#SubTopicId");
    if ($topicList.length > 0 && $subTopicList.length > 0) {
        $topicList.on("change", function () {
            var topicId = $(this).val();
            if (topicId) {
                $subTopicList.empty();

                $.getJSON("/event/populatesubtopics?topicId=" + topicId, function (data) {
                    $.each(data, function (i, subTopic) {
                        $subTopicList.append("<option value='" + subTopic.subtopicid + "'>" + subTopic.name + "</option>");
                    });
                });
            }
        });
    }

});

/* ========================
    Event Bootstrap Table
   ======================== */

// Event Title Formatter
function eventTitleFormatter(value, row) {
    return [
        value,
        "<ul class='list-unstyled list-inline'>",
        "<li><a class='update' href='/event/updateevent?id=" + row.eventid + "'>Güncelle</a></li>",
        "<li><a class='remove' href='/event/deleteevent?id=" + row.eventid + "'>Sil</a></li>",
        "</ul>"
    ].join("");
}

// Event Status Formatter
function eventStatusFormatter(value) {
    if (value) {
        return "<span class='text-success'>Yayında</span>";
    } else {
        return "<span class='text-warning'>Taslak</span>";
    }
}

// Json Date Formatter
function jsonDateFormatter(value) {
    if (value) {
        return "<abbr class='initialism' title='" + moment(value).format() + "'>" + moment(value).format("LLLL") + "</abbr>";
    }
    return "-";
}

/* ====================
    Utils
   ==================== */

// Parse form for validations
function parseForValidations(selector) {
    $(selector)
        .removeData("validator")
        .removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(selector);
}

// String.format
// http://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format
if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != "undefined"
                ? args[number]
                : match;
        });
    };
}

// Slug formatter
// http://stackoverflow.com/questions/1053902/how-to-convert-a-title-to-a-url-slug-in-jquery
function convertToSlug(url) {
    return url
        .toLowerCase()
        .replace(/ /g, "-")
        .replace(/[^\w-]+/g, "");
}

// IEC (1024) file size prefix
// http://stackoverflow.com/questions/10420352/converting-file-size-in-bytes-to-human-readable
function fileSizeIEC(a, b, c, d, e) {
    return (b = Math, c = b.log, d = 1024, e = c(a) / c(d) | 0, a / b.pow(d, e)).toFixed(2) + " " + (e ? "KMGTPEZY"[--e] + "iB" : "Bytes");
}

// SI (1000) file size prefix
// http://stackoverflow.com/questions/10420352/converting-file-size-in-bytes-to-human-readable
function fileSizeSI(a, b, c, d, e) {
    return (b = Math, c = b.log, d = 1e3, e = c(a) / c(d) | 0, a / b.pow(d, e)).toFixed(2) + " " + (e ? "kMGTPEZY"[--e] + "B" : "Bytes");
}