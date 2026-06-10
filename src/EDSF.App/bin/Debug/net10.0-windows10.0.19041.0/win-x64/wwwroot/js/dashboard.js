window.dashboard = {
    charts: {},

    createLine: function (id, labels, data, label) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        if (this.charts[id]) this.charts[id].destroy();
        this.charts[id] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: label || 'Valor',
                    data: data,
                    borderColor: '#1565C0',
                    backgroundColor: 'rgba(21,101,192,0.1)',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' } },
                    x: { grid: { display: false } }
                }
            }
        });
    },

    createDoughnut: function (id, labels, data) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        if (this.charts[id]) this.charts[id].destroy();
        this.charts[id] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: ['#1565C0', '#2E7D32', '#F9A825', '#C62828', '#00838F']
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { position: 'bottom' } }
            }
        });
    },

    createBar: function (id, labels, data, label) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        if (this.charts[id]) this.charts[id].destroy();
        this.charts[id] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: label || 'Valor',
                    data: data,
                    backgroundColor: '#1565C0'
                }]
            },
            options: {
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' } },
                    x: { grid: { display: false } }
                }
            }
        });
    }
};

window.downloadFile = function (filename, base64Content) {
    const bytes = atob(base64Content);
    const array = new Uint8Array(bytes.length);
    for (let i = 0; i < bytes.length; i++) array[i] = bytes.charCodeAt(i);
    const blob = new Blob([array], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.click();
    URL.revokeObjectURL(link.href);
};
