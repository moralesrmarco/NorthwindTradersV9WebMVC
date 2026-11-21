(() => {
    const root = document.getElementById('tableroControlVendedores');
    if (!root) return;

    const charts = {};
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#9b59b6', '#f1c40f', '#e67e22', '#1abc9c', '#34495e', '#e84393', '#fd79a8'];
    const byId = id => document.getElementById(id);
    const endpoint = handler => `/Tableros/${handler}`;

    async function data(handler, parameters) {
        const response = await fetch(`${endpoint(handler)}?${new URLSearchParams(parameters)}`);
        if (!response.ok) throw new Error(`No se pudo obtener ${handler}.`);
        return response.json();
    }    
    function draw(id, config) {
        charts[id]?.destroy();
        charts[id] = new Chart(byId(id), config);
    }
    function privateOptions(title, tooltipCallbacks) {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: title }, legend: { position: 'top' }, tooltip: { callbacks: tooltipCallbacks } },
            scales: {
                x: { grid: { display: false } },
                y: { display: false, beginAtZero: true, grid: { display: false } }
            }
        };
    }
    function groupedDatasets(rows, labels, type) {
        const groups = rows.reduce((result, row) => {
            (result[row.vendedor] ??= []).push(row);
            return result;
        }, {});
        return Object.entries(groups).map(([vendedor, values], index) => ({
            label: vendedor,
            data: labels.map((_, monthIndex) => values.find(x => x.mes === monthIndex + 1)?.indice ?? 0),
            borderColor: colors[index % colors.length],
            backgroundColor: colors[index % colors.length],
            borderWidth: 2,
            pointRadius: type === 'line' ? 4 : 0,
            tension: .3
        }));
    }
    const sellerMonthTooltip = {
        title: context => `Mes: ${context[0].label}`,
        label: context => `Vendedor: ${context.dataset.label}`
    };
    async function loadSellerMonths(canvasId, selectId, type) {
        const anio = byId(selectId).value;
        const rows = await data('VendedoresVentasMensualesPorVendedor', { anio });
        const labels = [...rows].sort((a, b) => a.mes - b.mes)
            .map(x => x.nombreMes).filter((month, index, all) => all.indexOf(month) === index);
        draw(canvasId, {
            type,
            data: { labels, datasets: groupedDatasets(rows, labels, type) },
            options: privateOptions(`Ventas mensuales por vendedor — ${anio < 0 ? 'todos los años' : anio}`, sellerMonthTooltip)
        });
    }
    async function loadComparison() {
        const anios = byId('aniosComparativo').value;
        const rows = await data('VendedoresComparativoVentas', { anios });
        const groups = rows.reduce((result, row) => { (result[row.year] ??= []).push(row); return result; }, {});
        const years = Object.keys(groups).map(Number).sort((a, b) => b - a);
        const labels = years.length ? groups[years[0]].sort((a, b) => a.mes - b.mes).map(x => x.nombreMes) : [];
        draw('chartComparativo', {
            type: 'line',
            data: { labels, datasets: years.map((year, index) => ({ label: `Año ${year}`, data: groups[year].sort((a, b) => a.mes - b.mes).map(x => x.indice), borderColor: colors[index % colors.length], borderWidth: 2, pointRadius: 4, tension: .3 })) },
            options: privateOptions(`Comparativo de ventas mensuales — últimos ${anios} años`, { title: context => `Mes: ${context[0].label}`, label: context => context.dataset.label })
        });
    }
    async function loadProducts() {
        const cantidad = byId('cantidadProductos').value, anio = byId('anioProductos').value;
        const rows = await data('VendedoresTopProductos', { cantidad, anio });
        draw('chartProductos', {
            type: 'bar',
            data: { labels: rows.map((x, index) => `${index + 1}. ${x.nombreProducto}`), datasets: [{ label: 'Productos', data: rows.map(x => x.indice), backgroundColor: rows.map((_, index) => colors[index % colors.length]) }] },
            options: privateOptions(`Top ${cantidad} productos — ${anio < 0 ? 'todos los años' : anio}`, { title: context => `Producto: ${context[0].label}`, label: () => '' })
        });
    }
    async function loadDoughnut() {
        const anio = byId('anioDona').value;
        const rows = await data('VendedoresVentasPorVendedores', { anio });
        draw('chartDona', {
            type: 'doughnut',
            data: { labels: rows.map(x => x.vendedor), datasets: [{ data: rows.map(x => x.indice), backgroundColor: rows.map((_, index) => colors[index % colors.length]), borderColor: '#fff', borderWidth: 2 }] },
            options: { responsive: true, maintainAspectRatio: false, plugins: { title: { display: true, text: `Ventas por vendedor — ${anio < 0 ? 'todos los años' : anio}` }, legend: { position: 'right' }, tooltip: { callbacks: { title: context => `Vendedor: ${context[0].label}`, label: () => '' } } } }
        });
    }
    function safely(action) { action().catch(error => { console.error(error); alert('No fue posible cargar el tablero de vendedores.'); }); }
    const controls = [
        ['anioLinea', () => loadSellerMonths('chartLinea', 'anioLinea', 'line')],
        ['aniosComparativo', loadComparison], ['cantidadProductos', loadProducts], ['anioProductos', loadProducts],
        ['anioBarras', () => loadSellerMonths('chartBarras', 'anioBarras', 'bar')], ['anioDona', loadDoughnut]
    ];
    controls.forEach(([id, action]) => byId(id).addEventListener('change', () => safely(action)));
    controls.forEach(([, action]) => safely(action));
})();
