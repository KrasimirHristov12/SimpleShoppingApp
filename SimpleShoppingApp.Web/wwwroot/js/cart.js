$(".remove-btn").on("click", function () {
    let removeBtn = $(this);
    const verificationToken = $("[name='__RequestVerificationToken']").attr("value");
    let idOfProduct = removeBtn.closest(".product").find("input:hidden").val();
    $.ajax({
        type: "POST",
        url: "/Cart/DeleteProduct",
        headers: {
            'X-CSRF-TOKEN': verificationToken
        },
        data: {
            productId: idOfProduct,
        },
        success: function (data) {
            removeBtn.closest(".product").next().remove();
            removeBtn.closest(".product").remove();
            let newProductsLength = data.newCount
            const products = $(".product");
            for (let i = 0; i < products.length; i++) {
                let quantity = "quantities[" + i + "]";
                products.eq(i).find(".quantity-input").prop("id", quantity).prop("name", quantity);
                products.eq(i).find(".quantity-input").prev().prop("for", quantity);
            }
            $(".products-count").text(newProductsLength);
            $(".total-price").text('$' + parseFloat(data.newTotalPrice).toFixed(2));
        },
        dataType: "json",
    });
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

$(".modal-address-footer .submit").on("click", function () {
    let address = $(".modal input#Name");
    const verificationToken = $("[name='__RequestVerificationToken']").attr("value");
    if (address.val()) {

        $.ajax({
            type: "POST",
            url: "/Addresses/AddAddress",
            headers: {
                'X-CSRF-TOKEN': verificationToken
            },
            data: {
                name: address.val(),
            },
            success: function (data) {
                $(".address-select").append(`<option value="${data.id}">${data.name}</option>`);
                $(".address-select").val(data.id).trigger("change");
                address.val('');
                $(".modal-address-footer .close").trigger("click");
            },
            dataType: "json",
        });
    }
});