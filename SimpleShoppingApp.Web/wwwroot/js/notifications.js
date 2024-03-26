$(".notifications-container").on("click", function () {
    if ($(this).find(".list-group").hasClass("d-none")) {
        $(this).find(".list-group").removeClass("d-none"); 
    }
    else {
        $(this).find(".list-group").addClass("d-none"); 
    }
});