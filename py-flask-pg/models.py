from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()

class Fortune(db.Model):
    __tablename__ = 'fortune'

    id = db.Column(db.Integer(), primary_key = True)
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