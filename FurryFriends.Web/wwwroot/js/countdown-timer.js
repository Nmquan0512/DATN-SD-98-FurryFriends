// Countdown Timer Functions
function initializeCountdownTimers() {
    const countdownSections = document.querySelectorAll('.countdown-section');
    
    countdownSections.forEach(section => {
        const endDate = new Date(section.dataset.endDate);
        const timer = section.querySelector('.countdown-timer');
        
        if (timer && endDate) {
            updateCountdown(timer, endDate);
            
            // Update every second
            setInterval(() => {
                updateCountdown(timer, endDate);
            }, 1000);
        }
    });
}

function updateCountdown(timer, endDate) {
    const now = new Date().getTime();
    const distance = endDate.getTime() - now;
    
    if (distance < 0) {
        // Countdown has ended
        timer.innerHTML = '<span style="color: #ff6b6b;">Đã kết thúc</span>';
        return;
    }
    
    // Calculate time units
    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);
    
    // Update timer display
    const daysSpan = timer.querySelector('.countdown-days');
    const hoursSpan = timer.querySelector('.countdown-hours');
    const minutesSpan = timer.querySelector('.countdown-minutes');
    const secondsSpan = timer.querySelector('.countdown-seconds');
    
    if (daysSpan) daysSpan.textContent = days.toString().padStart(2, '0');
    if (hoursSpan) hoursSpan.textContent = hours.toString().padStart(2, '0');
    if (minutesSpan) minutesSpan.textContent = minutes.toString().padStart(2, '0');
    if (secondsSpan) secondsSpan.textContent = seconds.toString().padStart(2, '0');
    
    // Add urgency effect when less than 1 hour remaining
    if (distance < 3600000) { // Less than 1 hour
        timer.style.color = '#ffeb3b';
        timer.style.animation = 'countdownPulse 1s infinite';
    }
}

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { initializeCountdownTimers, updateCountdown };
}
