// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function createDeviceItem(type)
{
    disableButton(`#buttonCreate${type}`);
    let component_selects = [...document.getElementsByClassName(`${type}sSelect`)];
    let length = component_selects.length;
    $.get(`/devices/GetPartialViewForDeviceItem?type=${type}&index=${length}`, function(data) {
        $(`#${type}Tr0`).before(data)
        $('.js-example-basic-single').select2();
    });

    updateIndex(`${type}sIds`);
    undisableButton(`#buttonCreate${type}`);
}

function removeMontages(index) {
    let component_selects = [...document.getElementsByClassName('MontagesSelect')];
    let length = component_selects.length;

    if (length === 0)
    {
        return;
    }
    
    let tr = document.querySelector(`#MontagesTr${index}`);
    tr.remove();
    updateIndex('MontagesIds');
}

function removeDesigns(index) {
    let design_selects = [...document.getElementsByClassName('DesignsSelect')];
    let length = design_selects.length;

    if (length === 0)
    {
        return;
    }
    
    let tr = document.querySelector(`#DesignsTr${index}`);
    tr.remove();
    updateIndex('DesignsIds');
}

function disableButton(selector)
{
    let button = document.querySelector(selector);
    button.disabled = true;
    button.querySelector('.spinner-border').hidden = false;
}

function undisableButton(selector)
{
    let button = document.querySelector(selector);
    button.disabled = false;
    button.querySelector('.spinner-border').hidden = true;
}

function updateIndex(selector)
{
    let td_with_ids = [...document.getElementsByClassName(selector)];
    td_with_ids.forEach((elem, index) => elem.innerText  = index + 1);
}

async function createDevice() {
    disableButton('#buttonCreateDevice');
    let component_selects = [...document.getElementsByClassName('DeviceSelect')];
    let length = component_selects.length + 1;
    let str = await createTextDevice(length);

    let lastTr = document.querySelector(`#devTr${length - 1}`);
    lastTr.insertAdjacentHTML('afterEnd', str);
    updateIndex('DeviceIds');
    undisableButton('#buttonCreateDevice');
}

async function createTextDevice(id) {
    let str = `<tr id="devTr${id}"><td class="DeviceIds"></td><td><select class="DeviceSelect align-top width-100 form-select" name="DeviceIds">`;

    let r = await new Request('/Devices/GetAllDevices');
    let devicesJson = await fetch(r).then(c => c.json());

    for (let i = 0; i < devicesJson.length; i++) {
        let c = devicesJson[i]
        str += `<option value="${c.id}">${c.name}</option>`;
    }
    str += `</select></td><td><input class="DeviceInput align-top form-control" name="DeviceQuantity" type="number" required autocomplete="off" min="0" />`;
    str += `</td><td><textarea name="DeviceDescriptions" class="align-top form-control w-100"></textarea></td>`;
    str += `<td class="border-0"><button type="button" class="btn-close" aria-label="Close" onclick="removeDevice(${id})"></button></td></tr>`;
    return str;
}

function removeDevice(index) {
    let device_selects = [...document.getElementsByClassName('DeviceSelect')];
    let length = device_selects.length;

    if (length === 0)
    {
        return;
    }
    
    let tr = document.querySelector(`#devTr${index}`);
    tr.remove();
    updateIndex('DeviceIds');
}

function changeStateTransfer()
{
    let selector = document.querySelector('#to');
    if (selector.value == 32 || selector.value == 64)
    {
        document.querySelector('#radioTransferTrue').checked = true;
        document.querySelector('#radioTransferFalse').disabled = true;
    }
    else 
    {
        document.querySelector('#radioTransferFalse').disabled = false;
    }
}

