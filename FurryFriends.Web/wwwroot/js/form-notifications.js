// Form Notifications Handler
document.addEventListener('DOMContentLoaded', function() {
    // Xử lý tất cả các form có class 'form-with-notifications'
    const forms = document.querySelectorAll('form[data-enable-notifications="true"]');
    
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            // Kiểm tra validation
            if (!this.checkValidity()) {
                e.preventDefault();
                return;
            }
            
            // Hiển thị thông báo đang xử lý
            // Swal.fire({
            //     title: 'Đang xử lý...',
            //     text: 'Vui lòng chờ trong giây lát',
            //     allowOutsideClick: false,
            //     allowEscapeKey: false,
            //     showConfirmButton: false,
            //     didOpen: () => {
            //         Swal.showLoading();
            //     }
            // });
        });
    });
    
    // Xử lý các nút xóa có class 'delete-btn' - ĐÃ TẮT ĐỂ TRÁNH CONFLICT
    // const deleteButtons = document.querySelectorAll('.delete-btn');
    
    // deleteButtons.forEach(btn => {
    //     btn.addEventListener('click', function(e) {
    //         e.preventDefault();
    //         const itemName = this.dataset.itemName || 'mục này';
    //         const deleteUrl = this.getAttribute('href');
            
    //         Swal.fire({
    //             title: 'Xác nhận xóa',
    //             text: `Bạn có chắc muốn xóa ${itemName}?`,
    //             icon: 'warning',
    //             showCancelButton: true,
    //             confirmButtonColor: '#d33',
    //             cancelButtonColor: '#3085d6',
    //             confirmButtonText: 'Xóa',
    //             cancelButtonText: 'Hủy'
    //         }).then((result) => {
    //             if (result.isConfirmed) {
    //                 // Hiển thị loading - ĐÃ TẮT
    //                 // Swal.fire({
    //                 //     title: 'Đang xóa...',
    //                 //     text: 'Vui lòng chờ trong giây lát',
    //                 //     allowOutsideClick: false,
    //                 //     allowEscapeKey: false,
    //                 //     showConfirmButton: false,
    //                 //     didOpen: () => {
    //                 //         Swal.showLoading();
    //                 //     }
    //                 // });
                    
    //                 // Chuyển hướng đến URL xóa
    //                 window.location.href = deleteUrl;
    //             }
    //         });
    //     });
    // });
    
    // Xử lý các nút xác nhận có class 'confirm-btn'
    const confirmButtons = document.querySelectorAll('.confirm-btn');
    
    confirmButtons.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const message = this.dataset.message || 'Bạn có chắc muốn thực hiện hành động này?';
            const formId = this.dataset.formId;
            
            Swal.fire({
                title: 'Xác nhận',
                text: message,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Xác nhận',
                cancelButtonText: 'Hủy'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Submit form nếu có
                    if (formId) {
                        const form = document.getElementById(formId);
                        if (form) {
                                                // Hiển thị loading - ĐÃ TẮT
                    // Swal.fire({
                    //     title: 'Đang xử lý...',
                    //     text: 'Vui lòng chờ trong giây lát',
                    //     allowOutsideClick: false,
                    //     allowEscapeKey: false,
                    //     showConfirmButton: false,
                    //     didOpen: () => {
                    //         Swal.showLoading();
                    //     }
                    // });
                    
                    form.submit();
                        }
                    }
                }
            });
        });
    });
});

// Hàm hiển thị thông báo thành công
function showSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: 'Thành công!',
        text: message,
        timer: 3000,
        showConfirmButton: false
    });
}

// Hàm hiển thị thông báo lỗi
function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: message,
        confirmButtonText: 'Đóng'
    });
}

// Hàm hiển thị thông báo cảnh báo
function showWarning(message) {
    Swal.fire({
        icon: 'warning',
        title: 'Cảnh báo!',
        text: message,
        confirmButtonText: 'Đóng'
    });
}

// Hàm hiển thị thông báo thông tin
function showInfo(message) {
    Swal.fire({
        icon: 'info',
        title: 'Thông tin',
        text: message,
        confirmButtonText: 'Đóng'
    });
}
