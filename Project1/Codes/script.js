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

//----------------------
//  PRINCIPAL FUNCTION 
//----------------------
form.addEventListener("submit",async(e)=>{
    e.preventDefault();
    const message = userInput.value.trim();
    if (!message) return;

    addMessage(message, "user");
    userInput.value = "";

    //Adding message on screen "Thinking..."
    addMessage("Thinking...","bot");

    try{
        const response = await fetch("http://127.0.0.1:5000/chat",{
            method:"POST",
            headers:{
                "Content-Type":"application/json",
            },
            body:JSON.stringify({prompt:message}),
        });

        const data = await response.json();

        //Delete "Thinking..." and replace it 
        chatBox.removeChild(chatBox.lastElementChild);
        addMessage(data.response, "bot");
    } catch (error){
        console.error("Error: ", error);
        chatBox.removeChild(chatBox.lastElementChild);
        addMessage("Error connecting to backend.","bot");
    }
});

//------------------------------
//  FORMAT TEXT SHOW ON SCREEN
//------------------------------
function formatResponse(text){
    
    text = text.replace(/</g,"&lt;").replace(/>/g,"&gt;");

    text = text
        .replace(/\*\*(.*?)\*\*/g,"<strong>$1</strong>") //negrita
        .replace(/\*(.*?)\*/g,"<em>$1</em>") //cursiva
        .replace(/__(.*?)__/g,"<u>$1</u>")  //subrayado
        .replace(/`(.*?)`/g,"<code>$1</code>"); //code
        //Convert text to paragraphs
        const paragraph = text
        .split(/\n\s*\n/)
        .map(p=>`<p>${p.trim()}</p>`)
        .join("");

        return paragraph.replace(/\n/g,"<br>");
}

//----------------------
//  ADD MESSAGE TO CHAT
//----------------------
function addMessage (text, sender){
    const messageDiv = document.createElement("div");
    messageDiv.classList.add("message",sender);

    const bubble = document.createElement("div");
    bubble.classList.add("bubble");
    //Response of Ollama
    bubble.innerHTML = formatResponse(text);

    messageDiv.appendChild(bubble);
    chatBox.appendChild(messageDiv);
    chatBox.scrollTop = chatBox.scrollHeight;
}

//--------------
// MENU ATTACH
//--------------
    attachBtn.addEventListener("click",()=>{
        attachMenu.style.display = attachMenu.style.display === "block"?"none":"block";
    });

     document.addEventListener("click",(e)=>{
        if (!attachBtn.contains(e.target) && !attachMenu.contains(e.target)){
            attachMenu.style.display="none";
        }
    });

    // Image
    document.getElementById("add-image").addEventListener("click",()=> imgInput.click());
   
    // Document
    document.getElementById("add-doc").addEventListener("click",()=> docInput.click());

//-----------------------
// SEND FILE TO BACKEND
//-----------------------
imgInput.addEventListener("change",() => uploadFile(imgInput.files[0]));
docInput.addEventListener("change", () => uploadFile(docInput.files[0]));

async function uploadFile(file) {
    if(!file) return;

    const formData = new FormData();
    formData.append("file",file);
    addMessage(`Uploading: ${file.name}`,"user");

    try{
        const res = await fetch("http://127.0.0.1:5000/upload",{
            method:"POST",
            body:formData
        });

        const data = await res.json();
        addMessage(data.response,"bot");
    } catch(error){
        console.error(error);
        addMessage("Error uploading file","bot");
    }
}

//------------------------
//  HISTORY LOCAL STORAGE
//------------------------

function saveChat(prompt,response){
    const history = JSON.parse(localStorage.getItem("history")||"[]");

    history.push({prompt,response,time:Date.now()});

    localStorage.setItem("history", JSON.stringify(history));
}

historyBtn.addEventListener("click",() => {
    historyMenu.style.display = historyMenu.style.display === "block"?"none":"block";
    renderHistory();
});

document.addEventListener("click",(e)=>{
    if (!historyBtn.contains(e.target) && !historyBtn.contains(e.target)){
        historyMenu.style.display="none";
    }
});

function renderHistory(){
    const history = JSON.parse(localStorage.getItem("history")||"[]");
    chatList.innerHTML="";

    history.forEach((item,i)=>{
        const div = document.createElement("div");
        div.classList.add("chat-item");
        div.textContent="Chat"+(i+1);
        div.onclick= () => loadChat(i);
        chatList.appendChild(div);
    });
}

function loadChat(index){
    const history=JSON.parse(localStorage.getItem("history")||"[]");
    const chat=history[index];
        
    chatBox.innerHTML="";
    addMessage(chatBox.prompt,"user");
    addMessage(chatBox.response,"bot");
}

//-----------
//  NEW CHAT
//-----------

newChatBtn.addEventListener("click",()=>{
    chatBox.innerHTML="";
    addMessage("New chat created!","bot");
});
    


