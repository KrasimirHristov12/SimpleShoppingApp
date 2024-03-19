$(".image-url").each(function () {
    if ($(this).val()) {
        $(this).removeClass("d-none");
    }
})

$(".add-url-btn").on("click", function (e) {
    e.preventDefault();
    const hiddenFields = $(".image-url.d-none");
    if (hiddenFields.length == 1) {
        $(this).addClass("d-none");
    }
    hiddenFields.eq(0).removeClass("d-none");


});

