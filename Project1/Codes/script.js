//Date 2025/22/05  Inventec Project
//Name Project "Model Deployment and Basic Calling Ollama"
//Vanessa Nava Alberto 

const form = document.getElementById("chat-form");
const userInput = document.getElementById("user-input");
const chatBox = document.getElementById("chat-box");
const historyBtn = document.getElementById("history-btn");
const attachBtn= document.getElementById("attach-btn");

const historyMenu = document.querySelector(".history-menu");
const attachMenu= document.querySelector(".attach-menu");

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

    try{
        addDocument();

    }catch(error){
        console.error("Error: ", error);
        chatBox.removeChild(chatBox.lastElementChild);
        addMessage("Your document is not compatible, sorrry.","bot");
    }

    try{
        history();
    }catch(error){
        console.error("Error: ", error);
        chatBox.removeChild(chatBox.lastElementChild);
        addMessage("Hello?","bot");
    }

});

//Impove format of response of Ollama in the web page 
function formatResponse(text){
    
    //
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
    
        //
        return paragraph.replace(/\n/g,"<br>");
}

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

function addDocument(){

    attachBtn.addEventListener("click",()=>{
    attachMenu.style.display = attachMenu.style.display === "block"?"none":"block";
    });

    document.addEventListener("click",(e)=>{
        if(!attachBtn.contains(e.target)&&!attachMenu.contains(e.target)){
            attachMenu.style.display="none";
        }     
    });

    document.getElementById("add-image").addEventListener("click",()=>{
        alert("Aqui se abrira el selector de imagen");
    });

    document.getElementById("add-doc").addEventListener("click",()=>{
        alert("Aqui se abrira el selector de documentos");
    });
}

function history(){
    historyBtn.addEventListener("click",()=>{
        historyMenu.style.display = historyMenu.style.display === "block"?"none":"block";
    });

    document.addEventListener("click",(e)=>{
        if (!historyBtn.contains(e.target)&&!historyMenu.contains(e.target)){
            historyMenu.style.display="none";
        }
    })
}


