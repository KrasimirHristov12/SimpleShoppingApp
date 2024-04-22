$("table").addClass("table").addClass("table-striped").addClass("table-bordered");

const productId = $(".productId").val();
$.get(`/Products/GetReviewText?productId=${productId}`, function (data) {
    $("#review-text-input").val(data);
});




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
    $(".review-text-input-container").removeClass("d-none");
    $(".rating-info").addClass("d-none");
    let rating = $(".bi-star-fill").index($(this)) + 1;
    $(".bi-star-fill").removeClass("stars-yellow");
    $(".bi-star-fill.star-selected").removeClass("star-selected");
    $(".bi-star-fill").eq(rating - 1).addClass("star-selected");
    for (let i = 0; i < rating; i++) {
        $(".bi-star-fill").eq(i).addClass("stars-yellow");
    }
});

$(".review-text-input-container").find(".btn").on("click", function () {
    const verificationToken = $("[name='__RequestVerificationToken']").attr("value");
    let reviewText = $("#review-text-input").val();
    let rating = $(".bi-star-fill").index($(".star-selected")) + 1;
    $.ajax({
        type: "POST",
        url: "/Products/AddUpdateRating",
        headers: {
            'X-CSRF-TOKEN': verificationToken
        },
        data: {
            productId: productId,
            rating: rating,
            text: reviewText,
        },
        success: function (data) {
            $(".rating-number").text(rating);
            $(".rating-number-avg").text(data.toFixed(2));
            $(".rating-info").removeClass("d-none");
            $(".review-text-input-container").addClass("d-none");
            $(".error-message").remove();
            alert("Thanks for your review!");
        },
        error: function (xhr, status, error) {
            if (xhr.status == 400) {
                let errors = xhr.responseJSON.errors;
                $(".error-message").remove();
                for (let error of errors.Text) {
                    $(`<div><span class="error-message text-danger">${error}</span></div>`).insertBefore($(".review-text-input-container .btn"));
                }
            }
        },
        dataType: "json",
    });
});