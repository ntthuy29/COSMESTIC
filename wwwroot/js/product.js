const images = [
    "/Img/haski1.jpg",
    "/Img/mungkhaitruong.png",
    "/Img/haski.jpg"
];

let current = 0;
const adsContainer = document.getElementById('ads');

function changeBackgroundImage(index) {
    adsContainer.style.backgroundImage = `url(${images[index]})`; // Set background image
}

// Gọi lần đầu
changeBackgroundImage(current);

// Đặt interval để thay đổi hình nền mỗi 3 giây
setInterval(() => {
    current++;
    if (current >= images.length) {
        current = 0;
    }
    changeBackgroundImage(current);
}, 3000);
const applyButton = document.getElementById('apply-filter');

if (applyButton) {
    applyButton.addEventListener('click', function () {
        console.log("ĐÃ NHẤN ÁP DỤNG GIÁ RỒI");
        const minPrice = parseFloat(document.getElementById('min').value) || 0;
        const maxPrice = parseFloat(document.getElementById('max').value) || Infinity;

        fetch(`/Product/filerProductFollowPrice?min=${minPrice}&max=${maxPrice}&catalogID=@Model[0].catalogID`)
            .then(response => response.text())
            .then(html => {
                document.getElementById('product-list').innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    });
} else {
    console.log("khong click duoc");
}
//đoạn code cho cata =log
