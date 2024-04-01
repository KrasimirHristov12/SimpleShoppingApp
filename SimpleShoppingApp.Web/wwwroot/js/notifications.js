$(".list-group-item a").on("click", function (e) {
    e.preventDefault();
    const currentClickedLink = $(this);
    const verificationToken = $("[name='__RequestVerificationToken']").attr("value");

    e.preventDefault();

    let notificationId = currentClickedLink.closest(".list-group-item").attr("id").split("-")[1];
    $.ajax({
        type: "POST",
        url: "/Notifications/Read",
        headers: {
            'X-CSRF-TOKEN': verificationToken
        },
        data: {
            notificationId: notificationId,
        },
        success: function () {
            window.open(currentClickedLink.attr("href"));
        },
    });
});
