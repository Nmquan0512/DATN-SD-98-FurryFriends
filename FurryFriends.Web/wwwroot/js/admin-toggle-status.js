// Admin Toggle Status Handler
$(document).ready(function() {
    // Xử lý toggle status cho tất cả các nút toggle-status-btn
    $(document).on('click', '.toggle-status-btn', function() {
        var button = $(this);
        var id = button.data('id');
        var currentStatus = button.data('current-status');
        var isActive = currentStatus === '1' || currentStatus === 'true'; // Hỗ trợ cả "1" và "true"
        var controller = button.data('controller');
        var area = button.data('area') || 'Admin';
        
        // Hiển thị loading trên button
        var originalHtml = button.html();
        button.html('<i class="fas fa-spinner fa-spin"></i>');
        button.prop('disabled', true);

        $.ajax({
            url: '/' + area + '/' + controller + '/ToggleStatus',
            type: 'POST',
            data: { id: id },
            success: function(response) {
                if (response.success) {
                    // Cập nhật trạng thái trong bảng
                    var row = button.closest('tr');
                    var statusCell = row.find('.status-cell');
                    var statusBadge = statusCell.find('.badge');
                    
                    // Cập nhật badge
                    statusBadge.removeClass('bg-success bg-secondary').addClass(response.statusClass);
                    statusBadge.text(response.statusText);
                    
                    // Cập nhật button
                    button.data('current-status', response.newStatus ? '1' : '2');
                    button.attr('title', response.newStatus ? 'Vô hiệu hóa' : 'Kích hoạt');
                    
                    // Cập nhật icon dựa trên trạng thái mới
                    var newIcon = response.newStatus ? 'fas fa-ban' : 'fas fa-check';
                    button.find('i').removeClass('fas fa-ban fas fa-check').addClass(newIcon);
                    
                    // Hiển thị thông báo thành công
                    showNotification(response.message, 'success');
                    
                    // Cập nhật số liệu thống kê nếu có
                    updateStatistics();
                } else {
                    showNotification(response.message, 'error');
                }
            },
            error: function() {
                showNotification('Có lỗi xảy ra khi cập nhật trạng thái!', 'error');
            },
            complete: function() {
                // Khôi phục button
                button.html(originalHtml);
                button.prop('disabled', false);
            }
        });
    });

    function showNotification(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        var alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
                       message +
                       '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                       '</div>';
        
        // Thêm thông báo vào đầu container
        $('.container-fluid').prepend(alertHtml);
        
        // Tự động ẩn sau 3 giây
        setTimeout(function() {
            $('.alert').fadeOut();
        }, 3000);
    }

    function updateStatistics() {
        // Cập nhật số liệu thống kê bằng cách reload trang
        location.reload();
    }
});
