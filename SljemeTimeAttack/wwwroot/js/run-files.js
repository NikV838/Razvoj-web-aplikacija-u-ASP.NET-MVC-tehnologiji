(() => {
    const root = document.querySelector("[data-run-files]");
    if (!root) return;

    const runId = root.dataset.runId;
    const canEdit = root.dataset.canEdit === "true";
    const list = root.querySelector("[data-run-files-list]");
    const status = root.querySelector("[data-run-files-status]");
    const uploadForm = root.querySelector("[data-run-file-upload]");

    const setStatus = (message) => {
        if (status) status.textContent = message || "";
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
            const item = document.createElement("li");
            const link = document.createElement("a");
            link.href = file.filePath;
            link.textContent = file.originalFileName;
            link.target = "_blank";
            link.rel = "noopener";
            item.appendChild(link);

            if (canEdit) {
                const button = document.createElement("button");
                button.type = "button";
                button.textContent = "Delete";
                button.addEventListener("click", async () => {
                    const deleteResponse = await fetch(`/api/runs/files/${file.id}`, { method: "DELETE" });
                    if (deleteResponse.ok) await loadFiles();
                });
                item.appendChild(button);
            }

            list.appendChild(item);
        }
    };

    uploadForm?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const input = uploadForm.querySelector("input[type='file']");
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

        input.value = "";
        await loadFiles();
    });

    loadFiles();
})();
