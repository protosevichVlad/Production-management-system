
function initTreeView()
{
    let toggler = document.getElementsByClassName("caret");
    let leaves = document.getElementsByClassName("leaves");

    for (let i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function(event) {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
            document.querySelector("#SelectedDirectoryId").value = event.target.id;

            getPathToDirectory(event.target.id).then(r =>
            {
                document.querySelector("#fullPath").innerHTML = r;
            })
        });
    }

    for (let i = 0; i < leaves.length; i++) {
        leaves[i].addEventListener("click", function(event) {
            document.querySelector("#SelectedDatabaseId").value = event.target.id;

            getPathToTable(event.target.id).then(r =>
            {
                document.querySelector("#fullPath").innerHTML = r;
            })
        });
    }

    openDirectory(document.querySelector("#SelectedDirectoryId").value);
}

async function getPathToTable(id)
{
    let url = `/api/AltiumDB/get-path-by-table-id/${id}`;
    let resp = await fetch(url);
    if (resp.ok)
    {
        return resp.text();
    }

    return '';
}

async function getPathToDirectory(id)
{
    let url = `/api/AltiumDB/get-path-by-directory-id/${id}`;
    let resp = await fetch(url);
    if (resp.ok)
    {
        return resp.text();
    }
    
    return '';
}

function openDirectory(id)
{
    let current = [...toggler].filter(x => x.id == id)[0];
    current?.click();
    while (current != undefined && current.classList!= undefined && current.classList.contains('caret'))
    {
        current.click();
        current = current.parentElement.parentElement.parentElement.childNodes[0];
    }
    
}