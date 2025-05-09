const images = [
    "./Img/images.jpg",
    "./Img/ads02.jpg",
    "./Img/ads03.jpg"
];

let current = 0;
const slideshow = document.getElementById('ads-img');

function showImage(index) {
    slideshow.src = images[index]; // sửa chỗ này
}

// Gọi lần đầu
showImage(current);

// Đặt interval để chạy mỗi 3 giây
setInterval(() => {
    current++;
    if (current >= images.length) {
        current = 0;
    }
    showImage(current);
}, 3000);


