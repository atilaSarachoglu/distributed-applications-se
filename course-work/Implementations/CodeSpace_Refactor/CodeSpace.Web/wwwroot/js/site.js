async function loadFeed() {
    const feed = await apiGet("Feed");
    const list = document.getElementById("feed");
    list.innerHTML = "";

    feed.forEach(post => {
        const card = document.createElement("div");
        card.className = "card mb-3";

        // BODY
        const body = document.createElement("div");
        body.className = "card-body";
        const pContent = document.createElement("p");
        pContent.innerText = post.content;
        body.appendChild(pContent);

        // FOOTER (buttons etc.)
        const footer = document.createElement("div");
        footer.className = "card-footer bg-white border-0 pt-0";

        // *** EDIT BUTTON  start
        if (post.canEdit) {                                  // ← value is sent by the API
            const btnEdit = document.createElement("button");
            btnEdit.className = "btn btn-outline-secondary btn-sm me-2";
            btnEdit.innerText = "Edit Post";
            btnEdit.onclick = () => buildEditable(
                pContent,                    // element to replace with textarea
                post.content,                // initial text
                async newText => {           // callback when Save is clicked
                    await apiPut(`Feed/${post.id}`, { content: newText });
                    loadFeed();              // refresh list
                });
            footer.appendChild(btnEdit);
        }
        // *** EDIT BUTTON  end

        card.appendChild(body);
        card.appendChild(footer);
        list.appendChild(card);

        // <<< render replies the same way; for each reply include “Edit Reply” identical
    });
}


function apiGet(path) {
    const jwt = sessionStorage.getItem('jwt');
    return fetch(`https://localhost:64706/api/${path}`, {
        headers: jwt ? { 'Authorization': `Bearer ${jwt}` } : {}
    }).then(r => r.json());
}

function apiPut(path, body) {
    const jwt = sessionStorage.getItem('jwt');
    return fetch(`https://localhost:64706/api/${path}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            ...(jwt && { 'Authorization': `Bearer ${jwt}` })
        },
        body: JSON.stringify(body)
    });
}

function apiDelete(path) {
    const jwt = sessionStorage.getItem('jwt');
    return fetch(`https://localhost:64706/api/${path}`, {
        method: 'DELETE',
        headers: jwt ? { 'Authorization': `Bearer ${jwt}` } : {}
    });
}

