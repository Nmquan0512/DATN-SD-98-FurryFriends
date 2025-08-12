// Test Dashboard functionality
console.log('Dashboard test script loaded');

// Test function để kiểm tra dữ liệu
function testDashboardData() {
    console.log('Testing dashboard data...');
    
    // Test API call với các period khác nhau
    const periods = ['month', 'quarter', 'year'];
    
    periods.forEach(period => {
        fetch(`/Admin/Dashboard/GetChartData?period=${period}`)
            .then(response => response.json())
            .then(data => {
                console.log(`Chart data for ${period}:`, data);
                
                if (data.success) {
                    console.log(`✅ ${period.toUpperCase()} data loaded successfully`);
                    if (data.revenueData) {
                        console.log(`   Revenue labels:`, data.revenueData[0]?.labels);
                        console.log(`   Revenue values:`, data.revenueData[1]?.values);
                    }
                } else {
                    console.error(`❌ ${period.toUpperCase()} data failed:`, data.message);
                }
            })
            .catch(error => {
                console.error(`❌ Error loading ${period} data:`, error);
            });
    });
}

// Test function để kiểm tra dữ liệu thống kê
function testStatisticsData() {
    console.log('Testing statistics data...');
    
    // Kiểm tra ViewBag data
    const stats = ['TotalCustomers', 'TotalProducts', 'TotalEmployees', 'TotalOrders', 'MonthlyRevenue'];
    stats.forEach(stat => {
        const value = typeof ViewBag !== 'undefined' ? ViewBag[stat] : 'Not available';
        console.log(`${stat}:`, value);
    });
}

// Auto-test khi trang load
document.addEventListener('DOMContentLoaded', function() {
    console.log('Dashboard page loaded, running tests...');
    setTimeout(() => {
        testStatisticsData();
        testDashboardData();
    }, 1000); // Delay 1 giây để đảm bảo trang đã load xong
});

// Export functions để test từ console
window.testDashboard = testDashboardData;
window.testStats = testStatisticsData;
