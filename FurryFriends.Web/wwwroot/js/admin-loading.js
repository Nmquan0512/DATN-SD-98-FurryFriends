// Admin Loading Handler
class AdminLoadingHandler {
    constructor() {
        this.isLoading = false;
        this.loadingTimeout = null;
        this.init();
    }

    init() {
        this.setupNavigationLoading();
        this.setupFormLoading();
        this.setupTableLoading();
        this.setupAjaxLoading();
        this.setupInitialLoading();
    }

    // Xử lý loading ban đầu khi trang load
    setupInitialLoading() {
        // Chỉ hiển thị loading nếu trang chưa load xong
        if (document.readyState !== 'complete') {
            this.showLoadingWithMessage('Đang tải trang...');
            
            // Ẩn loading khi trang đã load xong
            window.addEventListener('load', () => {
                setTimeout(() => {
                    this.hideLoading();
                }, 500); // Giảm thời gian xuống 500ms
            });
        } else {
            // Nếu trang đã load xong, không hiển thị loading
            this.hideLoading();
        }
    }

    // Hiển thị loading khi chuyển trang
    setupNavigationLoading() {
        document.addEventListener('DOMContentLoaded', () => {
            const navLinks = document.querySelectorAll('a[href*="/Admin/"]');
            navLinks.forEach(link => {
                link.addEventListener('click', (e) => {
                    // Chỉ hiển thị loading cho các link không phải là link hiện tại
                    if (link.href !== window.location.href && !link.href.includes('#')) {
                        // Không preventDefault để chuyển trang ngay lập tức
                        this.showLoadingWithMessage('Đang chuyển trang...');
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
                });
            });
        });
    }

    // Hiển thị loading cho table
    setupTableLoading() {
        document.addEventListener('DOMContentLoaded', () => {
            const tables = document.querySelectorAll('.table-container');
            tables.forEach(table => {
                // Chỉ hiển thị loading nếu table chưa có data
                if (table.querySelector('tbody tr').length === 0) {
                    table.classList.add('table-loading', 'loading');
                    
                    // Ẩn loading sau khi table đã load
                    setTimeout(() => {
                        table.classList.remove('loading');
                    }, 1000); // Giảm thời gian xuống 1 giây
                }
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
                    setTimeout(() => {
                        this.hideLoading();
                    }, 300); // Delay nhỏ để tránh nháy
                    return response;
                })
                .catch(error => {
                    setTimeout(() => {
                        this.hideLoading();
                    }, 300);
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
                setTimeout(() => {
                    window.adminLoading.hideLoading();
                }, 300);
            });
            return originalXHROpen.apply(this, arguments);
        };
    }

    // Hiển thị loading
    showLoading() {
        if (this.isLoading) return; // Tránh hiển thị nhiều lần
        
        this.isLoading = true;
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'flex';
            overlay.style.opacity = '0';
            
            // Fade in effect
            setTimeout(() => {
                overlay.style.opacity = '1';
            }, 10);
        }
    }

    // Ẩn loading
    hideLoading() {
        if (!this.isLoading) return; // Tránh ẩn nhiều lần
        
        this.isLoading = false;
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.opacity = '0';
            
            // Fade out effect
            setTimeout(() => {
                overlay.style.display = 'none';
            }, 300);
        }
    }

    // Hiển thị loading với message tùy chỉnh
    showLoadingWithMessage(message = 'Đang tải...') {
        const overlay = document.getElementById('loadingOverlay');
        const textElement = overlay?.querySelector('.loading-text');
        if (overlay && textElement) {
            textElement.textContent = message;
            this.showLoading();
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

// Khởi tạo loading handler khi trang load
document.addEventListener('DOMContentLoaded', () => {
    window.adminLoading = new AdminLoadingHandler();
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