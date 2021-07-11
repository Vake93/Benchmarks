const express = require('express');
const Pool = require('pg').Pool;

const app = express();
const port = 5000;

const pool = new Pool({
    connectionString: "postgresql://user:user@123@localhost:5432/fortunes?sslmode=disable",
    max: 5,
    idleTimeoutMillis: 30000,
    connectionTimeoutMillis: 2000,
})

app.use(express.json());

app.get('/', async (request, response) => {
    var results = await pool.query("SELECT id, message FROM fortune");
    response.status(200).json(results.rows);
});

app.listen(port, () => {
    console.log(`App running on port ${port}.`)
});
