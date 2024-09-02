let addBasketBtns = document.querySelectorAll(".add-to-basket");

addBasketBtns.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = btn.getAttribute("href");

        fetch(url)
            .then(response => {
                if (response.status == 200) {
                    alert("Added to basket")
                } else {
                    alert("Bele bir kitab yoxdur")
                }
            })
    })
})