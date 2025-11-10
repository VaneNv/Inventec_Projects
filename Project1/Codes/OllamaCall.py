#Date 2025/22/05  Inventec Project
#Name Project "Model Deployment and Basic Calling Ollama"
#Vanessa Nava Alberto 

import requests 
import json 

def chat_Ollama():

    print("---------------------------------------")
    print("\n Ollama...Calling...\n")
    print("---------------------------------------\n")
    
    url_Ollama="http://localhost:11434/api/generate"
    model="deepseek-r1:1.5b"

    print("Ollama (model Deepseek-r1:1.5b)\n")
    print("---------------------------------------")
    print("Write 'Out' to exit...\n")

    while True:
        user_input=input("You: ")
        if user_input.lower() in ["Out","Exit","Quit","out","exit","quit"]:
            print("\nChat End\nSee you soon!\n")
            print("---------------------------------------\n\n")
            break

        #Body 
        data={
            "model":model,
            "prompt":user_input
        }

        try:
            #Call Post local to Ollama
            response = requests.post(url_Ollama,json=data,stream=True)
            response.raise_for_status()

            #Print Ollama anwser
            print("\nOllama:",end=" ",flush=True)
            full_response = ""

            #process anwser by line
            for line in response.iter_lines():
                if line:
                    json_data = json.loads(line.decode("utf-8"))
                    text_part=json_data["response"]
                    full_response += text_part
                    print(text_part,end="",flush=True)
                elif json_data.get("done"):
                    break
            
            print("\n")

        except requests.exceptions.RequestException as e:
            print(f"\nError to connect Ollama: {e}\n")

        
if __name__=="__main__":
        chat_Ollama()

