console.log("test")
$(".edit-btn").on("click", function () {
    let spanEdit = $(this).closest("li").find("span.edit-block");
    if (spanEdit.hasClass("d-none")) {
        spanEdit.removeClass("d-none").addClass("d-inline");
    }
    else {
        spanEdit.removeClass("d-inline").addClass("d-none");
    }
    
});

const obj = {
    "full-name-edit-block": {
        url: "EditFullName",
        objToSentKey: "FullName",
        spanClassName: "full-name",
        propName: "fullName",
    },
    "phone-number-edit-block": {
        url: "EditPhoneNumber",
        objToSentKey: "PhoneNumber",
        spanClassName: "phone-number",
        propName: "phoneNumber",
    },
}
$(".save-btn").on("click", function () {
    let editBlock = $(this).closest("span.edit-block");
    let updatedValue = editBlock.find("input:text").val();
    for (const [key, value] of Object.entries(obj)) {


        if (editBlock.hasClass(key)) {

            const objToSent = {}
            objToSent[value["objToSentKey"]] = updatedValue
            let url = value["url"];
            let spanClassName = value["spanClassName"];
            let propName = value["propName"];

            $.ajax({
                type: "POST",
                url: `/Users/${url}`,
                contentType: "application/json",
                data: JSON.stringify(objToSent),
                success: function (data) {
                    console.log(data);
                    $(`span.${spanClassName}`).text(data[propName]);
                    editBlock.addClass("d-none");
                },
                error: function (error) {
                    let errorObj = error.responseJSON.errors;
                    $(`.${key} .errors-list`).empty();
                    for (const [key, value] of Object.entries(errorObj)) {
                        for (const err of value) {
                            editBlock.append($(`<div class="errors-list"><span class="text-danger">${err}</span></div>`));
                        }
                    }
                },
                dataType: "json",
            });
        }
    }

});