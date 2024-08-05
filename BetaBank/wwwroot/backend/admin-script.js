

const banBtns = document.querySelectorAll(".banBtn");
const unBanBtns = document.querySelectorAll(".unBanBtn");
const newsDeleteBtns = document.querySelectorAll(".newsDeleteBtn");

const disableBtns = document.querySelectorAll(".disableBtn");
const unDisableBtns = document.querySelectorAll(".unDisableBtn");

const suspendBtns = document.querySelectorAll(".SuspendBtn");
const unSuspendBtns = document.querySelectorAll(".UnSuspendBtn");

banBtns.forEach(BanBtn => {
    BanBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");
        console.log(url)
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to Ban this user!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, Ban this user!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Ban!",
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


disableBtns.forEach(DisableBtn => {
    DisableBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");
        console.log(url)
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to Disable this card!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, Disable this card!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Disable!",
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


unDisableBtns.forEach(unDisableBtn => {
    unDisableBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to unDisable this card!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, unDisable this card!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "UnDisable!",
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


suspendBtns.forEach(SuspendBtn => {
    SuspendBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");
        console.log(url)
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to Suspend this Account!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, Suspend this account!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "Suspend!",
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


unSuspendBtns.forEach(unSuspendBtn => {
    unSuspendBtn.addEventListener("click", function (e) {
        e.preventDefault();

        let url = this.getAttribute("href");

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to unSuspend this Account!",
            icon: "warning",
            background: "#131311",
            color: "#FFFFFF",
            showCancelButton: true,
            confirmButtonColor: "#74FA56",
            cancelButtonColor: "#1E1D1B",
            confirmButtonText: "Yes, unSuspend this Account!"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url, { method: "POST" })
                    .then(response => response.json())
                    .then(data => {
                        Swal.fire({
                            title: "UnSuspend!",
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