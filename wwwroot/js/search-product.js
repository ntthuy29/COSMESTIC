document.querySelector('#search-product-home').addEventListener('input', function () {
    let query = this.value;

    // Kiểm tra nếu query có ít nhất 2 ký tự
    if (query.length >= 1) {
        fetch(`/Product/Search?query=${encodeURIComponent(query)}`)
            .then(response => response.text()) // Nhận dữ liệu từ server dưới dạng HTML
            .then(html => {
                if (html.trim() === "") {
                    document.querySelector('#main').innerHTML = '<p class="text-danger">Không tìm thấy kết quả.</p>';
                } else {
                    document.querySelector('#main').innerHTML = html; // Chèn kết quả vào phần tử #main
                }
            })
            .catch(() => {
                document.querySelector('#main').innerHTML = '<p class="text-danger">Đã xảy ra lỗi.</p>';
            });
    } else {
        // Nếu ô input trống, load lại trang ban đầu
        window.location.href = '/';  // Hoặc sử dụng URL phù hợp để quay lại trang chủ hoặc trạng thái ban đầu
    }
});
