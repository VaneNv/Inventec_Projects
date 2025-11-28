//Date 2025/22/05  Inventec Project
//Name Project "Model Deployment and Basic Calling Ollama"
//Vanessa Nava Alberto 

//-------------
//  SELECTORS
//-------------
const form = document.getElementById("chat-form");
const userInput = document.getElementById("user-input");
const chatBox = document.getElementById("chat-box");

const historyBtn = document.getElementById("history-btn");
const historyMenu = document.querySelector(".history-menu");
const chatList = document.getElementById("chat-list");
const newChatBtn = document.getElementById("new-chat-btn");

const attachBtn = document.getElementById("attach-btn");
const attachMenu = document.querySelector(".attach-menu");

const imgInput = document.getElementById("file-image-input");
const docInput = document.getElementById("file-doc-input");

let currentChatId = null;  // ID del chat activo
renderHistory();

//----------------------
//  FORM SUBMIT
//----------------------
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    const message = userInput.value.trim();
    if (!message) return;

    // Verificar que haya un chat activo
    if (!currentChatId) {
        alert("Primero debes crear un chat nuevo con el botón 'Nuevo Chat'.");
        return;
    }

    addMessage(message, "user");
    userInput.value = "";

    addMessage("Thinking...", "bot");

    try {
        const response = await fetch("http://127.0.0.1:5000/chat", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ prompt: message }),
        });

        const data = await response.json();

        if (
            chatBox.lastElementChild &&
            chatBox.lastElementChild.querySelector(".bubble") &&
            chatBox.lastElementChild.querySelector(".bubble").innerText === "Thinking..."
        ) {
            chatBox.removeChild(chatBox.lastElementChild);
        }

        const botText = data && data.response ? data.response : "No response from backend";
        addMessage(botText, "bot");

        saveChatMessage("user", message);
        saveChatMessage("bot", botText);

    } catch (error) {
        console.error(error);
        if (
            chatBox.lastElementChild &&
            chatBox.lastElementChild.querySelector(".bubble") &&
            chatBox.lastElementChild.querySelector(".bubble").innerText === "Thinking..."
        ) {
            chatBox.removeChild(chatBox.lastElementChild);
        }
        addMessage("Error connecting to backend.", "bot");
    }
});

//---------------
//  FORMAT TEXT
//---------------
function formatResponse(text) {
    if (text === undefined || text === null) return "";

    text = String(text).replace(/</g, "&lt;").replace(/>/g, "&gt;");

    text = text
        .replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>")
        .replace(/\*(.*?)\*/g, "<em>$1</em>")
        .replace(/__(.*?)__/g, "<u>$1</u>")
        .replace(/`(.*?)`/g, "<code>$1</code>");

    const paragraph = text
        .split(/\n\s*\n/)
        .map(p => `<p>${p.trim()}</p>`)
        .join("");

    return paragraph.replace(/\n/g, "<br>");
}

//----------------------
//  ADD MESSAGE TO CHAT
//----------------------
function addMessage(text, sender) {
    const messageDiv = document.createElement("div");
    messageDiv.classList.add("message", sender);

    const bubble = document.createElement("div");
    bubble.classList.add("bubble");
    bubble.innerHTML = formatResponse(text);

    messageDiv.appendChild(bubble);
    chatBox.appendChild(messageDiv);
    chatBox.scrollTop = chatBox.scrollHeight;
}

//----------------------
//  ATTACH MENU
//----------------------
attachBtn.addEventListener("click", () => {
    attachMenu.style.display = attachMenu.style.display === "block" ? "none" : "block";
});

document.addEventListener("click", (e) => {
    if (!attachBtn.contains(e.target) && !attachMenu.contains(e.target)) {
        attachMenu.style.display = "none";
    }
});

document.getElementById("add-image").addEventListener("click", () => imgInput.click());
document.getElementById("add-doc").addEventListener("click", () => docInput.click());

imgInput.addEventListener("change", () => {
    if (!imgInput.files || !imgInput.files[0] || !currentChatId) return;
    uploadFile(imgInput.files[0]);
    imgInput.value = "";
});
docInput.addEventListener("change", () => {
    if (!docInput.files || !docInput.files[0] || !currentChatId) return;
    uploadFile(docInput.files[0]);
    docInput.value = "";
});

async function uploadFile(file) {
    if (!file || !currentChatId) return;

    const formData = new FormData();
    formData.append("file", file);
    addMessage(`Uploading: ${file.name}`, "user");

    try {
        const res = await fetch("http://127.0.0.1:5000/upload", {
            method: "POST",
            body: formData
        });

        const data = await res.json();
        const botText = data && data.response ? data.response : `File uploaded: ${file.name}`;
        addMessage(botText, "bot");

        saveChatMessage("user", `[file:${file.name}]`);
        saveChatMessage("bot", botText);

    } catch (error) {
        console.error(error);
        addMessage("Error uploading file", "bot");
    }
}

//------------------------
//  SAVE CHAT MESSAGE
//------------------------
function saveChatMessage(role, content) {
    if (!currentChatId) return;

    const chats = JSON.parse(localStorage.getItem("history") || '{"chats":[]}').chats;
    const chat = chats.find(c => c.id === currentChatId);
    if (!chat) return;

    chat.messages.push({ role, content });
    localStorage.setItem("history", JSON.stringify({ chats }));
    renderHistory();
}

//----------------------
//  RENDER HISTORY MENU
//----------------------
historyBtn.addEventListener("click", () => {
    historyMenu.style.display = historyMenu.style.display === "block" ? "none" : "block";
    renderHistory();
});

document.addEventListener("click", (e) => {
    if (!historyBtn.contains(e.target) && !historyMenu.contains(e.target)) {
        historyMenu.style.display = "none";
    }
});

function renderHistory() {
    const chats = JSON.parse(localStorage.getItem("history") || '{"chats":[]}').chats;
    chatList.innerHTML = "";

    if (!chats || chats.length === 0) {
        const empty = document.createElement("div");
        empty.classList.add("chat-item");
        empty.textContent = "No chats yet";
        chatList.appendChild(empty);
        return;
    }

    chats.forEach((chat) => {
        const div = document.createElement("div");
        div.classList.add("chat-item");
        const lastMessage = chat.messages.length > 0 ? chat.messages[chat.messages.length - 1].content : "Empty chat";
        div.textContent = lastMessage.length > 30 ? lastMessage.slice(0, 30) + "..." : lastMessage;
        div.onclick = () => loadChat(chat.id);
        chatList.appendChild(div);
    });
}

//-------------
//  LOAD CHAT
//-------------
function loadChat(id) {
    const chats = JSON.parse(localStorage.getItem("history") || '{"chats":[]}').chats;
    const chat = chats.find(c => c.id === id);
    if (!chat) return;

    currentChatId = chat.id;
    chatBox.innerHTML = "";
    chat.messages.forEach(m => addMessage(m.content, m.role));
}

//------------
//  NEW CHAT
//------------
newChatBtn.addEventListener("click", () => {
    const chats = JSON.parse(localStorage.getItem("history") || '{"chats":[]}').chats;
    const newId = Date.now();

    const newChat = {
        id: newId,
        title: "New Chat",
        messages: [],
        time: newId
    };

    chats.push(newChat);
    localStorage.setItem("history", JSON.stringify({ chats }));
    currentChatId = newId;
    chatBox.innerHTML = "";
    addMessage("New chat created!", "bot");
    renderHistory();
});