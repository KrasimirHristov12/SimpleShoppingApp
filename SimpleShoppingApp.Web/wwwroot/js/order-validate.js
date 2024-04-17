const inputs = $(".quantity-input");
const form = $("#order-form");
const validateObject = {
    rules: {

    },
    submitHandler: function (form) {
        form.submit();
    }
}

for (let i = 0; i < inputs.length; i++) {
    validateObject.rules[inputs.eq(i).attr("name")] = {
        required: true,
        min: 1,
    }
}

form.validate(validateObject);


inputs.on("keyup change", function () {

    if (!form.valid()) {
        $(".total-price").text("");
        const notHiddenErrors = $("label.error").not(":hidden");
        notHiddenErrors.each(function () {
            $(this).closest(".row").find(".product-price").text("");
        });
    }

    else {

        let quantityInput = $(this)
        const verificationToken = $("[name='__RequestVerificationToken']").attr("value");
        let updatedQuantity = quantityInput.val();
        let productName = quantityInput.closest(".row").find("p strong").eq(0).text();
        if (!isNaN(updatedQuantity)) {
            let updatedQuantityNum = parseInt(updatedQuantity);
            if (updatedQuantityNum > 0) {
                let idOfProduct = quantityInput.closest(".product").find("input:hidden").val();
                $.ajax({
                    type: "POST",
                    url: "/Cart/UpdateQuantity",
                    headers: {
                        'X-CSRF-TOKEN': verificationToken
                    },
                    data: {
                        productId: idOfProduct,
                        updatedQuantity: updatedQuantityNum.toString(),
                    },
                    success: function (data) {
                        if (data.updatedQuantity == 0) {
                            alert(`Unfortunately, ${productName} is out of stock. It will be deleted from your cart.`);
                            quantityInput.closest(".row").find(".remove-btn").trigger("click");
                        }
                        else {
                            let quantityLeft = data.updatedQuantity;
                            if (data.isThereLessThanRequested) {
                                alert(`There are ${quantityLeft} ${productName} left.`);
                            }

                            quantityInput.val(quantityLeft);
                            quantityInput.closest("div.mb-4").next().find(".product-price").text('$' + parseFloat(data.newProductPrice).toFixed(2));
                            $(".total-price").text('$' + parseFloat(data.newTotalPrice).toFixed(2));
                        }

                    },
                    dataType: "json",
                });

            }
        }
    }

});