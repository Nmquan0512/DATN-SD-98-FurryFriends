function togglePassword(inputId, iconId) {
    const passwordInput = document.getElementById(inputId);
    const toggleIcon = document.getElementById(iconId);
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.add('bi-eye-slash');
        toggleIcon.classList.remove('bi-eye');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.add('bi-eye');
        toggleIcon.classList.remove('bi-eye-slash');
    }
}
function checkPasswordStrength(password) {
    let strength = 0;
    let feedback = [];
    if (password.length >= 8) strength += 1;
    else feedback.push('ít nhất 8 ký tự');
    if (/[A-Z]/.test(password)) strength += 1;
    else feedback.push('chữ hoa');
    if (/[a-z]/.test(password)) strength += 1;
    else feedback.push('chữ thường');
    if (/\d/.test(password)) strength += 1;
    else feedback.push('số');
    if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) strength += 1;
    else feedback.push('ký tự đặc biệt');
    const strengthFill = document.getElementById('strengthFill');
    const strengthText = document.getElementById('strengthText');
    strengthFill.className = 'strength-fill';
    if (strength <= 2) {
        strengthFill.classList.add('strength-weak');
        strengthText.textContent = 'Yếu - Cần: ' + feedback.join(', ');
        strengthText.style.color = '#ff4757';
    } else if (strength === 3) {
        strengthFill.classList.add('strength-medium');
        strengthText.textContent = 'Trung bình - Cần: ' + feedback.join(', ');
        strengthText.style.color = '#ffa502';
    } else if (strength === 4) {
        strengthFill.classList.add('strength-good');
        strengthText.textContent = 'Tốt';
        strengthText.style.color = '#2ed573';
    } else {
        strengthFill.classList.add('strength-strong');
        strengthText.textContent = 'Rất mạnh';
        strengthText.style.color = '#2ed573';
    }
    return strength;
}

function validateForm() {
    const fullname = document.getElementById('fullname').value.trim();
    const username = document.getElementById('username').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    const terms = document.getElementById('terms').checked;
    const errors = [];
    if (!fullname) errors.push('Họ và tên không được để trống');
    if (!username) errors.push('Tài khoản không được để trống');
    if (username.length < 3) errors.push('Tài khoản phải có ít nhất 3 ký tự');
    if (!phone) errors.push('Số điện thoại không được để trống');
    if (!/^[0-9]{10,11}$/.test(phone)) errors.push('Số điện thoại không hợp lệ');
    if (!email) errors.push('Email không được để trống');
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) errors.push('Email không hợp lệ');
    if (!password) errors.push('Mật khẩu không được để trống');
    if (checkPasswordStrength(password) < 3) errors.push('Mật khẩu quá yếu');
    if (password !== confirmPassword) errors.push('Mật khẩu xác nhận không khớp');
    if (!terms) errors.push('Bạn phải đồng ý với điều khoản dịch vụ');
    return errors;
}
document.getElementById('password').addEventListener('input', function() {
    checkPasswordStrength(this.value);
});
document.querySelectorAll('input').forEach(input => {
    input.addEventListener('focus', function() {
        this.parentElement.style.transform = 'translateY(-2px)';
    });
    input.addEventListener('blur', function() {
        this.parentElement.style.transform = 'translateY(0)';
    });
});
document.querySelectorAll('button').forEach(button => {
    button.addEventListener('click', function(e) {
        const ripple = document.createElement('span');
        const rect = this.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = x + 'px';
        ripple.style.top = y + 'px';
        ripple.style.position = 'absolute';
        ripple.style.borderRadius = '50%';
        ripple.style.background = 'rgba(255, 255, 255, 0.3)';
        ripple.style.transform = 'scale(0)';
        ripple.style.animation = 'ripple 0.6s linear';
        ripple.style.pointerEvents = 'none';
        this.appendChild(ripple);
        setTimeout(() => {
            ripple.remove();
        }, 600);
    });
});
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(2);
            opacity: 0;
        }
    }
    button {
        position: relative;
        overflow: hidden;
    }
`;
document.head.appendChild(style);
document.addEventListener('DOMContentLoaded', function () {
    ['Password', 'ConfirmPassword'].forEach(function (field) {
        var input = document.getElementById(field);
        var toggle = document.getElementById(field === 'Password' ? 'toggleIcon1' : 'toggleIcon2');
        if (input && toggle) {
            // Ẩn/hiện icon khi load và khi nhập
            toggle.style.display = input.value.length > 0 ? 'block' : 'none';
            input.addEventListener('input', function () {
                toggle.style.display = input.value.length > 0 ? 'block' : 'none';
            });
        }
    });
}); 