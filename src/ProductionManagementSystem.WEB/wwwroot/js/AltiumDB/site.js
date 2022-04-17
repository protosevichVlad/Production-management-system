CarbonComponents.OverflowMenu.init()

async function createTableColumn() {
    disableButton(`#buttonCreateTableColumn`);
    let component_selects = [...document.getElementsByClassName(`Ids`)];
    let length = component_selects.length + 1;
    $.get(`/Tables/GetPartialViewForTableColumn?index=${length}`, function (data) {
        $(`#Tr0`).before(data)
        undisableButton(`#buttonCreateTableColumn`);
        updateIndex(`Ids`);
    });
}

function removeTableColumn(index) {
    let deviceItems = [...document.querySelectorAll(`.Ids`)];
    let length = deviceItems.length;

    if (length === 0) {
        return;
    }

    for (let i = index - 1; i < length - 1; i++) {
        document.getElementsByName(`TableColumns[${i}].ColumnName`)[0].value =
            document.getElementsByName(`TableColumns[${i + 1}].ColumnName`)[0].value
        document.getElementsByName(`TableColumns[${i}].ColumnType`)[0].value =
            document.getElementsByName(`TableColumns[${i + 1}].ColumnType`)[0].value
        document.getElementsByName(`TableColumns[${i}].Display`)[0].value =
            document.getElementsByName(`TableColumns[${i + 1}].Display`)[0].value
    }

    let tr = document.querySelector(`#Tr${length}`);
    tr.remove();
    updateIndex(`Ids`);
}

function disableButton(selector) {
    let button = document.querySelector(selector);
    button.disabled = true;
}

function undisableButton(selector) {
    let button = document.querySelector(selector);
    button.disabled = false;
}

function updateIndex(selector) {
    let td_with_ids = [...document.getElementsByClassName(selector)];
    td_with_ids.forEach((elem, index) => elem.innerText = index + 1);
}

function showDeleteModal(heading, text, action) {
    document.querySelector('#modal-danger-heading').innerHTML = heading;
    document.querySelector('#modal-danger-text').innerHTML = text;
    let listener = () => {
        action();
        document.querySelector('#modal-danger-delete-button').removeEventListener('click', listener, false)
    };

    document.querySelector('#modal-danger-delete-button').addEventListener('click', listener, false);
}

function deleteTable(id) {
    return fetch(`/Tables/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            location.reload();
        }
    })
}

function deleteEntity(id, action) {
    return fetch(`/Entities/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            if (action == null) {
                location.reload();
            } else {
                action();
            }
        }
    })
}

function deleteProject(id) {
    return fetch(`/pcb/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            location.reload();
        }
    })
}

function deleteDevice(id) {
    return fetch(`/api/devices/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            location.reload();
        }
    })
}

function markNoteAsCompleted(id) {
    return fetch(`/api/AltiumDB/to-do/${id}/completed`, {
        method: 'POST',
    }).then(response => {
        if (response.ok) {
            location.reload();
        }
    })
}

function deleteNote(id)
{
    return fetch(`/api/to-do/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            location.reload();
        }
    })
}

function getDateTime() {
    var date = new Date();
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();
    return (day < 10 ? '0' + day : day) + "/" + (month < 10 ? '0' + month : month) + "/" + year + "_" + (hour < 10 ? '0' + hour : hour) + ':' + (minute < 10 ? '0' + minute : minute) + ':' + (second < 10 ? '0' + second : second);
}

let globalSearch = document.querySelector('#globalSearch');
let globalSearchInput = document.querySelector('#globalSearchInput');
let globalSearchIcon = document.querySelector('#globalSearchIcon');

globalSearchIcon.addEventListener('click', (event) => {
    globalSearchIcon.style.display = 'none';
    globalSearch.style.display = 'flex';
    globalSearchInput.focus();
})

window.addEventListener('click', (event) => {
    let hints = document.querySelector('.altiumdb--global-search--hints');
    if (hints && !hints.contains(event.target)) {
        globalSearch.style.display = 'none';
        globalSearchIcon.style.display = 'block';
        globalSearchInput.value = '';
        $('#global-search--hints').remove();
    }
})

globalSearchInput.addEventListener('input', () => {
    $.get(`/search?q=${globalSearchInput.value}`, function (data) {
        $('#global-search--hints').remove();
        $(`#global-search--close-button`).after(data);
    });
})

function showQuantityModal(entity, type, id) {
    let text = {};
    if (type === 'add')
    {
        text.heading = 'Add to warehouse';
        text.quantityText = 'Quantity';
    }
    else if (type === 'get')
    {
        text.heading = 'Get from the warehouse';
        text.quantityText = 'Quantity';
    }
    
    document.querySelector('#modal-quantity-heading').innerText = text.heading;
    document.querySelector('#modal-quantity-text').innerText = text.quantityText;
    
    let modal = CarbonComponents.Modal.create(document.querySelector('#modal-quantity'));
    modal.show();
    document.querySelector('#modal-quantity-number-input').value = 0;    
    document.querySelector('#modal-quantity-number-input').focus();
    document.querySelector('#modal-quantity-ok-button').onclick = () => {
        modal.hide();
        ChangeQuantity(entity, type, id, document.querySelector('#modal-quantity-number-input').value);
    };
}

function ChangeQuantity(entity, type, id, quantity) {
    request(`/api/${entity}/${id}/${type}`, 'POST', Number(quantity)).then(response => {
        if (response.ok) {
            if (type === "add")
                document.querySelector('#quantity-value').innerText = 
                    Number(document.querySelector('#quantity-value').innerText) + 
                    Number(document.querySelector('#modal-quantity-number-input').value);
            else if(type === "get")
                document.querySelector('#quantity-value').innerText =
                    Number(document.querySelector('#quantity-value').innerText) -
                    Number(document.querySelector('#modal-quantity-number-input').value);
        }
    })
}

async function request(url, method, body) {
    return await fetch(url, {
        method: method,
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
    });
}

const HttpRequest = function (url, method, body)
{
    return request(url, method, body).then(r => r.json());
}

function changeUrl(paramName, value)
{
    
    let arr = location.search.slice(1).split('&').map(x => x.split('='));
    let index = arr.findIndex(x => x[0] === paramName);
    if (index === -1)
    {
        arr.push([paramName, value])
    }
    else {
        arr[index][1] = value;
    }
    location.search = '?' + arr.map(x => x.join('=')).join('&');
}
