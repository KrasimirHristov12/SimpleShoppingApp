$(".modal-address-footer .submit").on("click", function () {
    let address = $(".modal input#Name")
    if (address.val()) {

        $.ajax({
            type: "POST",
            url: "/Orders/AddAddress",
            data: {
                name: address.val(),
            },
            success: function (data) {
                $(".address-select").append(`<option value="${data.id}">${data.name}</option>`);
                address.val('');
                $(".modal-address-footer .close").click();
            },
            dataType: "json",
        });
    }
});