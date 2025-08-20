// Admin Sidebar Scroll Fix - Simple Version
// Giải quyết vấn đề sidebar scroll khi click menu

class AdminScrollFix {
    constructor() {
        this.init();
    }

    init() {
        this.setupSidebar();
        this.setupClickHandlers();
        this.setupActiveHighlight();
    }

    setupSidebar() {
        const sidebar = document.querySelector('.sidebar-nav');
        if (sidebar) {
            // Đảm bảo sidebar có thể scroll
            sidebar.style.overflowY = 'auto';
            sidebar.style.overflowX = 'hidden';
            sidebar.style.flex = '1';
            sidebar.style.minHeight = '0';
            
            console.log('Sidebar setup completed');
        }
    }

    setupClickHandlers() {
        // Xử lý click cho tất cả nav-link
        document.addEventListener('click', (e) => {
            const navLink = e.target.closest('.nav-link');
            if (navLink) {
                this.handleNavClick(navLink);
            }
        });
    }

    handleNavClick(navLink) {
        console.log('Nav clicked:', navLink.textContent.trim());
        
        // Set active cho link được click
        this.setActiveLink(navLink);
        
        // Scroll đến link được click
        if (!navLink.href.includes('Dashboard')) {
            this.scrollToLink(navLink);
        } else {
            this.scrollToTop();
        }
    }

    setActiveLink(activeLink) {
        // Xóa tất cả active
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('active');
        });
        
        // Thêm active cho link được chọn
        activeLink.classList.add('active');
        console.log('Set active:', activeLink.textContent.trim());
    }

    scrollToLink(link) {
        const sidebar = document.querySelector('.sidebar-nav');
        if (!sidebar || !link) return;

        // Tính toán vị trí scroll
        const linkOffsetTop = link.offsetTop;
        const sidebarHeight = sidebar.clientHeight;
        const linkHeight = link.offsetHeight;
        
        // Đặt link ở giữa sidebar
        const targetScrollTop = linkOffsetTop - (sidebarHeight / 2) + (linkHeight / 2);
        
        console.log('Scrolling to:', {
            link: link.textContent.trim(),
            offsetTop: linkOffsetTop,
            sidebarHeight: sidebarHeight,
            targetScrollTop: targetScrollTop
        });
        
        // Scroll đến vị trí
        sidebar.scrollTop = Math.max(0, targetScrollTop);
    }

    scrollToTop() {
        const sidebar = document.querySelector('.sidebar-nav');
        if (sidebar) {
            console.log('Scrolling to top');
            sidebar.scrollTop = 0;
        }
    }

    setupActiveHighlight() {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.nav-link');
        
        navLinks.forEach(link => {
            if (this.isLinkMatchingPath(link, currentPath)) {
                this.setActiveLink(link);
                
                // Scroll đến link active nếu không phải Dashboard
                if (!this.shouldScrollToTop()) {
                    setTimeout(() => {
                        this.scrollToLink(link);
                    }, 100);
                }
            }
        });
        
        // Fallback cho Dashboard
        if (this.shouldScrollToTop()) {
            const dashboardLink = document.querySelector('a[href*="Dashboard"]');
            if (dashboardLink) {
                this.setActiveLink(dashboardLink);
                this.scrollToTop();
            }
        }
    }

    isLinkMatchingPath(link, currentPath) {
        if (!link.href) return false;
        
        try {
            const linkUrl = new URL(link.href);
            const linkPath = linkUrl.pathname;
            
            // Exact match
            if (linkPath === currentPath) return true;
            
            // Controller-based matching
            const pathParts = currentPath.split('/').filter(part => part);
            const linkParts = linkPath.split('/').filter(part => part);
            
            // Match controller name
            if (pathParts.length >= 2 && linkParts.length >= 2) {
                return pathParts[1] === linkParts[1];
            }
            
            return false;
        } catch (error) {
            return false;
        }
    }

    shouldScrollToTop() {
        const currentPath = window.location.pathname;
        return currentPath === '/Admin' || currentPath === '/Admin/' || 
               currentPath.includes('/Admin/Dashboard') || currentPath.includes('/Admin/Home');
    }
}

// Khởi tạo khi DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.adminScrollFix = new AdminScrollFix();
    });
} else {
    window.adminScrollFix = new AdminScrollFix();
}

// Backup initialization
window.addEventListener('load', () => {
    if (!window.adminScrollFix) {
        window.adminScrollFix = new AdminScrollFix();
    }
});
