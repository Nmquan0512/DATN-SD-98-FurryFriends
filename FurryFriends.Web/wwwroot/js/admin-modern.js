// Modern Admin JavaScript with Enhanced Features

class ModernAdmin {
    constructor() {
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.initializeAnimations();
        this.setupCharts();
        this.setupNotifications();
        this.setupThemeToggle();
        this.setupSearch();
        this.setupKeyboardShortcuts();
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

    setupNotifications() {
        // Create notification system
        this.notificationContainer = document.createElement('div');
        this.notificationContainer.className = 'notification-container';
        this.notificationContainer.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            display: flex;
            flex-direction: column;
            gap: 10px;
        `;
        document.body.appendChild(this.notificationContainer);
    }

    showNotification(message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.style.cssText = `
            background: ${this.getNotificationColor(type)};
            color: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            transform: translateX(100%);
            transition: transform 0.3s ease;
            max-width: 300px;
            word-wrap: break-word;
        `;
        
        notification.innerHTML = `
            <div style="display: flex; align-items: center; gap: 10px;">
                <i class="fas ${this.getNotificationIcon(type)}"></i>
                <span>${message}</span>
            </div>
        `;

        this.notificationContainer.appendChild(notification);

        // Animate in
        setTimeout(() => {
            notification.style.transform = 'translateX(0)';
        }, 100);

        // Auto remove
        setTimeout(() => {
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, duration);

        // Click to dismiss
        notification.addEventListener('click', () => {
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        });
    }

    getNotificationColor(type) {
        const colors = {
            success: 'linear-gradient(135deg, #43e97b, #38f9d7)',
            error: 'linear-gradient(135deg, #f093fb, #f5576c)',
            warning: 'linear-gradient(135deg, #fa709a, #fee140)',
            info: 'linear-gradient(135deg, #4facfe, #00f2fe)'
        };
        return colors[type] || colors.info;
    }

    getNotificationIcon(type) {
        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };
        return icons[type] || icons.info;
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
            this.showNotification('Chuyển sang chế độ sáng', 'info');
        } else {
            body.classList.add('dark-theme');
            localStorage.setItem('theme', 'dark');
            this.showNotification('Chuyển sang chế độ tối', 'info');
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
        this.showNotification(`Đang tìm kiếm: ${query}`, 'info');
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
        window.modernAdmin.showNotification('Chào mừng đến với Furry Friends Admin!', 'success');
    }, 1000);
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ModernAdmin;
} 