const sessionId = crypto.randomUUID();
async function trackEvent(eventName, properties) {
    const controller = new AbortController();
    const timeoutId = window.setTimeout(() => controller.abort(), 3000);
    try {
        const request = { eventName, sessionId, pagePath: window.location.pathname, properties };
        const response = await fetch("/api/analytics/events", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(request),
            signal: controller.signal
        });
        if (!response.ok) console.debug("Analytics event was not persisted.", response.status);
    } catch {
        console.debug("Analytics endpoint is unavailable.");
    } finally {
        window.clearTimeout(timeoutId);
    }
}
function setStatus(message) {
    const element = document.getElementById("workflow-status");
    if (element) element.textContent = message;
}
void trackEvent("demo_opened", { variant: "demo" });
document.getElementById("start-workflow")?.addEventListener("click", () => {
    setStatus("Workflow started.");
    void trackEvent("workflow_started", { variant: "demo" });
});
document.getElementById("use-feature")?.addEventListener("click", () => {
    setStatus("Optional feature used.");
    void trackEvent("feature_used", { feature: "optional_demo_feature" });
});
document.getElementById("complete-workflow")?.addEventListener("click", () => {
    setStatus("Workflow completed.");
    void trackEvent("workflow_completed", { featureCount: 1, variant: "demo" });
});
document.getElementById("positive-feedback")?.addEventListener("click", () => {
    void trackEvent("feedback_submitted", { feedback: "positive" });
});
document.getElementById("negative-feedback")?.addEventListener("click", () => {
    void trackEvent("feedback_submitted", { feedback: "negative" });
});
