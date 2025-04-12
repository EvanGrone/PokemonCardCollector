// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener("DOMContentLoaded", function () {
    const cardForm = "#cardForm";
    const collectionForm = "#collectionForm";

    if (cardForm) {
        cardForm.addEventListener("submit", function (event) {
            const name = document.getElementById("Name").value;
            const SetName = document.getElementById("SetName").value;
            const SetNumber = document.getElementById("SetNumber").value;
            const type = document.getElementById("Type").value;
            const Price = document.getElementById("Price").value;

            if (!name || !SetName || !SetNumber || !type || !Price) {
                event.preventDefault();
                alert("Please fill in all fields.");
            }
        });
    }

    if (collectionForm) {
        collectionForm.addEventListener("submit", function (event) {
            const name = document.getElementById("Name").value;
            const Description = document.getElementById("Description").value;
            const CreatedDate = document.getElementById("CreatedDate").value;
            if (!name || !Description || !CreatedDate) {
                event.preventDefault();
                alert("Please fill in all fields.");
            }
        });
    }
});

   
