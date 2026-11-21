(() => {
    const root = document.getElementById('tableroAltaDireccion');
    if (!root) return;

    const select = id => document.getElementById(id);
    const anioMensual = select('anioMensual');
    const aniosComparativo = select('aniosComparativo');
    const cantidadProductos = select('cantidadProductos');
    const anioProductos = select('anioProductos');
    const anioVendedores = select('anioVendedores');
    const anioVendedoresLinea = select('anioVendedoresLinea');
    const anioVendedoresBarra = select('anioVendedoresBarra');
    Chart.register(ChartDataLabels);
    const charts = {};
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#9b59b6', '#f1c40f', '#e67e22', '#1abc9c', '#34495e'];
    const money = value => new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN', maximumFractionDigits: 0 }).format(value);
    const url = handler => `/Tableros/${handler}`;

    async function getJson(handler, params) {
        const response = await fetch(`${url(handler)}?${new URLSearchParams(params)}`);

        if (!response.ok)
            throw new Error(`No fue posible cargar ${handler}.`);

        return response.json();
    }

    function replaceChart(id, config) {
        charts[id]?.destroy();
        charts[id] = new Chart(document.getElementById(id), config);
    }
    function baseOptions(title) {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: title },
                legend: { position: 'top' },
                datalabels: {
                    display: context => Number(context.dataset.data[context.dataIndex]) !== 0,
                    anchor: 'end',
                    align: 'top',
                    color: '#212529',
                    formatter: value => money(value),
                    font: { size: 10, weight: 'bold' }
                },
                tooltip: { callbacks: { label: c => `${c.dataset.label || c.label}: ${money(c.parsed.y ?? c.parsed)}` } }
            },
            scales: { y: { beginAtZero: true, ticks: { callback: money } }, x: { grid: { display: false } } }
        };
    }
    async function loadMonthly() {
        const anio = anioMensual.value, data = await getJson('VentasMensuales', { anio });
        replaceChart('chartMensual', { type: 'line', data: { labels: data.map(x => x.nombreMes), datasets: [{ label: 'Ventas', data: data.map(x => x.total), borderColor: colors[0], backgroundColor: `${colors[0]}33`, fill: true, tension: .35 }] }, options: baseOptions(anio < 0 ? 'Ventas mensuales — todos los años' : `Ventas mensuales — ${anio}`) });
    }
    async function loadComparison() {
        const anios = aniosComparativo.value, data = await getJson('ComparativoVentas', { anios });
        const grouped = data.reduce((groups, item) => {
            (groups[item.year] ??= []).push(item);
            return groups;
        }, {});
        const years = Object.keys(grouped).map(Number).sort((a, b) => b - a);
        replaceChart('chartComparativo', { type: 'line', data: { labels: years.length ? grouped[years[0]].sort((a, b) => a.mes - b.mes).map(x => x.nombreMes) : [], datasets: years.map((year, i) => ({ label: `Ventas ${year}`, data: grouped[year].sort((a, b) => a.mes - b.mes).map(x => x.total), borderColor: colors[i % colors.length], tension: .35 })) }, options: baseOptions(`Comparativo de los últimos ${anios} años`) });
    }
    async function loadProducts() {
        const cantidad = cantidadProductos.value, anio = anioProductos.value, data = await getJson('TopProductos', { cantidad, anio });
        replaceChart('chartProductos', { type: 'bar', data: { labels: data.productos.map((x, i) => `${i + 1}. ${x.nombreProducto}`), datasets: [{ label: 'Unidades vendidas', data: data.productos.map(x => x.cantidadVendida), backgroundColor: data.productos.map((_, i) => colors[i % colors.length]) }] }, options: { ...baseOptions(`Top ${cantidad} productos — ${anio < 0 ? 'todos los años' : anio}`), plugins: { ...baseOptions('').plugins, datalabels: { anchor: 'end', align: 'top', color: '#212529', formatter: value => new Intl.NumberFormat('es-MX').format(value), font: { size: 10, weight: 'bold' } }, tooltip: { callbacks: { label: c => `Unidades: ${new Intl.NumberFormat('es-MX').format(c.parsed.y)}` } } }, scales: { y: { beginAtZero: true }, x: { ticks: { maxRotation: 45, minRotation: 45 } } } } });
    }
    async function loadSellers() {
        const anio = anioVendedores.value, data = await getJson('VentasPorVendedores', { anio });
        replaceChart('chartVendedores', { type: 'doughnut', data: { labels: data.vendedores.map(x => x.vendedor), datasets: [{ data: data.vendedores.map(x => x.totalVentas), backgroundColor: data.vendedores.map((_, i) => colors[i % colors.length]) }] }, options: { responsive: true, maintainAspectRatio: false, plugins: { title: { display: true, text: `Ventas por vendedor — ${anio < 0 ? 'todos los años' : anio}` }, datalabels: { color: '#fff', formatter: (value, context) => `${context.chart.data.labels[context.dataIndex]}\n${money(value)}`, font: { size: 10, weight: 'bold' }, textAlign: 'center' }, tooltip: { callbacks: { label: c => `${c.label}: ${money(c.parsed)}` } }, legend: { position: 'right' } } } });
    }
    async function loadMonthlySellers(canvasId, selectId, type) {
        const anio = document.getElementById(selectId).value, data = await getJson('VentasMensualesPorVendedor', { anio });
        const grouped = data.reduce((groups, item) => {
            (groups[item.vendedor] ??= []).push(item);
            return groups;
        }, {}), sellers = Object.keys(grouped);
        const labels = data.length ? [...data].sort((a, b) => a.mes - b.mes).map(x => x.nombreMes).filter((x, i, all) => all.indexOf(x) === i) : [];
        replaceChart(canvasId, { type, data: { labels, datasets: sellers.map((seller, i) => ({ label: seller, data: labels.map((_, month) => grouped[seller].find(x => x.mes === month + 1)?.totalVentas ?? 0), borderColor: colors[i % colors.length], backgroundColor: colors[i % colors.length], tension: .3 })) }, options: baseOptions(`${type === 'line' ? 'Ventas mensuales' : 'Comparativo mensual'} por vendedor — ${anio < 0 ? 'todos los años' : anio}`) });
    }
    function safe(action) { action().catch(error => { console.error(error); alert('Ocurrió un error al cargar los datos del tablero.'); }); }
    const controls = [
        [anioMensual, loadMonthly], [aniosComparativo, loadComparison], [cantidadProductos, loadProducts], [anioProductos, loadProducts], [anioVendedores, loadSellers],
        [anioVendedoresLinea, () => loadMonthlySellers('chartVendedoresLinea', 'anioVendedoresLinea', 'line')], [anioVendedoresBarra, () => loadMonthlySellers('chartVendedoresBarra', 'anioVendedoresBarra', 'bar')]
    ];
    controls.forEach(([element, action]) => element.addEventListener('change', () => safe(action)));
    safe(loadMonthly); safe(loadComparison); safe(loadProducts); safe(loadSellers);
    safe(() => loadMonthlySellers('chartVendedoresLinea', 'anioVendedoresLinea', 'line'));
    safe(() => loadMonthlySellers('chartVendedoresBarra', 'anioVendedoresBarra', 'bar'));
})();
