

//// Chỉ loading cho <button class="btn ...">
//document.querySelectorAll('button.btn').forEach(button => {
//    button.addEventListener('click', function (event) {
//        if (this.classList.contains('loading') || this.classList.contains('view-all')) {
//            return;
//        }
//        this.classList.add('loading');
//        const originalText = this.textContent;
//        this.textContent = 'Đang xử lý...';
//        setTimeout(() => {
//            this.textContent = originalText;
//            this.classList.remove('loading');
//        }, 1000);
//    });
//});



//    // Search filter for invoices
//    const searchInput = document.getElementById('searchInput');
//    if (searchInput) {
//        searchInput.addEventListener('input', function () {
//            const filter = this.value.toLowerCase();
//            const rows = document.querySelectorAll('#invoiceTable tbody tr');
//            rows.forEach(row => {
//                const invoiceId = row.querySelector('td[data-label="Mã hoá đơn"]').textContent.toLowerCase();
//                const customer = row.querySelector('td[data-label="Khách hàng"]').textContent.toLowerCase();
//                const date = row.querySelector('td[data-label="Ngày tạo"]').textContent.toLowerCase();
//                if (invoiceId.includes(filter) || customer.includes(filter) || date.includes(filter)) {
//                    row.style.display = '';
//                } else {
//                    row.style.display = 'none';
//                }
//            });
//        });
//    }

//    // Initialize DataTables
//    if ($.fn.DataTable) {
//        $('.datatable').DataTable({
//            language: {
//                url: '//cdn.datatables.net/plug-ins/1.11.5/i18n/vi.json'
//            },
//            responsive: true
//        });
//    }
//});