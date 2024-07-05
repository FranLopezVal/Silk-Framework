const __PORT__ = '80';                      // Silk Framework edit this port to the port of your server (literal change in a lifetime cycle of silk)
const __IP__ = '127.0.0.1';                 // Silk open this file and change. IP AND PORT (not change the names of the variables)

__refreshTime__ = 2000;
intervalLog = 0;

class LogApi {
    Date;
    Id;
    Level;
    Msg;
}

const onInitFetch = async function () {
    intervalLog = setInterval(() => {
        UpdateSilkLogs();
        FetchSilkServerInfo();
    }, __refreshTime__);
}

const changeTime = function () {
    const refreshTime = document.getElementById('refreshTime');
    __refreshTime__ = refreshTime.value * 1000;

    if (Number.isNaN(__refreshTime__)) {
        __refreshTime__ = 2000;
    }

    clearInterval(intervalLog);
    intervalLog = setInterval(() => {
        UpdateSilkLogs();
        FetchSilkServerInfo();
    }, __refreshTime__);
}

const UpdateSilkLogs = async function () {
    let data;
    await fetch(`http://${__IP__}:${__PORT__}/api/monitor/logs`)
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.json();
        })
        .then((json) => {
            data = json;
        })
        .catch(function () {
            console.error("An error occurred while fetching the JSON data.");
        });
    if (data && data.length > 0) {
        const tableBody = document.getElementById('logsTable').getElementsByTagName('tbody')[0];

        const autorefresh = document.getElementById('autoRefresh');
        const refreshTime = document.getElementById('refreshTime');

        if (!autorefresh.checked) {
            return;
        }

        // clear the table
        tableBody.innerHTML = '';

        data = data.reverse();

        data.forEach(log => {
            const row = tableBody.insertRow();

            const dateCell = row.insertCell(0);
            const idCell = row.insertCell(1);
            const levelCell = row.insertCell(2);
            const msgCell = row.insertCell(3);

            dateCell.textContent = log.Date;
            idCell.textContent = log.Id;
            levelCell.textContent = log.Level;
            msgCell.textContent = log.Msg;

            row.classList = 'row-log ' + log.Level;
        });

        // scroll to the bottom
        const logsTable = document.getElementById('logsTableContainer');
        logsTable.scrollTop = logsTable.scrollHeight;

    }
}

const FetchSilkServerInfo = async function () {
    let data;
    await fetch(`http://${__IP__}:${__PORT__}/api/monitor/serverinfo`)
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.json();
        })
        .then((json) => {
            data = json;
        })
        .catch(function () {
            console.error("An error occurred while fetching the JSON data.");
        });
    if (data) {
        const serverName = document.getElementById('serverName');
        const serverIP = document.getElementById('serverIP');
        const serverPort = document.getElementById('serverPort');
        const serverStatus = document.getElementById('serverStatus');
        const serverVersion = document.getElementById('serverVersion');
        const serverUptime = document.getElementById('serverUptime');

        serverName.innerHTML = data.serverName;
        serverIP.innerText = data.serverIP;
        serverPort.innerText = data.serverPort;
        serverStatus.innerText = data.serverStatus;
        serverVersion.innerText = data.serverVersion;
        serverUptime.innerText = formatDate(data.serverUptime);
    }
}

const formatDate = function (date) {
    const fdate = new Date(date);
    const options = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false,
        timeZoneName: 'short' // Mostrar nombre corto de la zona horaria
    };

    return fdate.toLocaleDateString('en-EN', options);
}

const GetSilkUrls = async function () {
    let data;
    await fetch(`http://${__IP__}:${__PORT__}/api/monitor/urls`)
        .then((response) => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.json();
        })
        .then((json) => {
            data = json;
        })
        .catch(function () {
            console.error("An error occurred while fetching the JSON data.");
        });
    if (data && data.length > 0) {
        const routesTable = document.getElementById('pathTables').getElementsByTagName('tbody')[0];
        
        // clear the table
        routesTable.innerHTML = '';        

        data.forEach(log => {
            const row = routesTable.insertRow();

            const urlCell = row.insertCell(0);
            const methodCell = row.insertCell(1);
            const actionCell = row.insertCell(2);

            urlCell.textContent = log.url;
            methodCell.textContent = log.method;
            actionCell.innerHTML = `<button class="btn-link" onclick="OpenUrl('${log.url}')">Open</button>`;

            row.classList = 'row-log';
        });
    }
}

OpenUrl = function (url) {
    window.open(url, '_blank');
}





GetSilkUrls();
// RUNTIME
onInitFetch();