

const blockBtns = document.querySelectorAll(".blockBtn");
const unBlockBtns = document.querySelectorAll(".unBlockBtn");

blockBtns.forEach(BlockBtn => {
    BlockBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");
        console.log(url)
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to Block this card!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, Block this card!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Block!",
                            text: data.message,
                            icon: "success",
                            background: "#131311",
                            color: "#FFFFFF"
                        }).then(result => {
                            location.reload();
                        });
                    });
            }
        });
    });
});


unBlockBtns.forEach(unBlockBtn => {
    unBlockBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to UnBlock this card!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, unBlock this card!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "UnBlock!",
                            text: data.message,
                            icon: "success",
                            background: "#131311",
                            color: "#FFFFFF"
                        }).then(result => {
                            location.reload();
                        });
                    });
            }
        });
    });
});



const loadMoreBtn = document.getElementById("loadMoreBtn");
const newsBox = document.getElementById("newsBox");
const newsCount = document.getElementById("newsCount").value;





let skip = 6
loadMoreBtn.addEventListener("click", function () {
    let url = `/News/LoadMore?skip=${skip}`;

    fetch(url).then(response => response.text()).then(data => newsBox.innerHTML += data)
    skip += 6;
    ``
    if (skip >= newsCount) {
        loadMoreBtn.remove();
    }
})






