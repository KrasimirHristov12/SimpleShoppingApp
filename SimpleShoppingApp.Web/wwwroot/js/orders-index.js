$(".btn-check").on("click", function () {
    let statusNum = $(this).hasClass("btn-delivered") ? 1 : 0;

    if (!$(this).hasClass("checked")) {
        $(this).addClass("checked");
        $(".btn-check").not($(this)).removeClass("checked");
        $.ajax({
            type: "GET",
            url: `/Orders/GetByStatus?status=${statusNum}`,
            success: function (data) {
                $(".my-orders").empty();
                const orders = data;
                if (orders.length == 0) {
                    $(".my-orders").append(`<p>No orders found!</p>`);
                }
                else {
                    $(".my-orders").append(`<ul class="list-group"></ul>`)
                    for (const order of orders) {
                        $(".my-orders ul").append(`<li class="list-group-item">Order #${order.id} - Total price: $${order.totalPrice.toFixed(2)}<br /><br /><a href="/Orders/Details/${order.id}">View Details</a></li>`);
                    }
                }
            },
            dataType: "json",
        });
    }
});
