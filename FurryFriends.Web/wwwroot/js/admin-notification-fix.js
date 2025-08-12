// Sửa thông báo đăng nhập hiện liên tục
document.addEventListener('DOMContentLoaded', function() {
    // Tìm và ẩn thông báo "Đăng nhập thành công" sau 3 giây
    const loginNotification = document.querySelector('.alert-success, .alert-info');
    if (loginNotification && loginNotification.textContent.includes('Đăng nhập thành công')) {
        setTimeout(() => {
            loginNotification.style.display = 'none';
        }, 3000);
    }
    
    // Tự động ẩn tất cả thông báo sau 5 giây
    const allAlerts = document.querySelectorAll('.alert');
    allAlerts.forEach(alert => {
        setTimeout(() => {
            if (alert.style.display !== 'none') {
                alert.style.display = 'none';
            }
        }, 5000);
    });
});

// Hàm ẩn thông báo thủ công
function hideNotification(alertElement) {
    if (alertElement) {
        alertElement.style.display = 'none';
    }
}
