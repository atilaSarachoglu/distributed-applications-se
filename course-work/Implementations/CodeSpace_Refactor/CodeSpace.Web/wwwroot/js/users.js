const apiBase = 'https://localhost:64706/api';   // API base URL
const jwt = sessionStorage.getItem('jwt');
const headers = { 'Content-Type': 'application/json', 'Authorization': `Bearer ${jwt}` };

let filterUsername = '';
let filterEmail = '';
let curPage = 1;
let pageSize = 10;
let totalUsers = 0;

function renderPager() {
    const container = document.querySelector('#paginationContainer');
    if (!container) {
        console.error('Pagination container not found!');
        return;
    }

    const totalPages = Math.ceil(totalUsers / pageSize);
    console.log('Pagination calculation:', { totalUsers, pageSize, curPage, totalPages });

    const infoDiv = container.querySelector('.pagination-info');
    const buttonsDiv = container.querySelector('.pagination-controls');
    
    if (!infoDiv || !buttonsDiv) {
        console.error('Pagination elements not found!');
        return;
    }

    if (totalPages <= 1) {
        infoDiv.innerHTML = 'No pagination needed - only one page';
        buttonsDiv.innerHTML = '';
        return;
    }

    infoDiv.innerHTML = `Page ${curPage} of ${totalPages} (${totalUsers} total users)`;
    buttonsDiv.innerHTML = '';

    // First page button
    const firstBtn = document.createElement('button');
    firstBtn.textContent = '<<';
    firstBtn.className = 'pagination-button';
    firstBtn.disabled = curPage === 1;
    firstBtn.onclick = () => { curPage = 1; loadUsers(); };
    buttonsDiv.appendChild(firstBtn);

    // Previous page button
    const prevBtn = document.createElement('button');
    prevBtn.textContent = '<';
    prevBtn.className = 'pagination-button';
    prevBtn.disabled = curPage === 1;
    prevBtn.onclick = () => { curPage--; loadUsers(); };
    buttonsDiv.appendChild(prevBtn);

    // Page numbers
    for (let i = Math.max(1, curPage - 2); i <= Math.min(totalPages, curPage + 2); i++) {
        const pageBtn = document.createElement('button');
        pageBtn.textContent = i;
        pageBtn.className = 'pagination-button';
        pageBtn.disabled = i === curPage;
        pageBtn.onclick = () => { curPage = i; loadUsers(); };
        buttonsDiv.appendChild(pageBtn);
    }

    // Next page button
    const nextBtn = document.createElement('button');
    nextBtn.textContent = '>';
    nextBtn.className = 'pagination-button';
    nextBtn.disabled = curPage === totalPages;
    nextBtn.onclick = () => { curPage++; loadUsers(); };
    buttonsDiv.appendChild(nextBtn);

    // Last page button
    const lastBtn = document.createElement('button');
    lastBtn.textContent = '>>';
    lastBtn.className = 'pagination-button';
    lastBtn.disabled = curPage === totalPages;
    lastBtn.onclick = () => { curPage = totalPages; loadUsers(); };
    buttonsDiv.appendChild(lastBtn);
}

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
    const qs = new URLSearchParams();
    if (filterUsername) qs.append('username', filterUsername);
    if (filterEmail) qs.append('email', filterEmail);
    qs.append('page', curPage);
    qs.append('pageSize', pageSize);

    const res = await fetch(`${apiBase}/Users?${qs}`, { headers });
    if (!res.ok) {
        console.error('Failed to load users:', res.status);
        return;
    }

    totalUsers = parseInt(res.headers.get('X-Total-Count') || '0', 10);
    const data = await res.json();

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
            if (confirm(`Delete user "${u.username}"?`)) {
                await apiDelete(`Users/${u.id}`);
                loadUsers();
            }
        };
        tdAct.appendChild(btnDel);
    });

    renderPager();
}

/* --- search form handlers --- */
document.getElementById('searchForm')?.addEventListener('submit', e => {
    e.preventDefault();
    filterUsername = document.getElementById('searchUsername').value.trim();
    filterEmail = document.getElementById('searchEmail').value.trim();
    curPage = 1; // Reset to first page on new search
    loadUsers();
});

document.getElementById('resetSearch')?.addEventListener('click', () => {
    document.getElementById('searchUsername').value = '';
    document.getElementById('searchEmail').value = '';
    filterUsername = '';
    filterEmail = '';
    curPage = 1; // Reset to first page on reset
    loadUsers();
});

document.addEventListener('DOMContentLoaded', loadUsers);


