(() => {
    const debounce = (callback, delay) => {
        let timer;
        return (...args) => {
            window.clearTimeout(timer);
            timer = window.setTimeout(() => callback(...args), delay);
        };
    };

    document.querySelectorAll("[data-autocomplete]").forEach(root => {
        const input = root.querySelector("[data-autocomplete-input]");
        const hidden = root.querySelector("[data-autocomplete-value]");
        const menu = root.querySelector("[data-autocomplete-menu]");
        const url = root.dataset.autocompleteUrl;
        let items = [];
        let activeIndex = -1;

        if (!input || !hidden || !menu || !url) {
            return;
        }

        const setStatus = text => {
            menu.innerHTML = `<div class="ta-autocomplete-status">${text}</div>`;
            menu.hidden = false;
            input.setAttribute("aria-expanded", "true");
        };

        const closeMenu = () => {
            menu.hidden = true;
            input.setAttribute("aria-expanded", "false");
            activeIndex = -1;
        };

        const selectItem = item => {
            input.value = item.text;
            hidden.value = item.id;
            hidden.dispatchEvent(new Event("change", { bubbles: true }));
            input.classList.remove("is-invalid");
            closeMenu();
        };

        const render = results => {
            items = results;
            activeIndex = results.length ? 0 : -1;

            if (!results.length) {
                setStatus("No matches");
                return;
            }

            menu.replaceChildren(...results.map((item, index) => {
                const option = document.createElement("button");
                option.type = "button";
                option.className = "ta-autocomplete-option";
                option.setAttribute("role", "option");
                option.setAttribute("aria-selected", index === activeIndex ? "true" : "false");
                option.innerHTML = `<span>${item.text}</span><em>${item.meta || ""}</em>`;
                option.addEventListener("mousedown", event => {
                    event.preventDefault();
                    selectItem(item);
                });
                return option;
            }));

            menu.hidden = false;
            input.setAttribute("aria-expanded", "true");
        };

        const search = debounce(async () => {
            const query = input.value.trim();
            hidden.value = "";

            if (query.length < 1) {
                closeMenu();
                return;
            }

            setStatus("Searching...");
            root.classList.add("is-loading");

            try {
                const response = await fetch(`${url}?query=${encodeURIComponent(query)}`, {
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (response.ok) {
                    render(await response.json());
                }
            } finally {
                root.classList.remove("is-loading");
            }
        }, 220);

        input.addEventListener("input", search);
        input.addEventListener("blur", () => {
            window.setTimeout(() => hidden.dispatchEvent(new Event("blur", { bubbles: true })), 120);
        });

        input.addEventListener("focus", () => {
            if (input.value.trim().length > 0 && hidden.value.length === 0) {
                search();
            }
        });

        input.addEventListener("keydown", event => {
            if (menu.hidden || !items.length) {
                return;
            }

            if (event.key === "ArrowDown" || event.key === "ArrowUp") {
                event.preventDefault();
                activeIndex = event.key === "ArrowDown"
                    ? (activeIndex + 1) % items.length
                    : (activeIndex - 1 + items.length) % items.length;

                menu.querySelectorAll("[role='option']").forEach((option, index) => {
                    option.setAttribute("aria-selected", index === activeIndex ? "true" : "false");
                });
            }

            if (event.key === "Enter" && activeIndex >= 0) {
                event.preventDefault();
                selectItem(items[activeIndex]);
            }

            if (event.key === "Escape") {
                closeMenu();
            }
        });

        document.addEventListener("click", event => {
            if (!root.contains(event.target)) {
                closeMenu();
            }
        });
    });
})();
