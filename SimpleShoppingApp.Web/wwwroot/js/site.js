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
    $(".bi-star-fill").removeClass("stars-yellow");
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

$(".quantity-input").on("keyup change", function () {
    let quantityInput = $(this)
    let updatedQuantity = quantityInput.val();
    if (!isNaN(updatedQuantity)) {
        let updatedQuantityNum = parseInt(updatedQuantity);
        if (updatedQuantityNum <= 0) {
            alert("Product quantity must be a positive number.");
            $(this).val("1");
        }
        else {
            let idOfProduct = quantityInput.closest(".product").find("input:hidden").val();
            $.ajax({
                type: "POST",
                url: "/Cart/UpdateQuantity",
                data: {
                    productId: idOfProduct,
                    updatedQuantity: updatedQuantityNum.toString(),
                },
                success: function (data) {
                    quantityInput.closest("div.mb-4").next().find(".product-price").text('$' + parseFloat(data.newProductPrice).toFixed(2));
                    $(".total-price").text('$' + parseFloat(data.newTotalPrice).toFixed(2));
                },
                dataType: "json",
            });
        }
    }
    else {
        alert("Product quantity must be a number");
        $(this).val("1");
    }
});



