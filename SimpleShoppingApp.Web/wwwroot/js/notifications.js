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
