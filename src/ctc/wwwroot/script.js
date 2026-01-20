let selected = undefined;

function select(evt) {
    var target = evt.target;
    var next = target == selected ? undefined : target;
    var prev = selected;

    prev?.classList.remove('selected');
    next?.classList.add('selected');

    selected = next;
}

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
}, true); // Use capture phase to run early


