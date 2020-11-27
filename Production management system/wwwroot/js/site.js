// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


async function createComponent() {
    let component_selects = [...document.getElementsByClassName('ComponentSelect')];
    let length = component_selects.length + 1;
    let str = await createTextComponent(length);

    let lastTr = document.querySelector(`#comTr${length - 1}`);
    lastTr.insertAdjacentHTML('afterEnd', str);
}

async function createDesign() {
    let design_selects = [...document.getElementsByClassName('DesignSelect')];
    let length = design_selects.length + 1;
    let str = await createTextDesign(length);

    let lastTr = document.querySelector(`#desTr${length - 1}`);
    lastTr.insertAdjacentHTML('afterEnd', str);
}

async function createTextComponent(id) {
    let str = `<tr id="comTr${id}"><td>${id}</td><td><select class="ComponentSelect align-top width-100" id="Component${id}" name="Component${id}">`;

    let r = await new Request('/Component/GetAllComponents');
    let componentsJson = await fetch(r).then(c => c.json());

    for (let i = 0; i < componentsJson.length; i++) {
        let c = componentsJson[i]
        str += `<option value="${c.id}">${c.name}</option>`;
    }
    str += `</select></td><td><input class="ComponentInput align-top" id="Component${id}Input" name="Component${id}Input" type="number" required autocomplete="off" min="0" />`;
    str += `</td><td><textarea name="Component${id}Text" class="align-top w-100"></textarea></td></tr>`;
    return str;
}

async function createTextDesign(id) {
    let str = `<tr id="desTr${id}"><td>${id}</td><td><select class="DesignSelect align-top width-100" id="Design${id}" name="Design${id}">`;

    let r = await new Request('/Design/GetAllDesigns');
    let componentsJson = await fetch(r).then(c => c.json());

    for (let i = 0; i < componentsJson.length; i++) {
        let c = componentsJson[i]
        str += `<option value="${c.id}">${c.name}</option>`;
    }
    str += `</select></td><td><input class="DesignInput align-top" id="Design${id}Input" name="Design${id}Input" type="number" required autocomplete="off" min="0" />`;
    str += `</td><td><textarea name="Design${id}Text" class="align-top w-100"></textarea></td></tr>`;
    return str;
}

function removeComponent() {
    let component_selects = [...document.getElementsByClassName('ComponentSelect')];
    let length = component_selects.length;

    let lastTr = document.querySelector(`#comTr${length}`);
    lastTr.remove()
}

function removeDesign() {
    let design_selects = [...document.getElementsByClassName('DesignSelect')];
    let length = design_selects.length;

    let lastTr = document.querySelector(`#desTr${length}`);
    lastTr.remove()
}
