// Toggle the side navigation
window.addEventListener('DOMContentLoaded', event => {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', () => {
            sidebar.classList.toggle('show');
            mainContent.classList.toggle('expanded');
        });
    }

    // Loading state for buttons
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', function (event) {
            if (this.classList.contains('loading') || this.classList.contains('view-all')) {
                return;
            }
            if (this.tagName === 'A') {
                event.preventDefault();
            }
            this.classList.add('loading');
            const originalText = this.textContent;
            this.textContent = 'Đang xử lý...';
            setTimeout(() => {
                this.textContent = originalText;
                this.classList.remove('loading');
            }, 1000);
        });
    });

    // Search filter for invoices
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const filter = this.value.toLowerCase();
            const rows = document.querySelectorAll('#invoiceTable tbody tr');
            rows.forEach(row => {
                const invoiceId = row.querySelector('td[data-label="Mã hoá đơn"]').textContent.toLowerCase();
                const customer = row.querySelector('td[data-label="Khách hàng"]').textContent.toLowerCase();
                const date = row.querySelector('td[data-label="Ngày tạo"]').textContent.toLowerCase();
                if (invoiceId.includes(filter) || customer.includes(filter) || date.includes(filter)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            });
        });
    }

    // Initialize DataTables
    if ($.fn.DataTable) {
        $('.datatable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.11.5/i18n/vi.json'
            },
            responsive: true
        });
    }
});