﻿async function createTableColumn()
{
    disableButton(`#buttonCreateTableColumn`);
    let component_selects = [...document.getElementsByClassName(`Ids`)];
    let length = component_selects.length + 1;
    $.get(`/AltiumDB/GetPartialViewForTableColumn?index=${length}`, function(data) {
        $(`#Tr0`).before(data)
        undisableButton(`#buttonCreateTableColumn`);
        updateIndex(`Ids`);
    });
}

function removeTableColumn(index)
{
    let deviceItems = [...document.querySelectorAll(`.Ids`)];
    let length = deviceItems.length;

    if (length === 0)
    {
        return;
    }

    for (let i = index - 1; i < length - 1; i++)
    {
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

function disableButton(selector)
{
    let button = document.querySelector(selector);
    button.disabled = true;
}

function undisableButton(selector)
{
    let button = document.querySelector(selector);
    button.disabled = false;
}

function updateIndex(selector)
{
    let td_with_ids = [...document.getElementsByClassName(selector)];
    td_with_ids.forEach((elem, index) => elem.innerText  = index + 1);
}

function showDeleteModal(heading, text, action)
{
    document.querySelector('#modal-danger-heading').innerHTML = heading;
    document.querySelector('#modal-danger-text').innerHTML = text;
    let listener = () => {
        action();
        document.querySelector('#modal-danger-delete-button').removeEventListener('click', listener, false)
    };
    
    document.querySelector('#modal-danger-delete-button').addEventListener('click', listener, false);
}

function deleteTableByName(tableName)
{
    return fetch(`/AltiumDB/Tables/${tableName}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok)
        {
            location.reload();
        }
    })
}

function deleteEntityFromTable(tableName, id)
{
    return fetch(`/AltiumDB/Tables/${tableName}/${id}`, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok)
        {
            location.reload();
        }
    })
}