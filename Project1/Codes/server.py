#Date 2025/22/05  Inventec Project
#Name Project "Model Deployment and Basic Calling Ollama"
#Vanessa Nava Alberto 

from flask  import Flask, request, jsonify
import requests
from flask_cors import CORS
import json

app = Flask(__name__)
CORS(app)

Ollama_URL = "http://localhost:11434/api/generate"
Model = "deepseek-r1:1.5b"

@app.route("/chat",methods=["POST"])
def chat():
    data = request.get_json()
    prompt = data.get("prompt","")

    try:
        response = requests.post(Ollama_URL,json={"model":Model,"prompt":prompt},stream=True)
        full_response = ""

        for line in response.iter_lines():
            if line:
                json_data = json.loads(line.decode("utf-8"))
                text_part = json_data.get("response","")
                full_response += text_part
            elif json_data.get("done"):
                break
        return jsonify({"response":full_response.strip()})
    except Exception as e:
        return jsonify({"response":f"Error:{e}"}),500
    

if __name__=="__main__":
    app.run(debug=True)