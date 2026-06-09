document.getElementById("editBtn")
    .addEventListener("click", function () {

        document
            .querySelectorAll(".profile-input")
            .forEach(input => {
                input.removeAttribute("readonly");
            });

        this.classList.add("d-none");

        document
            .getElementById("saveBtn")
            .classList.remove("d-none");
    });