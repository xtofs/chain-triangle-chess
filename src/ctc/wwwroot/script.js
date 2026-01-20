let selected = undefined;

function select(evt) {
    var target = evt.target;
    var next = target == selected ? undefined : target;
    var prev = selected;

    prev?.classList.remove('selected');
    next?.classList.add('selected');

    selected = next;
}

// Handle vertex selection via HTMX
document.addEventListener('htmx:afterRequest', function (evt) {
    // Check if this is a select request
    if (!evt.detail.xhr.responseURL.includes('/api/select/')) return;

    const target = evt.detail.target;

    // For vertex hits, we need to select the associated visible vertex circle
    // The hit zone is the target, so find the previous sibling vertex circle
    let vertexCircle = target.previousElementSibling;
    while (vertexCircle && !vertexCircle.classList.contains('vertex')) {
        vertexCircle = vertexCircle.previousElementSibling;
    }

    if (vertexCircle) {
        select({ target: vertexCircle });
    }
}, false);

// Handle SVG content loading via HTMX
document.addEventListener('htmx:afterRequest', function (evt) {
    const target = evt.detail.target;

    // Only process <g> elements
    if (target.tagName !== 'g') return;

    const xhr = evt.detail.xhr;
    if (!xhr || !xhr.responseText) return;

    // Parse response as SVG
    const parser = new DOMParser();
    const doc = parser.parseFromString(
        `<svg xmlns="http://www.w3.org/2000/svg">${xhr.responseText}</svg>`,
        'image/svg+xml'
    );

    // Check for parse errors
    if (doc.documentElement.tagName === 'parsererror') {
        console.error('SVG parse error:', doc.documentElement.textContent);
        return;
    }

    // Clear target and insert parsed SVG children
    target.innerHTML = '';
    for (const child of doc.documentElement.childNodes) {
        target.appendChild(document.importNode(child, true));
    }

    // Tell HTMX to process the newly inserted SVG elements so they respond to hx-* attributes
    htmx.process(target);
}, true); // Use capture phase to run early


