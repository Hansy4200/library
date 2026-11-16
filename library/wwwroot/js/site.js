
document.addEventListener("DOMContentLoaded", function () {

    // DARK AND LIGHT MODE
    const themeToggle = document.getElementById("themeToggle");

    const savedTheme = localStorage.getItem("library-theme");

    if (savedTheme === "dark") {
        document.body.classList.add("dark-mode");
    }

    function updateThemeButton() {
        if (!themeToggle) return;

        const isDark = document.body.classList.contains("dark-mode");

        themeToggle.textContent = isDark
            ? "☀ Light mode"
            : "☾ Dark mode";
    }

    updateThemeButton();

    if (themeToggle) {
        themeToggle.addEventListener("click", function () {

            document.body.classList.toggle("dark-mode");

            const isDark = document.body.classList.contains("dark-mode");

            localStorage.setItem(
                "library-theme",
                isDark ? "dark" : "light"
            );

            updateThemeButton();
        });
    }

    // TABLE SEARCH
    const searchInput = document.getElementById("tableSearch");
    const dataTable = document.getElementById("dataTable");

    if (searchInput && dataTable) {

        searchInput.addEventListener("input", function () {

            const searchValue = searchInput.value.toLowerCase();

            const rows = dataTable.querySelectorAll("tbody tr");

            rows.forEach(function (row) {

                const rowText = row.textContent.toLowerCase();

                row.style.display = rowText.includes(searchValue)
                    ? ""
                    : "none";
            });
        });
    }

});
