$("table").addClass("table").addClass("table-striped").addClass("table-bordered");

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