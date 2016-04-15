/* ======================================================
    Filename    : site.js
    Description : Site scripts
    Author      : Nejdet Eren Pinaz
   ====================================================== */

//TODO: Refactor all the code

if (!window.jQuery) {
    throw "Site scripts requires jQuery.";
}

// Don't validate ignored fields
jQuery.validator.defaults.ignore = ".ignore-validation";

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

    // Localize (tr) jQuery validation for decimal and date
    jQuery.extend(jQuery.validator.methods, {
        range: function (value, element, param) {
            var globalizedValue = value.replace(",", ".");
            return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
        },
        number: function (value, element) {
            return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
        },
        date: function (value, element) {
            return this.optional(element)
                || moment(new Date(value)).format();
        }
    });

    // Configure image uploaders
    var imageUploaderModal = $("#imageUploaderModal");
    var $imageUploaderTrigger = $("button.uploader-trigger");

    if (imageUploaderModal.length > 0) {
        var $cropForm = imageUploaderModal.find("form");
        var $cropEditor = imageUploaderModal.find(".crop-editor");
        var $fileInput = $cropEditor.find(".cropit-image-input");

        $imageUploaderTrigger.on("click", function (e) {
            e.preventDefault();

            if ($fileInput.get(0).files.length === 0) {
                $fileInput.trigger("click");
            } else {
                imageUploaderModal.modal("show");
            }
        });

        $cropEditor.cropit({
            imageBackground: true,
            width: $cropEditor.data("width"),
            height: $cropEditor.data("height"),
            smallImage: "stretch",
            initialZoom: "image",
            onFileChange: function (e) {
                var file = e.target.files[0];

                if (file) {
                    if (file.type.match("image/jpeg") || file.type.match("image/jpg") ||
                        file.type.match("image/gif") || file.type.match("image/png")) {
                        imageUploaderModal.modal("show");
                    }
                } else {
                    alert("Dosya seçilmedi.");
                }
            }
        });

        $cropForm.on("submit", function (ev) {
            ev.preventDefault();

            var offset = $cropEditor.cropit("offset");
            var previewSize = $cropEditor.cropit("previewSize");
            var zoom = $cropEditor.cropit("zoom");

            var x1 = Math.round(Math.abs(offset.x) / zoom);
            var x2 = Math.round((Math.abs(offset.x) + previewSize.width) / zoom);
            var y1 = Math.round(Math.abs(offset.y) / zoom);
            var y2 = Math.round((Math.abs(offset.y) + previewSize.height) / zoom);

            $(this).ajaxSubmit({
                cache: false,
                url: this.action,
                method: this.method,
                resetForm: true,
                data: {
                    x: x1,
                    y: y1,
                    w: (x2 - x1),
                    h: (y2 - y1)
                },
                uploadProgress: function (event, position, total, percentComplete) {
                    $(".progress-bar")
                            .css("width", percentComplete + "%")
                            .attr("aria-valuenow", percentComplete);
                },
                success: function (response) {
                    var $finalPreview = $(".uploader-preview");
                    var $finalImage = $("#Image");
                    if ($finalPreview && $finalImage) {
                        $finalPreview.attr("src", response.path);
                        $finalImage.val(response.path);
                    }
                },
                complete: function () {
                    $(".progress-bar")
                            .css("width", 0)
                            .attr("aria-valuenow", 0);

                    imageUploaderModal.modal("hide");
                }
            });
        });
    }
});

$(function () {

    // Organizer form configuration
    var $organizerForm = $("#organizerForm").find("form");
    if ($organizerForm.length > 0) {
        // Organizer description tinymce
        var $organizerDescription = $organizerForm.find("[data-tinymce='organizer']");
        if ($organizerDescription.length > 0) {
            tinymce.init({
                selector: "textarea[data-tinymce='organizer']",
                language: "tr_TR",
                autosave_interval: "20s",
                relative_urls: false,
                entity_encoding: "raw",
                min_height: 300,
                setup: function (editor) {
                    editor.on("change", function () {
                        editor.save();
                    });
                },
                plugins: [
                    "advlist autolink lists link image charmap print preview anchor",
                    "searchreplace visualblocks fullscreen autosave",
                    "insertdatetime media table contextmenu paste"
                ],
                toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
            });
        }

        // Organizer profile select list
        var $profileSelect = $organizerForm.find("#profile-select");
        if ($profileSelect.length > 0) {
            $($profileSelect).on("change", function () {
                window.location = $profileSelect.find("option:selected").data("url");
            });
        }
    }

    // Event form configuration
    var $eventForm = $("#eventForm").find("form");
    if ($eventForm.length > 0) {

        // Event linked datetime pickers
        var $eventStartDate = $eventForm.find("#StartDate");
        var $eventEndDate = $eventForm.find("#EndDate");
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
        var $eventDescription = $eventForm.find("[data-tinymce='event']");
        if ($eventDescription.length > 0) {
            tinymce.init({
                selector: "textarea[data-tinymce='event']",
                language: "tr_TR",
                autosave_interval: "10s",
                relative_urls: false,
                entity_encoding: "raw",
                min_height: 300,
                setup: function (editor) {
                    editor.on("change", function () {
                        editor.save();
                    });
                },
                plugins: [
                    "advlist autolink lists link image charmap print preview anchor",
                    "searchreplace visualblocks fullscreen autosave",
                    "insertdatetime media table contextmenu paste"
                ],
                toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
            });
        }

        // Event ticket container configuration
        var $ticketContainer = $eventForm.find("#ticket-container");
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
                                        parseForValidations($eventForm);
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
                        parseForValidations($eventForm);
                    })
                    .fail(function (jqXhr, textStatus, error) {
                        alert(error);
                    });
            });
        }

        // Event populate sub topics
        var $topicList = $eventForm.find("#TopicId");
        var $subTopicList = $eventForm.find("#SubTopicId");
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
    }

    // Event registration (free ticket) modal
    var getTicketModal = $("#getTicketModal");
    if (getTicketModal.length > 0) {
        getTicketModal.on("show.bs.modal", function (event) {
            var $button = $(event.relatedTarget);
            var url = $button.data("url");
            var $modal = $(this);

            $modal.find(".modal-content").html("<img src='/assets/images/ajax-loader.gif' alt='Loader' width='64' style='display:block; margin: 20px auto;'>");

            $.ajax({
                cache: false,
                url: url,
                method: "GET",
                success: function (data, status, xhr) {
                    var authJson = JSON.parse(xhr.getResponseHeader("X-Responded-Json"));
                    if (authJson) {
                        window.location.replace(authJson.headers.location);
                    }

                    if (data) {
                        $modal.find(".modal-content").html(data);
                        parseForValidations($modal.find("form"));
                    }
                },
                error: function (data, status, xhr) {
                    alert(status);
                }
            });
        });
    }

    // Event registration (paid ticket) modal
    //TODO: Implement paid ticket transaction
});

/* ========================
    Event Bootstrap Table
   ======================== */

// Event Title Formatter
function eventTitleFormatter(value, row) {
    return [
        value,
        "<ul class='list-unstyled list-inline'>",
        "<li><a class='manage' href='/event/manageevent?id=" + row.eventid + "'>Yönet</a></li>",
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