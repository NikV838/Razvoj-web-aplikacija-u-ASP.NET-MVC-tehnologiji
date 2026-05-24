(() => {
    const input = document.querySelector("[data-run-search]");
    const list = document.querySelector("[data-run-list]");
    const status = document.querySelector("[data-run-search-status]");
    const empty = document.querySelector("[data-run-empty]");
    const panel = document.querySelector("[data-run-search-panel]");
    const filterButtons = [...document.querySelectorAll("[data-run-direction-filter]")];
    let timer;
    let controller;
    let activeDirection = "";

    if (!input || !list) {
        return;
    }

    const formatCount = count => `${count} ${count === 1 ? "run" : "runs"}`;

    const getRows = () => [...list.querySelectorAll("[data-run-direction]")];

    const updateEmptyState = visibleCount => {
        if (empty) {
            empty.hidden = visibleCount !== 0;
        }

        if (status) {
            status.textContent = activeDirection
                ? `${formatCount(visibleCount)} shown`
                : formatCount(visibleCount);
        }
    };

    const applyDirectionFilter = () => {
        let visibleCount = 0;

        // JS filtering occurs here: rows are matched by their data-run-direction attribute.
        getRows().forEach(row => {
            const isVisible = !activeDirection || row.dataset.runDirection === activeDirection;
            row.classList.toggle("is-filter-hidden", !isVisible);
            row.setAttribute("aria-hidden", String(!isVisible));

            if (isVisible) {
                visibleCount += 1;
            }
        });

        updateEmptyState(visibleCount);
    };

    const setActiveDirection = direction => {
        activeDirection = activeDirection === direction ? "" : direction;

        // Active filter state is handled here with classes and aria-pressed.
        filterButtons.forEach(button => {
            const isActive = button.dataset.runDirectionFilter === activeDirection;
            button.classList.toggle("is-active", isActive);
            button.setAttribute("aria-pressed", String(isActive));
        });

        list.classList.add("is-filtering");
        applyDirectionFilter();
        window.setTimeout(() => list.classList.remove("is-filtering"), 260);
    };

    const createRow = run => {
        const item = document.createElement("li");
        item.dataset.runDirection = (run.direction || "").toLowerCase();

        const link = document.createElement("a");
        link.className = "run-list-link";
        link.href = run.detailsUrl;

        const main = document.createElement("span");
        main.className = "run-main";

        const name = document.createElement("span");
        name.className = "run-name";
        name.textContent = run.track;

        const meta = document.createElement("span");
        meta.className = "run-meta";
        meta.textContent = `Run ${run.id} | ${run.driverName} | ${run.carName} | ${run.registrationNumber} | ${run.date}`;

        const side = document.createElement("span");
        side.className = "run-side";

        const bestTime = document.createElement("span");
        bestTime.className = "run-besttime";
        bestTime.textContent = run.bestTime;

        const weather = document.createElement("span");
        weather.className = "run-track-badge";
        weather.textContent = run.weather;

        const actions = document.createElement("div");
        actions.className = "run-row-actions";

        [["Edit", run.editUrl], ["Delete", run.deleteUrl]].forEach(([text, url]) => {
            const action = document.createElement("a");
            action.textContent = text;
            action.href = url;
            actions.appendChild(action);
        });

        main.append(name, meta);
        side.append(bestTime, weather);
        link.append(main, side);
        item.append(link, actions);
        return item;
    };

    filterButtons.forEach(button => {
        const startDrivingPreview = () => {
            // Hover-controlled animation: JS only toggles this state, CSS keyframes do the driving.
            button.classList.add("is-driving");
        };

        const stopDrivingPreview = () => {
            button.classList.remove("is-driving");
        };

        button.addEventListener("pointerenter", startDrivingPreview);
        button.addEventListener("pointerleave", stopDrivingPreview);
        button.addEventListener("focus", startDrivingPreview);
        button.addEventListener("blur", stopDrivingPreview);

        button.addEventListener("click", () => {
            setActiveDirection(button.dataset.runDirectionFilter);
        });
    });

    input.addEventListener("input", () => {
        window.clearTimeout(timer);
        timer = window.setTimeout(async () => {
            controller?.abort();
            controller = new AbortController();
            panel?.setAttribute("data-state", "loading");
            list.classList.add("is-updating");

            try {
                const url = `${input.dataset.runSearchUrl}?query=${encodeURIComponent(input.value)}`;
                const response = await fetch(url, {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                    signal: controller.signal
                });

                if (!response.ok) {
                    throw new Error("Run search failed");
                }

                const runs = await response.json();
                list.replaceChildren(...runs.map(createRow));
                list.classList.remove("is-updating");
                list.classList.add("has-refreshed");
                window.setTimeout(() => list.classList.remove("has-refreshed"), 260);
                panel?.setAttribute("data-state", "ready");
                applyDirectionFilter();
            } catch (error) {
                if (error.name === "AbortError") {
                    return;
                }

                list.classList.remove("is-updating");
                panel?.setAttribute("data-state", "error");
                if (status) {
                    status.textContent = "Search unavailable";
                }
            }
        }, 180);
    });

    applyDirectionFilter();
})();
