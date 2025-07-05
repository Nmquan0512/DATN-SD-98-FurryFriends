// Admin Loading Handler
class AdminLoadingHandler {
    constructor() {
        this.init();
    }

    init() {
        this.setupNavigationLoading();
        this.setupFormLoading();
        this.setupTableLoading();
        this.setupAjaxLoading();
    }

    // Hiển thị loading khi chuyển trang
    setupNavigationLoading() {
        document.addEventListener('DOMContentLoaded', () => {
            const navLinks = document.querySelectorAll('a[href*="/Admin/"]');
            navLinks.forEach(link => {
                link.addEventListener('click', (e) => {
                    // Chỉ hiển thị loading cho các link không phải là link hiện tại
                    if (link.href !== window.location.href && !link.href.includes('#')) {
                        e.preventDefault(); // Ngăn chuyển trang ngay lập tức
                        this.showLoadingWithMessage('Đang chuyển trang...');
                        
                        // Chuyển trang sau 2 giây để loading hiển thị rõ
                        setTimeout(() => {
                            window.location.href = link.href;
                        }, 2000);
                    }
                });
            });
        });
    }

    // Hiển thị loading khi submit form
    setupFormLoading() {
        document.addEventListener('DOMContentLoaded', () => {
            const forms = document.querySelectorAll('form');
            forms.forEach(form => {
                form.addEventListener('submit', (e) => {
                    // Kiểm tra nếu form có validation errors thì không hiển thị loading
                    if (!form.checkValidity()) {
                        return;
                    }
                    
                    this.showLoadingWithMessage('Đang xử lý...');
                    // Đảm bảo loading hiển thị ít nhất 2 giây
                    setTimeout(() => {
                        // Loading sẽ được ẩn bởi page redirect hoặc AJAX response
                    }, 2000);
                });
            });
        });
    }

    // Hiển thị loading cho table
    setupTableLoading() {
        document.addEventListener('DOMContentLoaded', () => {
            const tables = document.querySelectorAll('.table-container');
            tables.forEach(table => {
                table.classList.add('table-loading', 'loading');
                
                // Ẩn loading sau khi table đã load
                setTimeout(() => {
                    table.classList.remove('loading');
                }, 2000);
            });
        });
    }

    // Xử lý loading cho AJAX requests
    setupAjaxLoading() {
        // Intercept fetch requests
        const originalFetch = window.fetch;
        window.fetch = (...args) => {
            this.showLoading();
            return originalFetch(...args)
                .then(response => {
                    this.hideLoading();
                    return response;
                })
                .catch(error => {
                    this.hideLoading();
                    throw error;
                });
        };

        // Intercept XMLHttpRequest
        const originalXHROpen = XMLHttpRequest.prototype.open;
        const originalXHRSend = XMLHttpRequest.prototype.send;
        
        XMLHttpRequest.prototype.open = function() {
            this.addEventListener('loadstart', () => {
                window.adminLoading.showLoading();
            });
            this.addEventListener('loadend', () => {
                window.adminLoading.hideLoading();
            });
            return originalXHROpen.apply(this, arguments);
        };
    }

    // Hiển thị loading
    showLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'flex';
        }
    }

    // Ẩn loading
    hideLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    // Hiển thị loading với message tùy chỉnh
    showLoadingWithMessage(message = 'Đang tải...') {
        const overlay = document.getElementById('loadingOverlay');
        const textElement = overlay?.querySelector('.loading-text');
        if (overlay && textElement) {
            textElement.textContent = message;
            overlay.style.display = 'flex';
        }
    }

    // Hiển thị loading cho table cụ thể
    showTableLoading(tableSelector) {
        const table = document.querySelector(tableSelector);
        if (table) {
            table.classList.add('loading');
        }
    }

    // Ẩn loading cho table cụ thể
    hideTableLoading(tableSelector) {
        const table = document.querySelector(tableSelector);
        if (table) {
            table.classList.remove('loading');
        }
    }
}

// Hiển thị loading ngay khi script được load
(function() {
    const overlay = document.getElementById('loadingOverlay');
    const textElement = overlay?.querySelector('.loading-text');
    if (overlay && textElement) {
        textElement.textContent = 'Đang tải trang...';
        overlay.style.display = 'flex';
    }
})();

// Khởi tạo loading handler khi trang load
document.addEventListener('DOMContentLoaded', () => {
    window.adminLoading = new AdminLoadingHandler();
    
    // Đảm bảo loading hiển thị
    window.adminLoading.showLoadingWithMessage('Đang tải trang...');
    
    // Luôn chờ đủ 2 giây trước khi ẩn loading, bất kể trang đã load xong chưa
    setTimeout(() => {
        window.adminLoading.hideLoading();
    }, 2000);

    // Ẩn loading sau 15 giây nếu có lỗi
    setTimeout(() => {
        window.adminLoading.hideLoading();
    }, 15000);
});

// Global functions để sử dụng từ bên ngoài
window.showLoading = function(message) {
    if (window.adminLoading) {
        window.adminLoading.showLoadingWithMessage(message || 'Đang tải...');
    }
};

window.hideLoading = function() {
    if (window.adminLoading) {
        window.adminLoading.hideLoading();
    }
};

// Export cho các module khác sử dụng
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AdminLoadingHandler;
} 