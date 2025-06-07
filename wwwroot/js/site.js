// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function addToCart() {
    var countDiv = document.getElementById("countItemCart");
    var currentCount = parseInt(countDiv.innerText) || 0; 
    var newCount = currentCount + 1;
    countDiv.innerText = newCount;
}

document.addEventListener('click', function (e) {
    if (e.target.id === 'apply') {
        console.log("hihi");
        const min = document.getElementById('min').value;
        const max = document.getElementById('max').value;
        filterByApply(min ? parseInt(min+"000") : 0, max ? parseInt(max+"000") : Infinity);
    }
});

function filterByApply(minPrice, maxPrice) {
    const item = document.querySelectorAll('.item');
    console.log("ddang tim gia");
    console.log(item);
    item.forEach(item => {
        const price = parseInt(item.id);
        const isInRange = (minPrice === '' || price >= minPrice) &&
            (maxPrice === '' || price <= maxPrice);
            console.log("ddax ap ding")
        item.style.display = isInRange ? 'block' : 'none';
    });
}