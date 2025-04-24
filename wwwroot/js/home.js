const images = [
    "url('Img/chup-hinh-my-pham-3.png')",
    "url('Img/images.jpg')",
    "url('Img/Anh-my-pham-3.jpg')"
];

let current = 0;
const slideshow = document.getElementById('home-container');
console.log(slideshow);

function showImage(index) {
    slideshow.style.backgroundImage = images[index];
}

document.getElementById('prev').onclick = () => {
    current = (current - 1 + images.length) % images.length;
    showImage(current);
};

document.getElementById('next').onclick = () => {
    current = (current + 1) % images.length;
    showImage(current);
};

// Initial display
showImage(current);