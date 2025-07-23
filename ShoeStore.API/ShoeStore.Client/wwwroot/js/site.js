// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Custom JavaScript for ShoeStore

// Global variables
let cartItemCount = 0;
let isInitialized = false;

// Initialize when document is ready
$(document).ready(function() {
    if (!isInitialized) {
        initializeApp();
        isInitialized = true;
    }
});

// Main initialization function
function initializeApp() {
    setupEventListeners();
    loadCartCount();
    setupLazyLoading();
}

// Setup event listeners with delegation for better performance
function setupEventListeners() {
    // Use event delegation for dynamic content
    $(document).on('click', '.add-to-cart-btn', function(e) {
        e.preventDefault();
        const productId = $(this).data('product-id');
        addToCart(productId);
    });

    // Quantity controls with delegation
    $(document).on('click', '.quantity-btn', function() {
        const action = $(this).data('action');
        const input = $(this).siblings('.quantity-input');
        let value = parseInt(input.val());
        
        if (action === 'increase') {
            const max = parseInt(input.attr('max'));
            if (value < max) {
                input.val(value + 1).trigger('change');
            }
        } else if (action === 'decrease') {
            if (value > 1) {
                input.val(value - 1).trigger('change');
            }
        }
    });

    // Search functionality
    $('#searchForm').on('submit', function(e) {
        e.preventDefault();
        const searchTerm = $('#searchInput').val().trim();
        if (searchTerm) {
            window.location.href = `/Products?searchTerm=${encodeURIComponent(searchTerm)}`;
        }
    });

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(e) {
        e.preventDefault();
        const target = $(this.getAttribute('href'));
        if (target.length) {
            $('html, body').animate({
                scrollTop: target.offset().top - 100
            }, 600);
        }
    });
}

// Optimized cart functions
function addToCart(productId) {
    // Show loading state
    const btn = $(`.add-to-cart-btn[data-product-id="${productId}"]`);
    const originalText = btn.html();
    btn.addClass('loading').html('<i class="fas fa-spinner fa-spin"></i> Đang thêm...').prop('disabled', true);

    // Simulate API call (replace with actual API call)
    setTimeout(() => {
        cartItemCount++;
        updateCartBadge(cartItemCount);
        
        // Show success message
        showNotification('Sản phẩm đã được thêm vào giỏ hàng!', 'success');
        
        // Reset button
        btn.removeClass('loading').html(originalText).prop('disabled', false);
    }, 800);
}

function updateCartBadge(count) {
    const badge = $('#cartBadge');
    badge.text(count).fadeIn(200);
    if (count === 0) {
        badge.hide();
    }
}

function loadCartCount() {
    // Load cart count from session/localStorage
    const count = localStorage.getItem('cartItemCount') || 0;
    updateCartBadge(parseInt(count));
}

// Optimized lazy loading for images
function setupLazyLoading() {
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    if (img.dataset.src) {
                        img.src = img.dataset.src;
                        img.classList.remove('lazy');
                        imageObserver.unobserve(img);
                    }
                }
            });
        }, {
            rootMargin: '50px 0px',
            threshold: 0.01
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }
}

// Optimized form validation
function setupFormValidation() {
    // Contact form validation
    $('#contactForm').on('submit', function(e) {
        e.preventDefault();
        
        if (validateContactForm()) {
            submitContactForm();
        }
    });

    // Search form validation
    $('#searchForm').on('submit', function(e) {
        const searchTerm = $('#searchInput').val().trim();
        if (!searchTerm) {
            e.preventDefault();
            showNotification('Vui lòng nhập từ khóa tìm kiếm', 'warning');
        }
    });
}

function validateContactForm() {
    let isValid = true;
    const requiredFields = ['firstName', 'email', 'subject', 'message'];
    
    requiredFields.forEach(field => {
        const value = $(`#${field}`).val().trim();
        if (!value) {
            showFieldError(field, 'Trường này là bắt buộc');
            isValid = false;
        } else {
            clearFieldError(field);
        }
    });

    // Email validation
    const email = $('#email').val().trim();
    if (email && !isValidEmail(email)) {
        showFieldError('email', 'Email không hợp lệ');
        isValid = false;
    }

    return isValid;
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showFieldError(fieldId, message) {
    const field = $(`#${fieldId}`);
    field.addClass('is-invalid');
    
    let errorDiv = field.siblings('.invalid-feedback');
    if (errorDiv.length === 0) {
        errorDiv = $('<div class="invalid-feedback"></div>');
        field.after(errorDiv);
    }
    errorDiv.text(message);
}

function clearFieldError(fieldId) {
    const field = $(`#${fieldId}`);
    field.removeClass('is-invalid');
    field.siblings('.invalid-feedback').remove();
}

// Optimized notification system
function showNotification(message, type = 'info') {
    const notification = $(`
        <div class="alert alert-${type} alert-dismissible fade show notification-toast" role="alert">
            <i class="fas fa-${getNotificationIcon(type)} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);

    // Add to notification container
    let container = $('#notificationContainer');
    if (container.length === 0) {
        container = $('<div id="notificationContainer" class="position-fixed top-0 end-0 p-3" style="z-index: 9999;"></div>');
        $('body').append(container);
    }

    container.append(notification);

    // Auto remove after 4 seconds
    setTimeout(() => {
        notification.fadeOut(300, () => notification.remove());
    }, 4000);
}

function getNotificationIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// Optimized utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Debounce function for performance
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Optimized API helper functions
function apiCall(url, method = 'GET', data = null) {
    return $.ajax({
        url: url,
        method: method,
        data: data,
        contentType: 'application/json',
        dataType: 'json',
        timeout: 10000 // 10 second timeout
    }).fail(function(xhr, status, error) {
        console.error('API call failed:', error);
        showNotification('Có lỗi xảy ra, vui lòng thử lại sau', 'error');
    });
}

// Optimized local storage helpers
const Storage = {
    set: function(key, value) {
        try {
            localStorage.setItem(key, JSON.stringify(value));
        } catch (e) {
            console.error('Error saving to localStorage:', e);
        }
    },
    
    get: function(key, defaultValue = null) {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : defaultValue;
        } catch (e) {
            console.error('Error reading from localStorage:', e);
            return defaultValue;
        }
    },
    
    remove: function(key) {
        try {
            localStorage.removeItem(key);
        } catch (e) {
            console.error('Error removing from localStorage:', e);
        }
    }
};

// Export functions for global use
window.ShoeStore = {
    addToCart,
    updateCartBadge,
    showNotification,
    formatCurrency,
    Storage
};
