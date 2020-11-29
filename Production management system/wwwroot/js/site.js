﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

function autocomplete(inp, arr) {
    /*the autocomplete function takes two arguments,
    the text field element and an array of possible autocompleted values:*/
    var currentFocus;
    /*execute a function when someone writes in the text field:*/
    inp.addEventListener("input", function (e) {
        var a, b, i, val = this.value;
        /*close any already open lists of autocompleted values*/
        closeAllLists();
        if (!val) { return false; }
        currentFocus = -1;
        /*create a DIV element that will contain the items (values):*/
        a = document.createElement("DIV");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        /*append the DIV element as a child of the autocomplete container:*/
        this.parentNode.appendChild(a);
        /*for each item in the array...*/
        for (i = 0; i < arr.length; i++) {
            /*check if the item starts with the same letters as the text field value:*/
            if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                /*create a DIV element for each matching element:*/
                b = document.createElement("DIV");
                /*make the matching letters bold:*/
                b.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
                b.innerHTML += arr[i].substr(val.length);
                /*insert a input field that will hold the current array item's value:*/
                b.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
                /*execute a function when someone clicks on the item value (DIV element):*/
                b.addEventListener("click", function (e) {
                    /*insert the value for the autocomplete text field:*/
                    inp.value = this.getElementsByTagName("input")[0].value;
                    /*close the list of autocompleted values,
                    (or any other open lists of autocompleted values:*/
                    closeAllLists();
                });
                a.appendChild(b);
            }
        }
    });
    /*execute a function presses a key on the keyboard:*/
    inp.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 13) {
            /*If the ENTER key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) x[currentFocus].click();
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "autocomplete-active":*/
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all autocomplete items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all autocomplete lists in the document,
        except the one passed as an argument:*/
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}
