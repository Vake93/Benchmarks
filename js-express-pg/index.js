const express = require('express');
const Pool = require('pg').Pool;

const app = express();
const port = 5000;

const pool = new Pool({
    connectionString: "postgresql://viq.api:viq.api.123@localhost:5432/fortunes?sslmode=disable",
})

app.use(express.json());

app.get('/', async (request, response) => {
    var results = await pool.query("SELECT id, message FROM fortune");
    response.status(200).json(results.rows);
});

app.listen(port, () => {
    console.log(`App running on port ${port}.`)
});