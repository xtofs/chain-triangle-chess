let selectedVertex = undefined;
let reachableVertices = new Set();

// Handle vertex hit zone clicks - implement two-step selection
function onVertexClick(hitZoneElement) {
    // Get the vertex data from the hit zone's attributes
    const row = parseInt(hitZoneElement.getAttribute('data-row'));
    const col = parseInt(hitZoneElement.getAttribute('data-col'));
    const vertexStr = `${row},${col}`;

    // Find the actual vertex circle (previous sibling)
    let vertexCircle = hitZoneElement.previousElementSibling;
    while (vertexCircle && !vertexCircle.classList.contains('vertex')) {
        vertexCircle = vertexCircle.previousElementSibling;
    }

    if (!vertexCircle) return;

    if (!selectedVertex) {
        // First click: select this vertex and fetch reachable vertices
        selectedVertex = { row, col, element: vertexCircle };
        vertexCircle.classList.add('selected');

        // Fetch reachable vertices from server
        fetch(`/api/reachable/${vertexStr}`)
            .then(r => r.json())
            .then(vertices => {
                reachableVertices = new Set(vertices.map(v => `${v.row},${v.col}`));
                // Highlight reachable vertices
                document.querySelectorAll('.vertex').forEach(v => {
                    if (v === vertexCircle) return; // Skip the selected vertex itself
                    if (reachableVertices.has(`${v.getAttribute('data-row')},${v.getAttribute('data-col')}`)) {
                        v.classList.add('reachable');
                    }
                });
            });
    } else {
        // Second click: check if this is a reachable vertex
        if (reachableVertices.has(vertexStr)) {
            // Place the band
            const from = `${selectedVertex.row},${selectedVertex.col}`;
            fetch(`/api/place-band/${from}/${vertexStr}`, { method: 'POST' })
                .then(() => {
                    // Clear selection and reload board
                    clearSelection();
                    htmx.ajax('GET', '/api/game', { target: '#board-dynamic', swap: 'none' });
                });
        } else {
            // Not reachable, deselect or select this vertex instead
            clearSelection();
            onVertexClick(hitZoneElement);
        }
    }
}

function clearSelection() {
    if (selectedVertex) {
        selectedVertex.element.classList.remove('selected');
        selectedVertex = undefined;
    }
    document.querySelectorAll('.vertex.reachable').forEach(v => {
        v.classList.remove('reachable');
    });
    reachableVertices.clear();
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


