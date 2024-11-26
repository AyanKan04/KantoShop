// Các phần tử cần dùng
const searchIcon = document.getElementById('search-icon');
const searchContainerWrapper = document.getElementById('search-container-wrapper');
const bar = document.getElementById('bar');
const close = document.getElementById('close');
const nav = document.getElementById('navbar');

// Hiển thị thanh tìm kiếm khi nhấn vào biểu tượng kính lúp
if (searchIcon) {
    searchIcon.addEventListener('click', () => {
        searchContainerWrapper.classList.toggle('show'); // Hiển thị / ẩn thanh tìm kiếm
    });
}

// Thanh điều hướng: Mở
if (bar) {
    bar.addEventListener('click', () => {
        nav.classList.add('active');
    });
}

// Thanh điều hướng: Đóng
if (close) {
    close.addEventListener('click', () => {
        nav.classList.remove('active');
    });
}
$(document).ready(function () {
    $(".filter-sidebar button").click(function () {
        var priceRange = $("#price-range").val();
        var size = $("#size").val();
        var searchString = $("#SearchString").val();
        var url = "/Products/Index?PriceRange=" + priceRange + "&Size=" + size + "&SearchString=" + searchString;
        window.location.href = url;
    });
});

// JavaScript để reset bộ lọc khi trang được tải lại hoặc người dùng quay lại trang Index
document.addEventListener('DOMContentLoaded', function () {
    // Reset các bộ lọc khi tải lại trang
    resetFilters();

    // Xử lý sự kiện nhấn nút áp dụng bộ lọc
    document.getElementById('apply-filters').addEventListener('click', function () {
        applyFilters();
    });
});

function resetFilters() {
    // Đặt giá trị mặc định cho các bộ lọc
    document.getElementById('price-range').value = '0';
    document.getElementById('size').value = '0';
    document.getElementById('product-type').value = '0';
}

function applyFilters() {
    // Lấy giá trị bộ lọc
    var priceRange = document.getElementById('price-range').value;
    var size = document.getElementById('size').value;
    var productType = document.getElementById('product-type').value;

    // Lọc sản phẩm theo giá trị lọc
    // Giả sử bạn gửi giá trị các bộ lọc tới server hoặc thay đổi nội dung trang
    console.log("Áp dụng bộ lọc: ", {
        priceRange: priceRange,
        size: size,
        productType: productType
    });

    // Có thể sử dụng AJAX để tải lại sản phẩm theo bộ lọc mà không cần tải lại trang
    // Ví dụ:
     $.ajax({
         url: '/Products/Filter',
         data: { priceRange: priceRange, size: size, productType: productType },
         success: function (response) {
             // Cập nhật lại giao diện với các sản phẩm mới
         }
     });
}
document.addEventListener("scroll", function () {
    const sidebar = document.getElementById("filter-sidebar");
    const header = document.querySelector("#header");
    const headerRect = header.getBoundingClientRect();
    const footer = document.querySelector("footer");
    const footerRect = footer.getBoundingClientRect();
    const sidebarHeight = sidebar.offsetHeight;

    // Kiểm tra xem thanh lọc có bị cuộn qua footer không
    if (footerRect.top < sidebarHeight) {
        sidebar.style.position = "absolute";
        sidebar.style.top = (footerRect.top - sidebarHeight - 20) + "px"; // Cách footer 20px
    } else {
        sidebar.style.position = "sticky";
        sidebar.style.top = "100px";
    }

    // Kiểm tra xem thanh lọc có cuộn qua header không
    if (headerRect.bottom <= 100) { // Nếu bottom của header <= 100px (khoảng cách từ top)
        sidebar.style.top = "100px"; // Cố định top của thanh lọc
        sidebar.style.clip = "rect(0, auto, auto, 0)"; // Ẩn phần tràn qua header
    } else {
        sidebar.style.clip = "auto"; // Hiển thị toàn bộ thanh lọc
    }
});