$(".search-btn").on("click", function (e) {

    e.preventDefault();
    if (!$(".search-input").val()) {
        console.log($(".search-input").val());
        alert("Please specify search term.");
    }
    else {
        $(".search-form").submit();
    }
});





