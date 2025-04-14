// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener("DOMContentLoaded", function () {
    const cardForm = "#cardForm"; // get the card form for validation
    const collectionForm = "#collectionForm"; // same with collection form

    if (cardForm) {
        cardForm.addEventListener("submit", function (event) { // when we submit the form get all the form values and check them for null/blank
            const name = document.getElementById("Name").value;
            const SetName = document.getElementById("SetName").value;
            const SetNumber = document.getElementById("SetNumber").value;
            const type = document.getElementById("Type").value;
            const Price = document.getElementById("Price").value;

            if (!name || !SetName || !SetNumber || !type || !Price) {
                event.preventDefault(); // stop user and alert them that they need to fill in all values
                alert("Please fill in all fields.");
            }
        });
    }

    if (collectionForm) { // same as card 
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

    const selectedCards = document.querySelector('select[name="SelectedCards"]'); // get selected cards when we create a collection

    // validate that at least one card is selected for our collection creation
    if (selectedCards && selectedCards.selectedOptions.length === 0) {
        e.preventDefault();
        alert('Please select at least one card for your collection');
        return false;
    }
});

   
