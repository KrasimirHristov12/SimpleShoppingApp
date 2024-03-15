// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(".search-btn").on("click", function (e) {

    e.preventDefault();
    if (!$(".search-input").val()) {
        console.log($(".search-input").val());
        alert("Please specify search term.");
    }
    else {
        $(".search-form").submit();
    }
})

$(".delete-btn").on("click", function (e) {
    e.preventDefault();
    swal({
        title: "Delete product",
        text: "Are you sure you want to delete this product?",
        icon: "error",
        buttons: true,
        dangerMode: true,
    }).then(willDelete => {
        if (willDelete) {
            $(".delete-product-form").submit();
        }
    });
});

$(".bi-star-fill").on("mouseenter", function () {
    let indexOfThis = $(".bi-star-fill").index($(this));

    for (let i = 0; i < indexOfThis + 1; i++) {
        $(".bi-star-fill").eq(i).addClass("stars-yellow")
    }

});

$(".bi-star-fill").on("mouseleave", function () {
    let indexOfSelected = $(".bi-star-fill").index($(".star-selected"));
    let indexOfThis = $(".bi-star-fill").index($(this));
    if (indexOfThis > indexOfSelected) {
        $(this).removeClass("stars-yellow");
    }
});


$(".bi-star-fill").on("click", function () {
    let rating = $(".bi-star-fill").index($(this)) + 1;
    $(".bi-star-fill").removeClass("stars-yellow");
    let productId = $(".productId").val();
    $.ajax({
        type: "POST",
        url: "/Products/AddUpdateRating",
        data: {
            productId: productId,
            rating: rating,
        },
        success: function (data) {
            $(".rating-number").text(rating);
            let selectedRating = $(".bi-star-fill").eq(rating - 1);
            $(".bi-star-fill").not(selectedRating).removeClass("star-selected");
            selectedRating.addClass("star-selected");
           for (let i = 0; i < rating; i++) {
               $(".bi-star-fill").eq(i).addClass("stars-yellow");
            }
            $(".rating-number-avg").text(data.toFixed(2));
        },
        dataType: "json",
    });
});
$(".remove-btn").on("click", function () {
    let removeBtn = $(this);
    let idOfProduct = removeBtn.closest(".product").find("input:hidden").val();
    $.ajax({
        type: "POST",
        url: "/Cart/DeleteProduct",
        data: {
            productId: idOfProduct,
        },
        success: function (data) {
            let deletedProductId = data.productId;
            removeBtn.closest(".product").next().remove();
            removeBtn.closest(".product").remove();
            $(".products-count").text(data.newCount);
            $(".total-price").text('$' + parseFloat(data.newTotalPrice).toFixed(2));
        },
        dataType: "json",
    });
});



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

$(".specify-address-flag").on("change", function () {
    let addressTextField = $(this).closest(".form-check").prev().find("input:text");
    if ($(this).is(":checked")) {
        addressTextField.val('').prop("disabled", true);
    }
    else {
        addressTextField.prop("disabled", false);
    }
}).trigger("change");

$(".address-select").on("change", function () {
    $(".address-list span").text($(".address-select option:selected").text());
}).trigger("change");

$(".paymentMethod-dropdown").on("change", function () {
    $(".payment-method span").text($(".paymentMethod-dropdown option:selected").text());
}).trigger("change");

$("#PhoneNumber").on("keyup", function () {
    $(".phone-number span").text($(this).val());
}).trigger("keyup");





