console.log("test")
$(".edit-btn").on("click", function (e) {
    e.preventDefault();
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
$(".save-btn").on("click", function (e) {
    e.preventDefault();
    let editBlock = $(this).closest("span.edit-block");
    let updatedValue = editBlock.find("input:text").val();
    const verificationToken = $("[name='__RequestVerificationToken']").eq(1).attr("value");
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
                headers: {
                    'X-CSRF-TOKEN': verificationToken, 
                },
                data: JSON.stringify(objToSent),
                success: function (data) {
                    $(`.${key} .errors-list`).remove();
                    $(`span.${spanClassName}`).text(data[propName]);
                    editBlock.addClass("d-none");
                },
                
                error: function (error) {
                    let errorObj = error.responseJSON.errors;
                    $(`.${key} .errors-list`).remove();
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