var connection = new signalR.HubConnectionBuilder().withUrl("/notificationsHub").build();


connection.on("Connected", function () {
    console.log("connected successfully");
})
connection.on("ReceiveNotification", function (notificationText, productLink, notificationId) {
    $(".notifications-container ul").prepend(`<li id="n-${notificationId}" class="list-group-item">
                                                <span class="new-label" style="font-size: 10px;">NEW<br /></span>
                                                <a class="dropdown-item" href="${productLink}">${notificationText}</a>
                                             </li>`);
    $(".new-notifications-count").text("(" + $("li .new-label").length.toString() + ")");
});

$(".notifications-container").on("click", "a.dropdown-item",  function (e) {
    e.preventDefault();
    console.log($(this))
    let notificationId = parseInt($(this).closest("li").attr("id").replace("n-", ""));
    connection.invoke("ReadAsync", notificationId).catch(err => console.log(err.toString()));
    let link = $(this).attr("href");
    if (!link.includes("#")) {
        window.open(link, "_top");
    }

    else {

        $(this).prev(".new-label").remove();
        $(".new-notifications-count").text("(" + $("li .new-label").length.toString() + ")");
    }
});

connection.start();