from flask import Flask, jsonify
from models import Fortune, db
from waitress import serve

app = Flask(__name__)

app.config.from_pyfile('config.py')
db.init_app(app)


@app.route('/')
def hello():
    fortunes = Fortune.query.all()
    return jsonify([f.serialize for f in fortunes])


if __name__ == '__main__':
    serve(app, host="0.0.0.0", port=5000, threads=8)
