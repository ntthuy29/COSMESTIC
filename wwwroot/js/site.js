// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function addToCart() {
    var countDiv = document.getElementById("countItemCart");
    var currentCount = parseInt(countDiv.innerText) || 0;  // Lấy số hiện tại, nếu rỗng hoặc NaN thì mặc định 0
    var newCount = currentCount + 1;
    countDiv.innerText = newCount;
}
// Sử dụng Event Delegation để xử lý dynamic content
document.addEventListener('click', function (e) {
    if (e.target.id === 'btnPrice') {
        const min = document.getElementById('min').value;
        const max = document.getElementById('max').value;
        filterByPrice(min ? parseInt(min+"000") : 0, max ? parseInt(max+"000") : Infinity);
    }
});

function filterByPrice(minPrice, maxPrice) {
    const items = document.querySelectorAll('.item');
    items.forEach(item => {
        const price = parseInt(item.id);
        const isInRange = (minPrice === '' || price >= minPrice) &&
            (maxPrice === '' || price <= maxPrice);
        item.style.display = isInRange ? 'block' : 'none';
    });
}