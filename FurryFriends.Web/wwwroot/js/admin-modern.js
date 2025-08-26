// Modern Admin JavaScript with Enhanced Features

class ModernAdmin {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.initializeAnimations();
        this.setupCharts();

        this.setupThemeToggle();
        this.setupSearch();
        this.setupKeyboardShortcuts();
        this.setupScrollPosition(); // Thêm chức năng lưu vị trí cuộn
    }

    setupScrollPosition() {
        // Tạm thời vô hiệu hóa để tránh xung đột với admin-scroll-fix.js
        // Logic scroll đã được xử lý trong admin-scroll-fix.js
        console.log('ModernAdmin scroll position disabled - using admin-scroll-fix.js instead');
    }

    setupEventListeners() {
        // Sidebar toggle
        document.getElementById('sidebarToggle')?.addEventListener('click', () => {
            this.toggleSidebar();
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', (e) => {
            const sidebar = document.getElementById('sidebar');
            const toggle = document.getElementById('sidebarToggle');
            
            if (window.innerWidth <= 768 && 
                sidebar?.classList.contains('show') && 
                !sidebar.contains(e.target) && 
                !toggle?.contains(e.target)) {
                this.toggleSidebar();
            }
        });

        // Smooth scrolling for anchor links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', (e) => {
                e.preventDefault();
                const target = document.querySelector(anchor.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });

        // Add loading states to buttons
        document.querySelectorAll('.btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                if (!btn.classList.contains('btn-loading')) {
                    this.addLoadingState(btn);
                }
            });
        });
    }

    initializeAnimations() {
        // Intersection Observer for scroll animations
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, observerOptions);

        // Observe elements for animation
        document.querySelectorAll('.dashboard-card, .chart-card, .stat-card').forEach(el => {
            observer.observe(el);
        });

        // Parallax effect for dashboard header
        window.addEventListener('scroll', () => {
            const header = document.querySelector('.dashboard-header');
            if (header) {
                const scrolled = window.pageYOffset;
                const rate = scrolled * -0.5;
                header.style.transform = `translateY(${rate}px)`;
            }
        });
    }

    setupCharts() {
        // Initialize Chart.js with custom options
        if (typeof Chart !== 'undefined') {
            Chart.defaults.font.family = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
            Chart.defaults.color = '#333';
            Chart.defaults.plugins.legend.labels.usePointStyle = true;
            Chart.defaults.plugins.legend.labels.padding = 20;
            
            // Custom animation duration
            Chart.defaults.animation.duration = 2000;
            Chart.defaults.animation.easing = 'easeInOutQuart';
        }
    }



    setupThemeToggle() {
        // Create theme toggle button
        const themeToggle = document.createElement('button');
        themeToggle.className = 'theme-toggle';
        themeToggle.innerHTML = '<i class="fas fa-moon"></i>';
        themeToggle.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            width: 50px;
            height: 50px;
            border-radius: 50%;
            background: linear-gradient(135deg, #667eea, #764ba2);
            color: white;
            border: none;
            cursor: pointer;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
            z-index: 1000;
        `;

        themeToggle.addEventListener('click', () => {
            this.toggleTheme();
        });

        themeToggle.addEventListener('mouseenter', () => {
            themeToggle.style.transform = 'scale(1.1)';
        });

        themeToggle.addEventListener('mouseleave', () => {
            themeToggle.style.transform = 'scale(1)';
        });

        document.body.appendChild(themeToggle);
    }

    toggleTheme() {
        const body = document.body;
        const isDark = body.classList.contains('dark-theme');
        
        if (isDark) {
            body.classList.remove('dark-theme');
            localStorage.setItem('theme', 'light');
    
        } else {
            body.classList.add('dark-theme');
            localStorage.setItem('theme', 'dark');
    
        }
    }

    setupSearch() {
        const searchBar = document.querySelector('.search-bar');
        if (searchBar) {
            let searchTimeout;

            searchBar.addEventListener('input', (e) => {
                clearTimeout(searchTimeout);
                const query = e.target.value.trim();

                searchTimeout = setTimeout(() => {
                    if (query.length > 2) {
                        this.performSearch(query);
                    }
                }, 300);
            });

            // Add search suggestions
            searchBar.addEventListener('focus', () => {
                this.showSearchSuggestions();
            });
        }
    }

    performSearch(query) {
        // Implement search functionality
        console.log('Searching for:', query);

    }

    showSearchSuggestions() {
        // Create search suggestions dropdown
        const suggestions = [
            'Quản lý sản phẩm',
            'Quản lý đơn hàng',
            'Quản lý khách hàng',
            'Báo cáo doanh thu',
            'Cài đặt hệ thống'
        ];

        const dropdown = document.createElement('div');
        dropdown.className = 'search-suggestions';
        dropdown.style.cssText = `
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            margin-top: 5px;
            z-index: 1000;
            max-height: 200px;
            overflow-y: auto;
        `;

        suggestions.forEach(suggestion => {
            const item = document.createElement('div');
            item.className = 'suggestion-item';
            item.textContent = suggestion;
            item.style.cssText = `
                padding: 10px 15px;
                cursor: pointer;
                transition: background-color 0.2s ease;
            `;

            item.addEventListener('mouseenter', () => {
                item.style.backgroundColor = '#f8f9ff';
            });

            item.addEventListener('mouseleave', () => {
                item.style.backgroundColor = 'transparent';
            });

            item.addEventListener('click', () => {
                document.querySelector('.search-bar').value = suggestion;
                dropdown.remove();
            });

            dropdown.appendChild(item);
        });

        const searchContainer = document.querySelector('.search-container');
        if (searchContainer) {
            searchContainer.appendChild(dropdown);

            // Remove suggestions when clicking outside
            document.addEventListener('click', (e) => {
                if (!searchContainer.contains(e.target)) {
                    dropdown.remove();
                }
            });
        }
    }

    setupKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + K for search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                document.querySelector('.search-bar')?.focus();
            }

            // Ctrl/Cmd + B for sidebar toggle
            if ((e.ctrlKey || e.metaKey) && e.key === 'b') {
                e.preventDefault();
                this.toggleSidebar();
            }

            // Escape to close modals/sidebar
            if (e.key === 'Escape') {
                const sidebar = document.getElementById('sidebar');
                if (sidebar?.classList.contains('show')) {
                    this.toggleSidebar();
                }
            }
        });
    }

    toggleSidebar() {
        const sidebar = document.getElementById('sidebar');
        const mainContent = document.getElementById('mainContent');
        
        if (sidebar && mainContent) {
            sidebar.classList.toggle('show');
            mainContent.classList.toggle('expanded');
        }
    }

    addLoadingState(button) {
        const originalText = button.innerHTML;
        const originalWidth = button.offsetWidth;

        button.classList.add('btn-loading');
        button.style.width = originalWidth + 'px';
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';
        button.disabled = true;

        // Remove loading state after 2 seconds (simulate processing)
        setTimeout(() => {
            button.classList.remove('btn-loading');
            button.innerHTML = originalText;
            button.disabled = false;
            button.style.width = '';
        }, 2000);
    }

    // Utility methods
    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    throttle(func, limit) {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.modernAdmin = new ModernAdmin();
    
    // Show welcome notification
    setTimeout(() => {

    }, 1000);
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ModernAdmin;
} 