// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(".search-btn").on("click", function (e) {
    
    e.preventDefault();
    if (!$(".search-input").val()) {
        console.log($(".search-input").val());
        alert("Please specify search term.");
    }
    else {
        $(".search-form").submit();
    }
})
