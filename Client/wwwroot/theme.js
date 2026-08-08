(function () {
    const storageKey = "birpos-theme";
    const darkTheme = "dark";
    const lightTheme = "light";
    let controlsObserver;

    function storedTheme() {
        try {
            return localStorage.getItem(storageKey);
        } catch {
            return null;
        }
    }

    function systemTheme() {
        return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches
            ? darkTheme
            : lightTheme;
    }

    function normalize(theme) {
        return theme === darkTheme ? darkTheme : lightTheme;
    }

    function currentTheme() {
        return normalize(storedTheme() || systemTheme());
    }

    function updateControls(theme) {
        const isDark = theme === darkTheme;
        document.querySelectorAll("[data-birpos-theme-toggle]").forEach((button) => {
            button.setAttribute("aria-pressed", String(isDark));
            button.setAttribute("title", isDark ? "Switch to light mode" : "Switch to dark mode");

            const label = button.querySelector("[data-birpos-theme-label]");
            if (label) {
                const nextLabel = isDark ? "Dark" : "Light";
                if (label.textContent !== nextLabel) {
                    label.textContent = nextLabel;
                }
            }

            const icon = button.querySelector("[data-birpos-theme-icon]");
            if (icon) {
                const nextIcon = isDark ? "fas fa-moon" : "fas fa-sun";
                if (icon.className !== nextIcon) {
                    icon.className = nextIcon;
                }
            }
        });
    }

    function startControlObserver() {
        if (controlsObserver || !document.body || !window.MutationObserver) {
            return;
        }

        controlsObserver = new MutationObserver((mutations) => {
            if (mutations.some((mutation) => mutation.addedNodes.length > 0)) {
                updateControls(currentTheme());
            }
        });

        controlsObserver.observe(document.body, { childList: true, subtree: true });
    }

    function applyTheme(theme) {
        const nextTheme = normalize(theme);
        const isDark = nextTheme === darkTheme;

        document.documentElement.setAttribute("data-theme", nextTheme);
        document.documentElement.style.colorScheme = nextTheme;

        if (document.body) {
            document.body.setAttribute("data-theme", nextTheme);
            document.body.classList.toggle("dark-mode", isDark);
            document.body.classList.toggle("birpos-theme-dark", isDark);
            document.body.classList.toggle("birpos-theme-light", !isDark);
        }

        updateControls(nextTheme);
        return nextTheme;
    }

    function setTheme(theme) {
        const nextTheme = applyTheme(theme);
        try {
            localStorage.setItem(storageKey, nextTheme);
        } catch {
            // Storage may be unavailable in private or restricted browser contexts.
        }

        return nextTheme;
    }

    function toggleTheme() {
        return setTheme(currentTheme() === darkTheme ? lightTheme : darkTheme);
    }

    window.birposTheme = {
        init: function () {
            const theme = applyTheme(currentTheme());
            startControlObserver();
            return theme;
        },
        set: setTheme,
        toggle: toggleTheme,
        toggleFromButton: function () {
            return toggleTheme();
        }
    };

    applyTheme(currentTheme());
    startControlObserver();

    document.addEventListener("DOMContentLoaded", function () {
        applyTheme(currentTheme());
        startControlObserver();
    });
})();
