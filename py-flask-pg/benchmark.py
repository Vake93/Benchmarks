from flask_sqlalchemy import SQLAlchemy
from flask import Flask, jsonify
from waitress import serve

app = Flask(__name__)

app.config.from_pyfile('config.py')

db = SQLAlchemy()
db.init_app(app)


@app.route('/')
def hello():
    fortunes = Fortune.query.all()
    return jsonify([f.serialize for f in fortunes])


class Fortune(db.Model):
    __tablename__ = 'fortune'

    id = db.Column(db.Integer(), primary_key=True)
    message = db.Column(db.String())

    def __init__(self, id, message):
        self.id = id
        self.message = message

    @property
    def serialize(self):
        return {
            'id': self.id,
            'message': self.message
        }


if __name__ == '__main__':
    serve(app, host="0.0.0.0", port=5000, threads=8)
