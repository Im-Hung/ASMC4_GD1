// ================================================
// UPDATE CART COUNT FUNCTION
// ================================================

async function updateCartCount() {
    try {
        const response = await fetch('/Cart/CartCountValue');
        const count = await response.text();
        
        // C?p nh?t t?t c? element có class cart-count
        const cartCountElements = document.querySelectorAll('.cart-count, [data-cart-count]');
        cartCountElements.forEach(el => {
            el.textContent = count;
            el.style.display = count !== '0' ? 'inline-block' : 'none';
        });
        
        console.log(`?? Cart count updated: ${count}`);
    } catch (error) {
        console.error('? Error updating cart count:', error);
    }
}

// G?i khi trang load
document.addEventListener('DOMContentLoaded', function () {
    updateCartCount();
});
