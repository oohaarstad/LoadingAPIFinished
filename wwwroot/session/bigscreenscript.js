// Henter URL-parametere for sessionId
const urlParams = new URLSearchParams(window.location.search);
const sessionId = urlParams.get('sessionId');
console.log("Session ID:", sessionId); // Debugging line

// Oppretter en forbindelse til SignalR-hubben
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5142/voteHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets})
    .build();

let currentStep = 0;
let totalSteps = 0;

// Håndterer NextStep-hendelsen fra serveren
connection.on("NextStep", function (receivedSessionId, stepNumber) {
    if (receivedSessionId.toString() === sessionId.toString()) {
        if (stepNumber === 1) {
            // Første steget, vis QR-koden og session code
            document.getElementById('qrcode').style.display = 'block';
            document.getElementById('sessionCode').style.display = 'block';
        } else {
            // Skjul QR-koden og session code etter første steget
            document.getElementById('qrcode').style.display = 'none';
            document.getElementById('sessionCode').style.display = 'none';
        }
        document.getElementById('stepNumber').textContent = `Step Number: ${stepNumber}`;
        currentStep = stepNumber;
        fetch(`http://localhost:5142/api/Sessions/${sessionId}`)
            .then(response => response.json())
            .then(data => {
                totalSteps = data.scenarios.length;
                if (currentStep <= totalSteps) {
                    const scenario = data.scenarios[stepNumber - 1];
                    document.getElementById('message').textContent = `Current Scenario: ${scenario.title}`;
                    startCountdown(); // Starter nedtellingen
                } else {
                    document.getElementById('message').textContent = 'Game Over';
                    document.getElementById('countdown').textContent = '';
                    document.getElementById('results').textContent = '';
                }
            }).catch(error => {
                console.error('Error fetching session data:', error);
                alert('Failed to fetch session data. Please try again.');
            });
    }
});

// Håndterer VoteResults-hendelsen fra serveren
connection.on("VoteResults", function (voteResults) {
    console.log('Vote results received:', voteResults);
    displayResults(voteResults);
});

// Starter SignalR-forbindelsen og håndterer eventuelle feil
connection.start().catch(function (err) {
    console.error('Error establishing SignalR connection:', err.toString());
});

let countdownActive = false;

// Funksjon for å starte nedtellingen
function startCountdown() {
    countdownActive = true;
    let countdown = 10;
    const countdownElement = document.getElementById('countdown');
    countdownElement.textContent = `Time left: ${countdown} seconds`;

    const interval = setInterval(() => {
        countdown--;
        countdownElement.textContent = `Time left: ${countdown} seconds`;

        if (countdown === 0) {
            clearInterval(interval);
            countdownActive = false;
            connection.invoke("EndVoting", parseInt(sessionId)).catch(function (err) {
                console.error('Error invoking EndVoting:', err.toString());
            });
        }
    }, 1000);
}

// Funksjon for å vise stemmeresultatene
function displayResults(voteResults) {
    fetch(`http://localhost:5142/api/Sessions/${sessionId}`)
        .then(response => response.json())
        .then(data => {
            const scenario = data.scenarios[currentStep - 1];
            const allOptions = scenario.options.map(option => {
                const result = voteResults.find(vote => vote.option === option);
                return { option: option, count: result ? result.count : 0 };
            });
            if (allOptions.length > 0) {
                document.getElementById('results').innerHTML = `<h3>Results:</h3>${allOptions.map(result => `<p class="resultP">${result.option}: ${result.count} vote(s)</p>`).join('')}`;
            } else {
                document.getElementById('results').innerHTML = `<h3>Results:</h3>${scenario.options.map(option => `<p class="resultP">${option}: 0 vote(s)</p>`).join('')}`;
            }
        }).catch(error => {
            console.error('Error fetching session data:', error);
        });
}

// Funksjon for å generere en unik bruker-ID
function generateUserId() {
    return 'user-' + Math.random().toString(36).substr(2, 9);
}

// Viser QR-kode og sesjonskode
window.onload = function() {
    console.log("Generating QR code for session:", sessionId); // Debugging line
    const userId = generateUserId();
    const qrElement = document.getElementById('qrcode');
    const qrcode = new QRCode(qrElement, {
        text: `http://localhost:5142/session/index.html?sessionId=${sessionId}&userId=${userId}`,
        width: 150,
        height: 150
    });

    console.log("QRCode instance created:", qrcode); // Debugging line
    console.log("QRCode element HTML:", qrElement.innerHTML); // Debugging line

    document.getElementById('sessionCode').textContent = `${sessionId}`;
};
