document.querySelector('#search-product-home').addEventListener('input', function () {
    let query = this.value;

    if (query.length >= 1) {
        fetch(`/Product/Search?query=${encodeURIComponent(query)}`)
            .then(response => response.text()) 
            .then(html => {
                if (html.trim() === "") {
                    document.querySelector('#main').innerHTML = '<p class="text-danger">Không tìm thấy kết quả.</p>';
                } else {
                    document.querySelector('#main').innerHTML = html; 
                }
            })
            .catch(() => {
                document.querySelector('#main').innerHTML = '<p class="text-danger">Đã xảy ra lỗi.</p>';
            });
    } else {

        window.location.href = '/';  
    }
});
function loadProducts(catalogId) {
    console.log(catalogId);  

    fetch(`/Product/GetByCatalog/${catalogId}`)
        .then(response => response.text())  
        .then(html => {
            console.log(html);  

            let productList = document.getElementById('main');

            productList.innerHTML = html; 

        })
        .catch(error => console.error('Error:', error));  
}


