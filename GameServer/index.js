const express = require('express');
const app = express();
const port = 3000;

app.use(express.json());

let leaderboard = [];

app.get('/scores', (req, res) => {
    res.json(leaderboard);
});

app.post('/scores', (req, res) => {
    const newScore = req.body;
    leaderboard.push(newScore);
    res.json({ message: "Score added successfuly" });
});

app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});
