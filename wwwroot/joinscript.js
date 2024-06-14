// Lytter til submit-hendelsen på skjemaet
document.getElementById('joinForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const sessionId = document.getElementById('sessionId').value;
    if (!sessionId) {
        alert('Please enter a session code.');
        return;
    }

    const userId = generateUserId(); // Genererer en unik bruker-ID
    document.getElementById('loading').classList.add('active');

    // Simulerer en sjekk på gyldig sesjonskode (kan utvides med API-kall for validering)
    setTimeout(() => {
        document.getElementById('loading').classList.remove('active');
        window.location.href = `/wwwroot/session/index.html?sessionId=${sessionId}&userId=${userId}`; // Omadresserer brukeren til sesjonssiden
    }, 1000); // Simulert forsinkelse
});

// Funksjon for å generere en unik bruker-ID
function generateUserId() {
    return 'user-' + Math.random().toString(36).substr(2, 9);
}
