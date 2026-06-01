(() => {
    const root = document.querySelector("[data-run-files]");
    if (!root) return;

    const runId = root.dataset.runId;
    const canEdit = root.dataset.canEdit === "true";
    const list = root.querySelector("[data-run-files-list]");
    const status = root.querySelector("[data-run-files-status]");
    const uploadForm = root.querySelector("[data-run-file-upload]");
    const fileInput = root.querySelector("[data-run-file-input]");
    const selectedFileName = root.querySelector("[data-run-selected-file]");
    const uploadButton = uploadForm?.querySelector(".run-file-upload-button");
    const imageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    const setStatus = (message) => {
        if (status) status.textContent = message || "";
    };

    const getFileName = (file) => file.originalFileName || "Uploaded file";

    const isImageFile = (file) => {
        const name = getFileName(file).toLowerCase();
        const contentType = (file.contentType || "").toLowerCase();
        return contentType.startsWith("image/") || imageExtensions.some(extension => name.endsWith(extension));
    };

    const createDeleteButton = (file) => {
        const button = document.createElement("button");
        button.type = "button";
        button.className = "run-file-card__delete";
        button.textContent = "Delete";
        button.addEventListener("click", async () => {
            const deleteResponse = await fetch(`/api/runs/files/${file.id}`, { method: "DELETE" });
            if (deleteResponse.ok) await loadFiles();
        });
        return button;
    };

    const createFileCard = (file) => {
        const item = document.createElement("li");
        item.className = "run-file-card";

        const name = document.createElement("span");
        name.className = "run-file-card__name";
        name.textContent = getFileName(file);

        if (isImageFile(file)) {
            item.classList.add("run-file-card--image");

            const link = document.createElement("a");
            link.className = "run-file-card__thumb-link";
            link.href = file.filePath;
            link.target = "_blank";
            link.rel = "noopener";
            link.setAttribute("aria-label", `Open ${getFileName(file)}`);

            const image = document.createElement("img");
            image.className = "run-file-card__thumb";
            image.src = file.filePath;
            image.alt = getFileName(file);
            image.loading = "lazy";

            link.appendChild(image);
            item.append(link, name);
        } else {
            const icon = document.createElement("span");
            icon.className = "run-file-card__icon";
            icon.setAttribute("aria-hidden", "true");
            icon.innerHTML = `
                <svg viewBox="0 0 32 32" focusable="false">
                    <path d="M9 4.5h9l5 5v18H9z" />
                    <path d="M18 4.5v5h5" />
                    <path d="M12 16h8M12 20h8M12 24h5" />
                </svg>`;

            item.append(icon, name);
        }

        if (canEdit) {
            item.appendChild(createDeleteButton(file));
        }

        return item;
    };

    const resetUploadControls = () => {
        if (fileInput) fileInput.value = "";
        if (selectedFileName) selectedFileName.textContent = "No file selected";
        if (uploadButton) uploadButton.hidden = true;
    };

    const loadFiles = async () => {
        setStatus("Loading files...");
        const response = await fetch(`/api/runs/${runId}/files`, { headers: { Accept: "application/json" } });
        if (!response.ok) {
            setStatus("Could not load files.");
            return;
        }

        const files = await response.json();
        list.innerHTML = "";
        if (!files.length) {
            setStatus("No files uploaded.");
            return;
        }

        setStatus("");
        for (const file of files) {
            list.appendChild(createFileCard(file));
        }
    };

    fileInput?.addEventListener("change", () => {
        const file = fileInput.files?.[0];
        if (selectedFileName) {
            selectedFileName.textContent = file?.name || "No file selected";
        }

        if (uploadButton) {
            uploadButton.hidden = !file;
        }
    });

    uploadForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const input = fileInput || uploadForm.querySelector("input[type='file']");
        if (!input?.files?.length) {
            setStatus("Choose a file first.");
            return;
        }

        const formData = new FormData();
        formData.append("file", input.files[0]);

        setStatus("Uploading...");
        const response = await fetch(`/api/runs/${runId}/files`, {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            setStatus("Upload failed.");
            return;
        }

        resetUploadControls();
        await loadFiles();
    });

    loadFiles();
})();
