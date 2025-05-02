/* helper – post/put/get/delete were already defined in site.js */

/* turn a row into inline-edit mode */
function activateEditor(tr, user) {
    tr.innerHTML = '';                // wipe existing cells

    // --- Id (read-only) ---
    tr.insertCell().textContent = user.id;

    // --- Username ---
    const tdUsername = tr.insertCell();
    const inUsername = document.createElement('input');
    inUsername.type = 'text';
    inUsername.value = user.username;
    inUsername.className = 'form-control form-control-sm';
    tdUsername.appendChild(inUsername);

    // --- Email ---
    const tdEmail = tr.insertCell();
    const inEmail = document.createElement('input');
    inEmail.type = 'email';
    inEmail.value = user.email ?? '';
    inEmail.className = 'form-control form-control-sm';
    tdEmail.appendChild(inEmail);

    // --- IsAdmin (checkbox) ---
    const tdAdmin = tr.insertCell();
    const chkAdmin = document.createElement('input');
    chkAdmin.type = 'checkbox';
    chkAdmin.checked = user.isAdmin;
    tdAdmin.className = 'text-center align-middle';
    tdAdmin.appendChild(chkAdmin);

    // --- Password (new) ---
    const tdPwd = tr.insertCell();
    const inPwd = document.createElement('input');
    inPwd.type = 'password';
    inPwd.placeholder = '••••••';
    inPwd.className = 'form-control form-control-sm';
    tdPwd.appendChild(inPwd);

    // --- Save / Cancel buttons ---
    const tdAct = tr.insertCell();

    const btnSave = document.createElement('button');
    btnSave.className = 'btn btn-sm btn-primary me-1';
    btnSave.textContent = 'Save';

    const btnCancel = document.createElement('button');
    btnCancel.className = 'btn btn-sm btn-secondary';
    btnCancel.textContent = 'Cancel';

    tdAct.appendChild(btnSave);
    tdAct.appendChild(btnCancel);

    btnSave.onclick = async () => {
        await apiPut(`Users/${user.id}`, {
            username: inUsername.value.trim(),
            email: inEmail.value.trim() || null,
            password: inPwd.value.trim() || null,   // null ⇒ keep old
            isAdmin: chkAdmin.checked
        });
        loadUsers();              // redraw table
    };

    btnCancel.onclick = loadUsers; // revert by reloading
}

/* ---------- load + render ---------- */
async function loadUsers() {
    const data = await apiGet('Users');
    const tbody = document.querySelector('#usersTable tbody');
    tbody.innerHTML = '';

    data.forEach(u => {
        const tr = tbody.insertRow();

        tr.insertCell().textContent = u.id;
        tr.insertCell().textContent = u.username;
        tr.insertCell().textContent = u.email ?? '';
        tr.insertCell().innerHTML = u.isAdmin ? '✔️' : '';

        const tdAct = tr.insertCell();

        // Edit
        const btnEdit = document.createElement('button');
        btnEdit.className = 'btn btn-sm btn-outline-secondary me-1';
        btnEdit.textContent = 'Edit';
        btnEdit.onclick = () => activateEditor(tr, u);
        tdAct.appendChild(btnEdit);

        // Delete
        const btnDel = document.createElement('button');
        btnDel.className = 'btn btn-sm btn-outline-danger';
        btnDel.textContent = 'Delete';
        btnDel.onclick = async () => {
            if (confirm(`Delete user “${u.username}”?`)) {
                await apiDelete(`Users/${u.id}`);
                loadUsers();
            }
        };
        tdAct.appendChild(btnDel);
    });
}

document.addEventListener('DOMContentLoaded', loadUsers);
