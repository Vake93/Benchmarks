from flask_sqlalchemy import SQLAlchemy
from flask import Flask, jsonify
from models import Fortune

app = Flask(__name__)

app.config.from_pyfile('config.py')
db = SQLAlchemy(app)

@app.route('/')
def hello():
    fortunes = Fortune.query.all()
    return jsonify([f.serialize for f in fortunes])

if __name__ == '__main__':
    app.run()