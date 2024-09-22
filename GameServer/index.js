const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const app = express();
const port = 3000;

app.use(express.json());

let db = new sqlite3.Database('./leaderboard.db', sqlite3.OPEN_READWRITE | sqlite3.OPEN_CREATE, (err) => {
    if (err) {
        return console.error(err.message);
    }
    console.log('Connected to the SQLite database.');
    console.log('Database file location:', __dirname + '/leaderboard.db');
});

db.run(`CREATE TABLE IF NOT EXISTS leaderboard (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    playerName TEXT,
    score INTEGER
    )`);

app.get('/scores', (req, res) => {
    db.all(`SELECT * FROM leaderboard ORDER BY score DESC`, [], (err, rows) => {
        if (err) {
            throw err
        }
        res.json(rows);
    });
});

app.post('/scores', (req, res) => {
    const { playerName, score } = req.body;

    if (!playerName || !score) {
        return res.status(400).json({ error: 'Missing playerName or score' });
    }

    db.run(`INSERT INTO leaderboard (playerName, score) VALUES (?, ?)`, [playerName, score], function(err) {
        if (err) {
            return console.error(err.message);
        }
        res.json({ message: "Score added successfully", id: this.lastID });
    });
});

app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});
