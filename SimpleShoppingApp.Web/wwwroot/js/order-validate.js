const inputs = $(".quantity-input");

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


$("#order-form").validate(validateObject);

$(".quantity-input").on("keyup change", function () {
    let quantityInput = $(this)
    let updatedQuantity = quantityInput.val();
    let productName = quantityInput.closest(".row").find("p strong").eq(0).text();
    if (!isNaN(updatedQuantity)) {
        let updatedQuantityNum = parseInt(updatedQuantity);
        if (updatedQuantityNum > 0) {
            let idOfProduct = quantityInput.closest(".product").find("input:hidden").val();
            $.ajax({
                type: "POST",
                url: "/Cart/UpdateQuantity",
                data: {
                    productId: idOfProduct,
                    updatedQuantity: updatedQuantityNum.toString(),
                },
                success: function (data) {
                    if (data.updatedQuantity == 0) {
                        alert(`Unfortunately, ${productName} is out of stock. It will be deleted from your cart.`);
                        quantityInput.closest(".row").find(".remove-btn").click();
                    }
                    else {
                        let quantityLeft = data.updatedQuantity;
                        if (data.isThereLessThanRequested) {
                            alert(`There are ${quantityLeft} ${productName}s left.`);
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
    else {
        alert("Product quantity must be a number");
        $(this).val("1");
    }
});