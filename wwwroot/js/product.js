const images = [
    "/Img/images.jpg",
    "/Img/ads05.jpg",
    "/Img/ad04.jpg"
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
