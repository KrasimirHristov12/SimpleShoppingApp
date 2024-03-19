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
                $(".address-select").trigger("change");
                address.val('');
                $(".modal-address-footer .close").trigger("click");
            },
            dataType: "json",
        });
    }
});