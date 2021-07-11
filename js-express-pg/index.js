const express = require('express');
const Pool = require('pg').Pool;

const app = express();
const port = 5000;

const pool = new Pool({
    connectionString: "postgresql://user:user@123@localhost:5432/fortunes?sslmode=disable"
})

app.use(express.json());

app.get('/text', (_, response) => {
    response.status(200).send("Hello World");
});

app.get('/json', (_, response) => {
    response.status(200).json({
        id: 1,
        message: "Hello World",
    });
});

app.get('/single', async (_, response) => {
    const results = await pool.query("SELECT id, message FROM fortune ORDER BY id");
    response.status(200).json(results.rows);
});

app.get('/multiple', async (_, response) => {
    const item = await pool.query("SELECT id, randomnumber FROM world WHERE id = 260").then(r => r.rows[0]);
    const results = await pool.query("SELECT id, randomnumber FROM world WHERE randomnumber > $1 ORDER BY id", [item.randomnumber]);
    response.status(200).json(results.rows);
});

app.listen(port, () => {
    console.log(`App running on port ${port}.`)
});
