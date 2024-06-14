// Henter URL-parametere for sessionId og userId
const urlParams = new URLSearchParams(window.location.search);
const sessionId = urlParams.get('sessionId');
const userId = urlParams.get('userId');
let hasVoted = false;
let countdownActive = true;

// Oppretter en forbindelse til SignalR-hubben
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5142/voteHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets})
    .build();

// Håndterer NextStep-hendelsen fra serveren
connection.on("NextStep", function (receivedSessionId, stepNumber) {
    if (receivedSessionId.toString() === sessionId.toString()) {
        hasVoted = false; // Nullstiller stemmestatus for det nye steget
        countdownActive = true; // Aktiverer stemmegivning
        document.getElementById('gameOver').style.display = 'none'; // Skjul Game Over-melding
        document.getElementById('stepNumber').textContent = `Step Number: ${stepNumber}`;
        fetch(`http://localhost:5142/api/Sessions/${sessionId}`)
            .then(response => response.json())
            .then(data => {
                if (stepNumber <= data.scenarios.length) {
                    const scenario = data.scenarios[stepNumber - 1];
                    const questionDiv = document.getElementById('question');
                    questionDiv.textContent = `Current Scenario: ${scenario.title}`;
                    const optionsDiv = document.getElementById('options');
                    optionsDiv.innerHTML = '';
                    scenario.options.forEach(option => {
                        const button = document.createElement('button');
                        button.textContent = option;
                        button.className = "optionBtn";
                        button.onclick = () => submitVote(option);
                        button.disabled = hasVoted; // Deaktiver knapp hvis allerede stemt
                        optionsDiv.appendChild(button);
                    });
                    startCountdown(); // Starter lokal nedtelling
                } else {
                    document.getElementById('question').textContent = '';
                    document.getElementById('options').innerHTML = '';
                    document.getElementById('countdown').textContent = '';
                    document.getElementById('gameOver').style.display = 'block'; // Vis Game Over-melding
                }
            }).catch(error => {
                console.error('Error fetching session data:', error);
                alert('Failed to fetch session data. Please try again.');
            });
    }
});

// Starter SignalR-forbindelsen og håndterer eventuelle feil
connection.start().catch(function (err) {
    console.error('Error establishing SignalR connection:', err);
});

// Funksjon for å sende inn en stemme
function submitVote(choice) {
    if (hasVoted || !countdownActive) return; // Forhindre flere stemmer og stemmegivning etter nedtellingen
    hasVoted = true;
    fetch(`http://localhost:5142/api/Votes/${sessionId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ sessionId: parseInt(sessionId), userId, choice })
    }).then(response => response.json())
    .then(result => {
        console.log('Vote submitted:', result);
        disableVoteButtons();
    }).catch(error => {
        console.error('Error submitting vote:', error);
        alert('Failed to submit vote. Please try again.');
    });
}

// Funksjon for å deaktivere alle stemmeknapper
function disableVoteButtons() {
    const buttons = document.querySelectorAll('#options button');
    buttons.forEach(button => {
        button.disabled = true;
    });
}

// Funksjon for å starte nedtellingen lokalt
function startCountdown() {
    let countdown = 10;
    const countdownElement = document.getElementById('countdown');
    countdownElement.textContent = `Time left: ${countdown} seconds`;

    const interval = setInterval(() => {
        countdown--;
        countdownElement.textContent = `Time left: ${countdown} seconds`;

        if (countdown === 0) {
            clearInterval(interval);
            countdownActive = false;
        }
    }, 1000);
}

// Gå til review
function review() {
    const urlParams = new URLSearchParams(window.location.search);
    const sessionId = urlParams.get('sessionId');
    const userId = urlParams.get('userId');
    window.location.href = `../user/review.html?sessionId=${sessionId}&userId=${userId}`;
}
