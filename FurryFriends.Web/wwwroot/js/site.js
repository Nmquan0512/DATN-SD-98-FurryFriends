const mobileMenuButton = document.getElementById('mobile-menu-button');
const mobileMenu = document.getElementById('mobile-menu');
const mobileMenuBackdrop = document.getElementById('mobile-menu-backdrop');
const hamburger = document.getElementById('hamburger');
const header = document.querySelector('header');
const openLoginBtn = document.getElementById('open-login');
const openCartBtn = document.getElementById('open-cart');
// Modal Elements

const signupModal = document.getElementById('signup-modal');

const openLoginBtnmb = document.getElementById('open-login-mobile');
const openCartBtnmb = document.getElementById('open-cart-mobile');
// Cart State

openLoginBtnmb?.addEventListener('click', () => {
    window.location.href = 'auth.html';  // chuyển sang trang auth.html
});

openCartBtnmb?.addEventListener('click', () => {
    window.location.href = 'cart.html';  // chuyển sang trang cart.html
});
// Cart State
let cartItems = [];
let cartCount = 0;

// Mobile Menu Functions
function toggleMobileMenu() {
    if (!mobileMenuButton || !mobileMenu || !mobileMenuBackdrop) return;

    const isOpen = mobileMenu.classList.contains('open');

    if (!isOpen) {
        mobileMenu.classList.add('open');
        mobileMenuBackdrop.classList.add('open');
        hamburger.classList.add('open');
        document.body.style.overflow = 'hidden';
    } else {
        closeMobileMenu();
    }
}

function closeMobileMenu() {
    if (mobileMenu && mobileMenuBackdrop && hamburger) {
        mobileMenu.classList.remove('open');
        mobileMenuBackdrop.classList.remove('open');
        hamburger.classList.remove('open');
        document.body.style.overflow = '';
    }
}

function initializeMobileMenu() {
    if (!mobileMenuButton || !mobileMenu) return;

    mobileMenuButton.addEventListener('click', (e) => {
        e.stopPropagation();
        toggleMobileMenu();
    });

    mobileMenuBackdrop?.addEventListener('click', closeMobileMenu);

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeMobileMenu();
        }
    });

    const mobileMenuLinks = mobileMenu.querySelectorAll('a');
    mobileMenuLinks.forEach(link => {
        link.addEventListener('click', closeMobileMenu);
    });
}

// Helper Functions
function updateCartBadge() {
    const cartBadges = document.querySelectorAll('.cart-badge span');
    cartBadges.forEach(badge => {
        badge.textContent = cartCount.toString();
    });
}

function showSuccessMessage(message) {
    const toast = document.createElement('div');
    toast.className = 'fixed top-4 right-4 bg-black text-white px-6 py-3 rounded-lg z-50';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
    }, 2000);
}

// Modal Functions
function openModal(modal) {
    modal?.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
}

function closeModal(modal) {
    modal?.classList.add('hidden');
    document.body.style.overflow = '';
}

function closeAllModals() {
    [loginModal, signupModal, cartModal].forEach(closeModal);
}

// DOM Ready
document.addEventListener('DOMContentLoaded', () => {
    initializeMobileMenu();
    updateCartBadge();

    // Header scroll effect
    window.addEventListener('scroll', () => {
        if (window.scrollY > 0) {
            header?.classList.add('bg-white', 'shadow-md');
            header?.classList.remove('glass');
        } else {
            header?.classList.remove('bg-white', 'shadow-md');
            header?.classList.add('glass');
        }
    });



    openLoginBtn?.addEventListener('click', () => {
        window.location.href = 'auth.html';  // chuyển sang trang auth.html
    });

    openCartBtn?.addEventListener('click', () => {
        window.location.href = 'cart.html';  // chuyển sang trang cart.html
    });


    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeAllModals();
        }
    });

    // Product Interactions
    document.querySelectorAll('.product-card').forEach(card => {
        const addToCartBtn = card.querySelector('.btn-primary');
        const wishlistBtn = card.querySelector('.fa-heart');

        addToCartBtn?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            cartCount++;
            updateCartBadge();
            showSuccessMessage('Đã thêm vào giỏ hàng!');
        });

        wishlistBtn?.parentElement.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            wishlistBtn.classList.toggle('far');
            wishlistBtn.classList.toggle('fas');
            wishlistBtn.classList.toggle('text-red-500');
            showSuccessMessage(
                wishlistBtn.classList.contains('fas')
                    ? 'Đã thêm vào danh sách yêu thích!'
                    : 'Đã xóa khỏi danh sách yêu thích!'
            );
        });
    });





    // Form Submissions
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            const submitBtn = form.querySelector('button[type="submit"]');
            if (!submitBtn) return;

            const originalText = submitBtn.textContent;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<div class="loading-spinner mx-auto"></div>';

            setTimeout(() => {
                submitBtn.disabled = false;
                submitBtn.textContent = originalText;
                showSuccessMessage('Thành công!');
                form.reset();

                const modal = form.closest('.modal');
                if (modal) {
                    setTimeout(() => closeModal(modal), 1000);
                }
            }, 1500);
        });
    });
});
