

const banBtns = document.querySelectorAll(".banBtn");
const unBanBtns = document.querySelectorAll(".unBanBtn");
const newsDeleteBtns = document.querySelectorAll(".newsDeleteBtn");

banBtns.forEach(banBtn => {
    banBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to ban this user!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, Ban this user!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Banned!",
                            text: data.message,
                            icon: "success"
                        }).then(result => {
                            location.reload();
                        });
                    })

            }
        });
    })
})


unBanBtns.forEach(unBanBtn => {
    unBanBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to unBan this user!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, unBan this user!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "UnBan!",
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



newsDeleteBtns.forEach(newsDeleteBtn => {
    newsDeleteBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to delete this news!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, delete this news!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Deleted!",
                            text: data.message,
                            icon: "success",
                            background: "#131311",
                            color: "#FFFFFF"
                        }).then(result => {
                            location.reload();
                        });
                    })
            }
        });
    });
});
