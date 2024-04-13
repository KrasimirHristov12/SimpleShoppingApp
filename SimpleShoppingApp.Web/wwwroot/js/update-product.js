$(".has-discount").on("change", function () {
    if ($(this).is(":checked")) {
        $(".new-price").removeClass("d-none");
    }
    else {
        $(".new-price").addClass("d-none");
        $(".new-price input:text").val("");
    }
}).trigger("change");