from flask_sqlalchemy import SQLAlchemy
from flask import Flask, jsonify
from waitress import serve

app = Flask(__name__)

app.config.from_pyfile('config.py')

db = SQLAlchemy()
db.init_app(app)


@app.route('/text')
def text_handler():
    return "Hello World"


@app.route('/json')
def json_handler():
    return jsonify({
        "id": 1,
        "message": "Hello World",
    })


@app.route('/single')
def single_handler():
    fortunes = Fortune.query.all()
    return jsonify([f.serialize for f in fortunes])


@app.route('/multiple')
def multiple_handler():
    item = World.query.filter_by(id=260).first()
    results = World.query.filter(World.random_number > item.random_number).all()
    return jsonify([r.serialize for r in results])


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


class World(db.Model):
    __tablename__ = 'world'

    id = db.Column(db.Integer(), primary_key=True)
    random_number = db.Column(db.Integer(), name="randomnumber")

    def __init__(self, id, random_number):
        self.id = id
        self.random_number = random_number

    @property
    def serialize(self):
        return {
            'id': self.id,
            'randomNumber': self.random_number
        }


if __name__ == '__main__':
    serve(app, host="0.0.0.0", port=5000, threads=8)
