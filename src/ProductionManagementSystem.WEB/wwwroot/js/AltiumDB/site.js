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
  return HttpRequest(`/Tables/${id}`, 'DELETE')
    .then(x => location.reload());
}

function deleteEntity(id, action) {
  return HttpRequest(`/Entities/${id}`, 'DELETE')
    .then(x => {
      if (action == null) {
        location.reload();
      } else {
        action();
      }
    });
}

function deleteProject(id) {
  return HttpRequest(`/pcb/${id}`, 'DELETE')
    .then(_ => location.reload())
}

function deleteDevice(id) {
  return HttpRequest(`/api/devices/${id}`, 'DELETE')
    .then(x => location.reload())
    .catch(console.log);
}

function markNoteAsCompleted(id) {
  return HttpRequest(`/api/AltiumDB/to-do/${id}/completed`, 'POST')
    .then(x => location.reload())
    .catch(console.log);
}

function deleteNote(id) {
  return HttpRequest(`/api/to-do/${id}`, 'DELETE')
    .then(x => location.reload())
    .catch(console.log);
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
  if (type === 'add') {
    text.heading = 'Добавить на склад';
    text.quantityText = 'Количество';
  } else if (type === 'get') {
    text.heading = 'Получить со склада';
    text.quantityText = 'Количество';
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
      else if (type === "get")
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

const HttpRequest = function (url, method, body) {
  return request(url, method, body).then(r => {
    if (r.ok)
      return r.json().catch(console.log)
    else {
      return r.json()
        .then(e => {
          if (e.message != null && e.title != null) {
            showPassiveModal(e.title, e.message);
          }

          throw e;
        });
    }
  });
}

const showPassiveModal = (title, message) =>
{
  let modal = CarbonComponents.Modal.create(document.querySelector('#passive-modal'));
  document.querySelector('#passive-modal-heading').innerHTML = title;
  document.querySelector('#passive-modal-message').innerHTML = message;
  modal.show();
}

const showError = (message) => showPassiveModal("Ошибка", message);

function formDataAppend(formData, prop, value, deep)
{
  if (deep === 0) return;
  
  if (Array.isArray(value))
  {
    for (let index in value)
    {
      formDataAppend(formData, `${prop}[${index}]`, value[index], deep - 1)
    }
  }
  else if (typeof value === 'object' && value !== null && !(value instanceof File)){
    for (let valueProp in value)
    {
      formDataAppend(formData, `${prop}.${valueProp}`, value[valueProp], deep - 1)
    }
  }
  else {
    formData.append(prop, value);
  }
}

function getFormDataFromObj(obj, deep){
  let formData = new FormData();
  for (var prop in obj)
  {
    formDataAppend(formData, prop, obj[prop], deep == null ? -1 : deep + 1);
  }
  return formData;
}

function sendObjectAsFormData(url, method, obj, deep)
{
  let formData = getFormDataFromObj(obj, deep);
  return fetch(url, {
    method : method,
    body : formData
  })
    .then(r => r.json())
    .catch(e => console.log(e));
}

function changeUrl(paramName, value) {

  let arr = location.search.slice(1).split('&').map(x => x.split('='));
  let index = arr.findIndex(x => x[0] === paramName);
  if (index === -1) {
    arr.push([paramName, value])
  } else {
    arr[index][1] = value;
  }
  location.search = '?' + arr.map(x => x.join('=')).join('&');
}

function deleteTask(id, action) {
  showDeleteModal('Удаление задачи', 'Вы действительно хотите удалить эту задачу?', () => {
    HttpRequest(`/api/tasks/${id}`, 'DELETE').then(action).catch(e => showPassiveModal("Error", e));
  });
}

function deletePcb(id, action) {
  showDeleteModal('Удаление Pcb', 'Вы действительно хотите удалить эту pcb?', () => {
    HttpRequest(`/api/pcb/${id}`, 'DELETE').then(action).catch(e => showPassiveModal("Error", e));
  });
}

function STLViewer(model, elementID) {
  var elem = document.getElementById(elementID);

  var camera = new THREE.PerspectiveCamera(70,
    elem.clientWidth/elem.clientHeight, 1, 1000);

  var renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.setSize(elem.clientWidth, elem.clientHeight);
  elem.appendChild(renderer.domElement);
  window.addEventListener('resize', function () {
    renderer.setSize(elem.clientWidth, elem.clientHeight);
    camera.aspect = elem.clientWidth/elem.clientHeight;
    camera.updateProjectionMatrix();
  }, false);

  var controls = new THREE.OrbitControls(camera, renderer.domElement);
  controls.enableDamping = true;
  controls.rotateSpeed = 0.4;
  controls.dampingFactor = 0.4;
  controls.enableZoom = true;
  controls.autoRotate = true;
  controls.autoRotateSpeed = .75;

  var scene = new THREE.Scene();
  const light = new THREE.PointLight( 0xffffff, 0.75, 100 );
  light.position.set( 25, 25, 25 );
  scene.add(light);
  const light1 = new THREE.PointLight( 0xffffff, 0.25, 100 );
  light1.position.set( 25, 25, -25 );
  scene.add(light1);
  scene.add(new THREE.AmbientLight(0xffffff, 1.5));
  (new THREE.STLLoader()).load(model, function (geometry) {
    var material = new THREE.MeshPhongMaterial({
      color: 0x6c6c6c,
      specular: 100,
      shininess: 100 });
    var mesh = new THREE.Mesh(geometry, material);
    scene.add(mesh);

    var middle = new THREE.Vector3();
    geometry.computeBoundingBox();
    geometry.boundingBox.getCenter(middle);
    mesh.geometry.applyMatrix(new THREE.Matrix4().makeTranslation(
      -middle.x, -middle.y, -middle.z ) );
    var largestDimension = Math.max(geometry.boundingBox.max.x,
      geometry.boundingBox.max.y,
      geometry.boundingBox.max.z)
    camera.position.z = largestDimension * 2;

    var animate = function () {
      requestAnimationFrame(animate);
      controls.update();
      renderer.render(scene, camera);
    };

    animate();
  });
}
