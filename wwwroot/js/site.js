// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function addToCart() {
    var countDiv = document.getElementById("countItemCart");
    var currentCount = parseInt(countDiv.innerText) || 0;  // Lấy số hiện tại, nếu rỗng hoặc NaN thì mặc định 0
    var newCount = currentCount + 1;
    countDiv.innerText = newCount;
}
