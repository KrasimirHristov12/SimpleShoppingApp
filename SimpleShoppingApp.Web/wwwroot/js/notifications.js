$(".notifications-container").on("click", function () {
    if ($(this).find(".list-group").hasClass("d-none")) {
        $(this).find(".list-group").removeClass("d-none"); 
    }
    else {
        $(this).find(".list-group").addClass("d-none"); 
    }
});

$(".list-group-item a").on("click", function (e) {

    const currentClickedLink = $(this);

    e.preventDefault();

    let notificationId = currentClickedLink.closest(".list-group-item").attr("id").split("-")[1];
    $.ajax({
        type: "POST",
        url: "/Notifications/Read",
        data: {
            notificationId: notificationId,
        },
        success: function () {
            console.log("test");
            window.open(currentClickedLink.attr("href"));
        },
    });
});
