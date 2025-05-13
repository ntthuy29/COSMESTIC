console.log("catalog");
document.addEventListener('DOMContentLoaded', function () {
    const filterPrice = document.getElementById('btnPrice');
    console.log(filterPrice);
    const itemPrice = document.querySelectorAll('.item');
    console.log(itemPrice);
    const min = document.getElementById('min');
    console.log(min);
    const max = document.getElementById('max');
    console.log(max);
    filterPrice.addEventListener('click',
        () => {
            const minVal = parseInt(min.value + "000");
            console.log(minVal);
            const maxVal = parseInt(max.value + "000");
            console.log(maxVal);
            itemPrice.forEach(item => {
                const price = parseInt(item.id);
                console.log(price);

                if (price >= minVal && price <= maxVal) {
                    item.style.display = 'block';
                } else {
                    item.style.display = 'none';
                }
            });
        }
    )

});