$(document).ready(function () {
    const bell = $('.fa-bell');
    const badge = bell.siblings('.badge');
    let dropdown = null;
    let notifications = [];

    // 🔹 Lấy danh sách thông báo từ API
    function fetchNotifications() {
        $.get('https://localhost:7289/api/ThongBao', function (data) {
            notifications = data.filter(n => !n.daDoc);
            updateBadge();
        });
    }

    // 🔹 Cập nhật số lượng trên badge
    function updateBadge() {
        if (notifications.length > 0) {
            badge.text(notifications.length).show();
        } else {
            badge.hide();
        }
    }

    // 🔹 Hiển thị danh sách khi click bell
    function showDropdown() {
        if (dropdown) dropdown.remove();

        dropdown = $('<div class="notification-dropdown card shadow" style="position:absolute; right:0; top:40px; min-width:320px; z-index:2000;"></div>');
        let html = '<div class="card-body p-2">';

        if (notifications.length === 0) {
            html += '<div class="text-center text-muted py-3">Không có thông báo mới</div>';
        } else {
            notifications.forEach(n => {
                html += `<div class="d-flex align-items-start gap-2 mb-2 notification-item" data-id="${n.thongBaoId}">
                    <i class="bi bi-info-circle-fill text-primary mt-1"></i>
                    <div>
                        <div class="fw-semibold">${n.noiDung}</div>
                        <div class="small text-secondary">${new Date(n.ngayTao).toLocaleString('vi-VN')}</div>
                    </div>
                    <button class="btn btn-link btn-sm ms-auto mark-read" title="Đánh dấu đã đọc"><i class="bi bi-check2-circle"></i></button>
                </div>`;
            });
        }

        html += '</div>';
        dropdown.html(html);
        bell.parent().append(dropdown);
    }

    // 🔹 Event: click chuông
    bell.on('click', function (e) {
        e.stopPropagation();
        showDropdown();
    });

    // 🔹 Click ngoài thì đóng dropdown
    $(document).on('click', function () {
        if (dropdown) dropdown.remove();
    });

    // 🔹 Đánh dấu đã đọc
    $(document).on('click', '.mark-read', function (e) {
        e.stopPropagation();
        const id = $(this).closest('.notification-item').data('id');
        $.post(`https://localhost:7289/api/ThongBao/mark-as-read/${id}`, function () {
            notifications = notifications.filter(n => n.thongBaoId !== id);
            updateBadge();
            showDropdown();
        });
    });

    // 🔹 Load ban đầu + polling
    fetchNotifications();
    setInterval(fetchNotifications, 10000);
});
