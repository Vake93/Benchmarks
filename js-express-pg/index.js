const express = require('express');
const Pool = require('pg').Pool;

const app = express();
const port = 5000;

const pool = new Pool({
    user: 'viq.api',
    host: 'localhost',
    database: 'fortunes',
    password: 'viq.api.123',
    port: 5432,
})

app.use(express.json());
app.use(
    express.urlencoded({
        extended: true,
    })
);

app.get('/', async (request, response) => {
    var results = await pool.query("SELECT id, message FROM fortune");
    response.status(200).json(results.rows);
});

app.listen(port, () => {
    console.log(`App running on port ${port}.`)
});