let categoryId = $(".products-filter-container").attr("data-categoryId");
let url = `/Products/GetFilteredProducts?category=${categoryId}&`;

const urlObj = {
    prices: [],
    ratings: [],
};

const filters = ["prices", "ratings"];

function constructUrl(url, urlObj) {
    for (const [key, value] of Object.entries(urlObj)) {
        for (const index of value) {
            url += `${key}[${value.indexOf(index)}]=${index}&`;
        }
    }
    return url[url.length - 1] != '&' ? url : url.substring(0, url.length - 1);
}

for (const filter of filters) {

    const filterCheckboxes = $(`.${filter}-filter-checkbox`);
    filterCheckboxes.on("change", function () {
        let priceIndex = filterCheckboxes.index($(this));
        if ($(this).is(":checked")) {
            if (!urlObj[filter].includes(priceIndex)) {
                urlObj[filter].push(priceIndex);
            }
        }

        else {
            if (urlObj[filter].includes(priceIndex)) {
                let indexOfPrice = urlObj[filter].indexOf(priceIndex);
                urlObj[filter].splice(indexOfPrice, 1);
            }
        }
        let updatedUrl = constructUrl(url, urlObj);

        $.ajax({
            type: "GET",
            url: updatedUrl,
            success: function (data) {
                console.log(updatedUrl);
                const allProducts = $(".product");
                allProducts.remove();
                console.log(data);
                $(".products-count").text(data.length);

                if (data.length == 0) {
                    $(".products-filter-container").append(`<p class="fw-bold col-md-9 no-products-found">No products found</p>`)
                }

                else {

                    $(".no-products-found").remove();
                    data.forEach(function (product) {
                        $(".products-filter-container").append(`
                        <div class="col-md-3 mb-3 product">
                             <a href="/Products/Index/${product.id}" class="link-dark text-decoration-none">
                                <div class="card">
                                   <img src="/images/products/${product.image.name}${product.image.extension}" class="card-img-top product-img" alt="${product.name} Image" />
                                   <div class="card-body">
                                       <h5 class="card-title">${product.name}</h5>
                                       <p class="card-text">Rating: ${product.rating.toFixed(2)}<br /><br /><span class="text-danger fw-bold fs-6">$${product.price.toFixed(2)}</span></p>
                                   </div>
                                </div>
                            </a>
                        </div>`);
                    });
                }


            },
            dataType: "json",
        });

    });
}

$(".page-link").on("click", function (e) {
    e.preventDefault();
    let currentPage = $(this);

    if (currentPage.closest(".page-item").hasClass("prev")) {
        if ($(".page-item.active").hasClass("page-1")) {
            currentPage = null;
        }
        else {
            currentPage = $(".page-item.active").prev().find(".page-link");
        }
        
    }

    else if (currentPage.closest(".page-item").hasClass("next")) {
        if ($(".page-item.active").next().hasClass("next")) {
            currentPage = null;
        }
        else {

            currentPage = $(".page-item.active").next().find(".page-link");
        }
        
    }

    if (currentPage != null) {

        let currentPageText = currentPage.closest(".page-item").attr("class").split(" ")[1].split("-")[1];
        let currentPageNum = parseInt(currentPageText);

        let elementsPerPage = 1

        $.ajax({
            type: "GET",
            url: `/Products/GetProductsPerPage?currentPage=${currentPageNum}&elementsPerPage=${elementsPerPage}&categoryId=${categoryId}`,
            success: function (data) {

                console.log(data);

                window.history.pushState(null, "" ,currentPage.attr("href"));

                //window.location.href = currentPage.attr("href");

                $(".page-item").not(currentPage.closest(".page-item")).removeClass("active");
                currentPage.closest(".page-item").addClass("active");

                if (data.totalPages == currentPageNum) {
                    currentPage.closest(".pagination").find(".next").addClass("disabled");
                }

                else {
                    currentPage.closest(".pagination").find(".next").removeClass("disabled");
                }

                if (currentPageNum == 1) {
                    currentPage.closest(".pagination").find(".prev").addClass("disabled");
                }

                else {
                    currentPage.closest(".pagination").find(".prev").removeClass("disabled");
                }


                if (data.products.length == 0) {
                    $(".products-filter-container").append(`<p class="fw-bold col-md-9 no-products-found">No products found</p>`)
                }

                else {

                    $(".no-products-found").remove();
                    $(".product").remove();
                    data.products.forEach(function (product) {
                        $(".products-filter-container").append(`
                        <div class="col-md-3 mb-3 product">
                             <a href="/Products/Index/${product.id}" class="link-dark text-decoration-none">
                                <div class="card">
                                   <img src="/images/products/${product.image.name}${product.image.extension}" class="card-img-top product-img" alt="${product.name} Image" />
                                   <div class="card-body">
                                       <h5 class="card-title">${product.name}</h5>
                                       <p class="card-text">Rating: ${product.rating.toFixed(2)}<br /><br /><span class="text-danger fw-bold fs-6">$${product.price.toFixed(2)}</span></p>
                                   </div>
                                </div>
                            </a>
                        </div>`);
                    });
                }

                $(".products-count").text(data.products.length);


            },
            dataType: "json",
        });
    }
})
