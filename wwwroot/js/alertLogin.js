
// script.js

console.log("ĐÃ NHẤN XÓA");
function alertLogin() {
    document.getElementById("deleteModal").style.display = "block";
    document.body.style.overflow = "hidden";
    document.getElementById("close").onclick = function () {
        document.getElementById("deleteModal").style.display = "none";
        document.body.style.overflow = "auto";

    }
}
